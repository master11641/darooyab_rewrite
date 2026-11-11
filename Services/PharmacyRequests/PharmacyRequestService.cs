using System.Data;
using System.Globalization;
using darooyab.Models;
using darooyab_rewrite.Areas.Admin.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace darooyab_rewrite.Services.PharmacyRequests;

public sealed class PharmacyRequestService(
    darooConContext context,
    IPharmacyWalletService pharmacyWalletService) : IPharmacyRequestService
{
    private const string PatientReferralCategoryType = "patient_referral";
    private const string CountLimitCategoryType = "Exchange_Seg1_CountLimit";
    private const string VisibilityCategoryType = "Exchange_Seg1_PationRefferalView";
    private const string OpenStatus = "1";
    private const int PageSize = 30;

    public async Task<PharmacyRequestPageViewModel> GetPageAsync(
        int pharmacyAccountId,
        PharmacyRequestFormViewModel? form = null,
        int? editId = null,
        string? searchString = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var referralCategory = await GetPatientReferralCategoryAsync(cancellationToken);
        var countLimitOptions = await context.Categories
            .AsNoTracking()
            .Where(category => category.Type == CountLimitCategoryType && category.Name != null)
            .Select(category => new PharmacyRequestCategoryOption(category.CatId, category.Name!))
            .ToListAsync(cancellationToken);
        var visibilityOptions = await context.Categories
            .AsNoTracking()
            .Where(category => category.Type == VisibilityCategoryType && category.Name != null)
            .Select(category => new PharmacyRequestCategoryOption(category.CatId, category.Name!))
            .ToListAsync(cancellationToken);

        if (countLimitOptions.Count == 0 || visibilityOptions.Count == 0)
        {
            throw new InvalidOperationException(
                "Pharmacy request categories are missing from the configured database.");
        }

        var currentForm = form ?? new PharmacyRequestFormViewModel();
        if (form is null && !editId.HasValue)
        {
            currentForm.CountLimitCategoryId = countLimitOptions[0].Id;
            currentForm.VisibilityCategoryIds = visibilityOptions
                .Select(option => option.Id)
                .ToArray();
        }

        var editRequestNotFound = false;
        if (editId.HasValue)
        {
            var request = await context.PharmacyRequests
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    item => item.Id == editId.Value &&
                            item.PharmacyAccountIdFk == pharmacyAccountId &&
                            item.RequestTypeFromCategoryIdFk == referralCategory.CatId,
                    cancellationToken);

            if (request is not null)
            {
                currentForm = new PharmacyRequestFormViewModel
                {
                    Id = request.Id,
                    SelectedIrcs = request.Irc.HasValue ? [request.Irc.Value] : [],
                    DurationDays = GetRemainingPeriod(request.RequestExpireDate),
                    CountLimitCategoryId = request.CountLimit,
                    VisibilityCategoryIds = ParseCategoryIds(request.PationRefferalView),
                    Description = request.Description
                };
            }
            else
            {
                editRequestNotFound = true;
            }
        }

        currentForm.SelectedIrcs = (currentForm.SelectedIrcs ?? [])
            .Where(irc => irc > 0)
            .Distinct()
            .ToArray();

        var selectedBrandRows = currentForm.SelectedIrcs.Length == 0
            ? []
            : await context.DrugsNames
                .AsNoTracking()
                .Where(drug => drug.Irc.HasValue &&
                               currentForm.SelectedIrcs.Contains(drug.Irc.Value) &&
                               drug.DrugNameLanguage == true)
                .Select(drug => new PharmacyRequestDrugOption(
                    drug.Irc!.Value,
                    drug.DrugName ?? string.Empty))
                .ToListAsync(cancellationToken);
        var selectedBrands = selectedBrandRows
            .GroupBy(brand => brand.Irc)
            .Select(group => group.First())
            .ToList();

        var normalizedSearch = searchString?.Trim();
        var requestsQuery = context.PharmacyRequests
            .AsNoTracking()
            .Where(request =>
                request.PharmacyAccountIdFk == pharmacyAccountId &&
                request.RequestTypeFromCategoryIdFk == referralCategory.CatId);

        if (!string.IsNullOrEmpty(normalizedSearch))
        {
            requestsQuery = requestsQuery.Where(request =>
                context.DrugsNames.Any(brand =>
                    brand.Irc == request.Irc &&
                    brand.DrugNameLanguage == true &&
                    context.DrugsNames.Any(matchingName =>
                        matchingName.Irc == brand.Irc &&
                        matchingName.DrugName != null &&
                        matchingName.DrugName.Contains(normalizedSearch))));
        }

        var today = DateTime.Today;
        requestsQuery = requestsQuery.Where(request => request.RequestExpireDate.HasValue);
        var totalCount = await requestsQuery.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
        var currentPage = Math.Clamp(page, 1, totalPages);

        var requestItems = await requestsQuery
            .OrderBy(request => request.RequestExpireDate!.Value.Date < today ? 0 : 1)
            .ThenBy(request => request.RequestExpireDate!.Value.Date < today
                ? (DateTime?)null
                : request.RequestExpireDate.Value.Date)
            .ThenByDescending(request => request.RequestExpireDate!.Value.Date < today
                ? request.RequestExpireDate.Value.Date
                : (DateTime?)null)
            .Skip((currentPage - 1) * PageSize)
            .Take(PageSize)
            .Select(request => new
            {
                request.Id,
                request.Irc,
                request.RegistrationDateTime,
                request.RequestExpireDate,
                request.CountLimit
            })
            .ToListAsync(cancellationToken);

        var ircs = requestItems
            .Where(item => item.Irc.HasValue)
            .Select(item => item.Irc!.Value)
            .Distinct()
            .ToArray();
        var drugNames = ircs.Length == 0
            ? new Dictionary<long, string>()
            : await context.DrugsNames
                .AsNoTracking()
                .Where(drug => drug.Irc.HasValue &&
                               ircs.Contains(drug.Irc.Value) &&
                               drug.DrugNameLanguage == true)
                .Select(drug => new { Irc = drug.Irc!.Value, drug.DrugName })
                .ToDictionaryAsync(
                    drug => drug.Irc,
                    drug => drug.DrugName ?? string.Empty,
                    cancellationToken);

        var countLimitNames = countLimitOptions.ToDictionary(option => option.Id, option => option.Name);
        var now = DateTime.Now;
        var requests = requestItems
            .Select(item => new PharmacyRequestListItem(
                item.Id,
                item.Irc.HasValue && drugNames.TryGetValue(item.Irc.Value, out var name)
                    ? name
                    : "نام برند در دسترس نیست",
                item.RegistrationDateTime,
                item.RequestExpireDate,
                FormatPersianDate(item.RegistrationDateTime, includeTime: true),
                FormatPersianDate(item.RequestExpireDate, includeTime: false),
                item.CountLimit.HasValue && countLimitNames.TryGetValue(item.CountLimit.Value, out var label)
                    ? label
                    : "ثبت نشده",
                item.RequestExpireDate.HasValue
                    ? (int)(item.RequestExpireDate.Value.Date - now.Date).TotalDays
                    : 0))
            .ToList();
        var brandDetails = currentForm.SelectedIrcs.Length == 1
            ? await GetBrandDetailsAsync(
                pharmacyAccountId,
                currentForm.SelectedIrcs[0],
                currentForm,
                cancellationToken)
            : null;

        return new PharmacyRequestPageViewModel
        {
            Form = currentForm,
            CountLimitOptions = countLimitOptions,
            VisibilityOptions = visibilityOptions,
            SelectedBrands = selectedBrands,
            BrandDetails = brandDetails,
            AvailablePeriods = Enumerable.Range(1, 60)
                .Where(days => currentForm.Id.HasValue || days != 21)
                .ToArray(),
            Requests = requests,
            Page = currentPage,
            TotalPages = totalPages,
            SearchString = normalizedSearch,
            EditRequestNotFound = editRequestNotFound
        };
    }

    public async Task<IReadOnlyList<PharmacyRequestDrugOption>> SearchBrandsAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        var normalizedQuery = query.Trim();
        if (normalizedQuery.Length is < 2 or > 80)
        {
            return [];
        }

        return await context.DrugsNames
            .AsNoTracking()
            .Where(drug =>
                drug.Irc.HasValue &&
                drug.DrugNameLanguage == true &&
                drug.DrugName != null &&
                context.DrugsNames.Any(matchingName =>
                    matchingName.Irc == drug.Irc &&
                    matchingName.DrugName != null &&
                    matchingName.DrugName.Contains(normalizedQuery)) &&
                !context.DarooyabBrandRestrictions.Any(restriction =>
                    restriction.IsActive &&
                    restriction.BrandRestrictionType == BrandRestrictionTypeEnum.FullyProhibited &&
                    restriction.IRC == drug.Irc.Value))
            .Select(drug => new PharmacyRequestDrugOption(
                drug.Irc!.Value,
                drug.DrugName!))
            .Take(25)
            .ToListAsync(cancellationToken);
    }

    public async Task<PharmacyRequestBrandDetailsViewModel?> GetBrandDetailsAsync(
        int pharmacyAccountId,
        long irc,
        PharmacyRequestFormViewModel? form = null,
        CancellationToken cancellationToken = default)
    {
        if (irc <= 0)
        {
            return null;
        }

        var owner = await context.PharmacyAccounts
            .AsNoTracking()
            .Where(account => account.Id == pharmacyAccountId)
            .Select(account => new
            {
                PharmacyId = account.PharmacyIdFk,
                account.PharmacyIdFkNavigation.ProvinceIdFk,
                ProvinceName = account.PharmacyIdFkNavigation.ProvinceIdFkNavigation.Name
            })
            .SingleOrDefaultAsync(cancellationToken);
        if (owner is null)
        {
            return null;
        }

        var brand = await context.DrugsNames
            .AsNoTracking()
            .Where(drug => drug.Irc == irc && drug.DrugNameLanguage == true)
            .OrderBy(drug => drug.Id)
            .Select(drug => new { drug.Irc, drug.DrugName })
            .FirstOrDefaultAsync(cancellationToken);
        if (brand?.Irc is null)
        {
            return null;
        }

        var isProhibited = await context.DarooyabBrandRestrictions.AnyAsync(
            restriction =>
                restriction.IsActive &&
                restriction.BrandRestrictionType == BrandRestrictionTypeEnum.FullyProhibited &&
                restriction.IRC == irc,
            cancellationToken);
        if (isProhibited)
        {
            return null;
        }

        var now = DateTime.Now;
        var activeRanks = await context.RankingTransactions
            .AsNoTracking()
            .Where(rank =>
                rank.Irc == irc &&
                rank.ProvinceId == owner.ProvinceIdFk &&
                rank.StartDate <= now &&
                rank.EndDate >= now)
            .OrderBy(rank => rank.RankNumber)
            .ThenBy(rank => rank.StartDate)
            .Select(rank => new
            {
                rank.RankNumber,
                rank.PharmacyAccountId,
                rank.PharmacyId,
                rank.EndDate,
                rank.Amount,
                rank.DurationInDays
            })
            .ToListAsync(cancellationToken);

        var rankPharmacyIds = activeRanks
            .Select(rank => rank.PharmacyId)
            .Distinct()
            .ToArray();
        var rankPharmacyNames = rankPharmacyIds.Length == 0
            ? new Dictionary<int, string>()
            : await context.Pharmacies
                .AsNoTracking()
                .Where(pharmacy => rankPharmacyIds.Contains(pharmacy.Id))
                .Select(pharmacy => new { pharmacy.Id, pharmacy.Name })
                .ToDictionaryAsync(
                    pharmacy => pharmacy.Id,
                    pharmacy => pharmacy.Name,
                    cancellationToken);

        var ownedRank = activeRanks.FirstOrDefault(rank => rank.PharmacyAccountId == pharmacyAccountId);
        var availableRank = ownedRank is null
            ? Enumerable.Range(1, 10)
                .FirstOrDefault(rank => activeRanks.All(active => active.RankNumber != rank))
            : 0;

        var latestPrice = await context.TblBrandAttributes
            .AsNoTracking()
            .Where(attribute => attribute.TblBrandAttributeIrc == irc)
            .OrderByDescending(attribute => attribute.LastPriceEditDate)
            .ThenByDescending(attribute => attribute.TblBrandAttributeId)
            .Select(attribute => new
            {
                attribute.Price,
                attribute.PriceUnitNum,
                attribute.LastPriceEditorPharmacyFk,
                attribute.LastPriceEditDate
            })
            .FirstOrDefaultAsync(cancellationToken);

        string? lastPriceEditorName = null;
        string? lastPriceEditorCity = null;
        if (latestPrice?.LastPriceEditorPharmacyFk is int editorPharmacyId)
        {
            var editorPharmacy = await context.Pharmacies
                .AsNoTracking()
                .Where(pharmacy => pharmacy.Id == editorPharmacyId)
                .Select(pharmacy => new
                {
                    pharmacy.Name,
                    CityName = pharmacy.CityIdFkNavigation.Name
                })
                .SingleOrDefaultAsync(cancellationToken);
            lastPriceEditorName = editorPharmacy?.Name;
            lastPriceEditorCity = editorPharmacy?.CityName;
        }

        var walletBalanceRial = await pharmacyWalletService.GetBalanceInRialAsync(
            pharmacyAccountId,
            cancellationToken);

        return new PharmacyRequestBrandDetailsViewModel
        {
            Irc = irc,
            BrandName = brand.DrugName ?? string.Empty,
            ProvinceName = owner.ProvinceName ?? "استان ثبت نشده",
            ActiveRankings = activeRanks
                .Select(rank => new PharmacyRequestActiveRankViewModel(
                    rank.RankNumber,
                    rankPharmacyNames.GetValueOrDefault(rank.PharmacyId) ?? "داروخانه",
                    FormatPersianDate(rank.EndDate, includeTime: false),
                    rank.DurationInDays > 0
                        ? rank.Amount / (rank.DurationInDays * 10)
                        : 0))
                .ToArray(),
            AvailableRankNumber = availableRank is > 0 ? (short)availableRank : null,
            DailyPriceToman = availableRank is > 0 ? PharmacyRankingPricing.GetDailyPriceToman(availableRank) : null,
            OwnedRankNumber = ownedRank?.RankNumber,
            OwnedRankExpiresAtDisplay = ownedRank is not null
                ? FormatPersianDate(ownedRank.EndDate, includeTime: false)
                : null,
            WalletBalanceToman = Math.Max(0, walletBalanceRial) / 10,
            CurrentPrice = latestPrice?.Price,
            CurrentPriceUnitNumber = latestPrice?.PriceUnitNum,
            LastPriceEditorName = lastPriceEditorName,
            LastPriceEditorCity = lastPriceEditorCity,
            LastPriceEditDateDisplay = latestPrice?.LastPriceEditDate is DateTime lastPriceEditDate
                ? FormatPersianDate(lastPriceEditDate, includeTime: true)
                : null,
            IsEditMode = form?.Id.HasValue == true,
            BuyRanking = form?.BuyRanking == true,
            PurchaseRankDays = form?.PurchaseRankDays,
            UpdateBrandPrice = form?.UpdateBrandPrice == true,
            SubmittedBrandPrice = form?.BrandPrice,
            SubmittedBrandPriceUnitNumber = form?.BrandPriceUnitNumber
        };
    }

    public async Task<PharmacyRequestSaveStatus> SaveAsync(
        int pharmacyAccountId,
        PharmacyRequestFormViewModel form,
        CancellationToken cancellationToken = default)
    {
        var selectedIrcs = (form.SelectedIrcs ?? [])
            .Where(irc => irc > 0)
            .Distinct()
            .ToArray();
        var visibilityIds = (form.VisibilityCategoryIds ?? [])
            .Where(id => id > 0)
            .Distinct()
            .ToArray();

        if (selectedIrcs.Length == 0 ||
            (form.Id.HasValue && selectedIrcs.Length != 1) ||
            (form.BuyRanking &&
             (selectedIrcs.Length != 1 ||
              !form.PurchaseRankNumber.HasValue ||
              form.PurchaseRankNumber is < 1 or > 10 ||
              !form.PurchaseRankDays.HasValue ||
              form.PurchaseRankDays is < 1 or > 60)) ||
            form.DurationDays is < 1 or > 60 ||
            form.Description is { Length: > 1000 } ||
            !form.CountLimitCategoryId.HasValue ||
            visibilityIds.Length == 0)
        {
            return PharmacyRequestSaveStatus.InvalidInput;
        }

        if (form.UpdateBrandPrice &&
            (form.Id.HasValue ||
             selectedIrcs.Length != 1 ||
             !form.BrandPrice.HasValue ||
             form.BrandPrice is < 0 or > 999999999999999999m ||
             decimal.Truncate(form.BrandPrice.Value) != form.BrandPrice.Value ||
             form.BrandPriceUnitNumber is not (1 or 2)))
        {
            return PharmacyRequestSaveStatus.InvalidBrandPrice;
        }

        var referralCategory = await GetPatientReferralCategoryAsync(cancellationToken);
        var countLimitExists = await context.Categories.AnyAsync(
            category => category.CatId == form.CountLimitCategoryId.Value &&
                        category.Type == CountLimitCategoryType,
            cancellationToken);
        var validVisibilityIds = await context.Categories
            .AsNoTracking()
            .Where(category =>
                visibilityIds.Contains(category.CatId) &&
                category.Type == VisibilityCategoryType)
            .Select(category => category.CatId)
            .ToListAsync(cancellationToken);

        if (!countLimitExists || validVisibilityIds.Count != visibilityIds.Length)
        {
            return PharmacyRequestSaveStatus.InvalidInput;
        }

        var validBrands = await context.DrugsNames
            .AsNoTracking()
            .Where(drug =>
                drug.Irc.HasValue &&
                selectedIrcs.Contains(drug.Irc.Value) &&
                drug.DrugNameLanguage == true)
            .Select(drug => drug.Irc!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (validBrands.Count != selectedIrcs.Length)
        {
            return PharmacyRequestSaveStatus.InvalidInput;
        }

        var hasProhibitedBrand = await context.DarooyabBrandRestrictions.AnyAsync(
            restriction =>
                restriction.IsActive &&
                restriction.BrandRestrictionType == BrandRestrictionTypeEnum.FullyProhibited &&
                selectedIrcs.Contains(restriction.IRC),
            cancellationToken);
        if (hasProhibitedBrand)
        {
            return PharmacyRequestSaveStatus.RestrictedBrand;
        }

        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        PharmacyRequest? editedRequest = null;
        if (form.Id.HasValue)
        {
            editedRequest = await context.PharmacyRequests.SingleOrDefaultAsync(
                request => request.Id == form.Id.Value &&
                           request.PharmacyAccountIdFk == pharmacyAccountId &&
                           request.RequestTypeFromCategoryIdFk == referralCategory.CatId &&
                           request.Irc == selectedIrcs[0],
                cancellationToken);
            if (editedRequest is null)
            {
                return PharmacyRequestSaveStatus.NotFound;
            }
        }

        var owner = await context.PharmacyAccounts
            .Where(account => account.Id == pharmacyAccountId)
            .Select(account => new
            {
                PharmacyId = account.PharmacyIdFk,
                account.PharmacyIdFkNavigation.ProvinceIdFk
            })
            .SingleOrDefaultAsync(cancellationToken);
        if (owner is null)
        {
            return PharmacyRequestSaveStatus.NotFound;
        }

        var visibilityCsv = string.Join(",", visibilityIds);
        var now = DateTime.Now;
        var effectiveDurationDays = form.DurationDays;

        if (form.BuyRanking)
        {
            var activeRanks = await context.RankingTransactions
                .Where(rank =>
                    rank.Irc == selectedIrcs[0] &&
                    rank.ProvinceId == owner.ProvinceIdFk &&
                    rank.StartDate <= now &&
                    rank.EndDate >= now)
                .Select(rank => new { rank.RankNumber, rank.PharmacyAccountId })
                .ToListAsync(cancellationToken);

            if (activeRanks.Any(rank => rank.PharmacyAccountId == pharmacyAccountId))
            {
                return PharmacyRequestSaveStatus.RankAlreadyOwned;
            }

            var nextAvailableRank = Enumerable.Range(1, 10)
                .FirstOrDefault(rank => activeRanks.All(active => active.RankNumber != rank));
            if (nextAvailableRank == 0 ||
                form.PurchaseRankNumber != nextAvailableRank)
            {
                return PharmacyRequestSaveStatus.RankUnavailable;
            }

            var rankingCostRial = PharmacyRankingPricing.GetPriceRial(
                nextAvailableRank,
                form.PurchaseRankDays!.Value);
            var balanceRial = await pharmacyWalletService.GetBalanceInRialAsync(
                pharmacyAccountId,
                cancellationToken);
            if (rankingCostRial > balanceRial)
            {
                return PharmacyRequestSaveStatus.InsufficientBalance;
            }

            effectiveDurationDays = Math.Max(
                effectiveDurationDays,
                form.PurchaseRankDays.Value);

            context.RankingTransactions.Add(new RankingTransaction
            {
                RegisterDate = now,
                RankNumber = (short)nextAvailableRank,
                StartDate = now,
                EndDate = now.AddDays(form.PurchaseRankDays.Value),
                PharmacyAccountId = pharmacyAccountId,
                PharmacyId = owner.PharmacyId,
                Amount = checked((int)rankingCostRial),
                Irc = selectedIrcs[0],
                ProvinceId = owner.ProvinceIdFk,
                DurationInDays = form.PurchaseRankDays.Value
            });
        }

        if (form.UpdateBrandPrice)
        {
            var latestPrice = await context.TblBrandAttributes
                .Where(attribute => attribute.TblBrandAttributeIrc == selectedIrcs[0])
                .OrderByDescending(attribute => attribute.LastPriceEditDate)
                .ThenByDescending(attribute => attribute.TblBrandAttributeId)
                .FirstOrDefaultAsync(cancellationToken);

            if (latestPrice is null)
            {
                context.TblBrandAttributes.Add(new TblBrandAttribute
                {
                    TblBrandAttributeIrc = selectedIrcs[0],
                    Price = form.BrandPrice!.Value,
                    PriceUnitNum = form.BrandPriceUnitNumber,
                    LastPriceEditorPharmacyFk = owner.PharmacyId,
                    LastPriceEditDate = now
                });
            }
            else
            {
                latestPrice.Price = form.BrandPrice!.Value;
                latestPrice.PriceUnitNum = form.BrandPriceUnitNumber;
                latestPrice.LastPriceEditorPharmacyFk = owner.PharmacyId;
                latestPrice.LastPriceEditDate = now;
            }
        }

        foreach (var irc in selectedIrcs)
        {
            var request = editedRequest;
            if (request is null)
            {
                var existing = await context.PharmacyRequests
                    .Where(item =>
                        item.PharmacyAccountIdFk == pharmacyAccountId &&
                        item.RequestTypeFromCategoryIdFk == referralCategory.CatId &&
                        item.Irc == irc &&
                        item.StatusNum == OpenStatus)
                    .Take(2)
                    .ToListAsync(cancellationToken);

                if (existing.Count > 1)
                {
                    return PharmacyRequestSaveStatus.Conflict;
                }

                request = existing.SingleOrDefault();
                if (request is null)
                {
                    request = new PharmacyRequest
                    {
                        PharmacyAccountIdFk = pharmacyAccountId,
                        RequestTypeFromCategoryIdFk = referralCategory.CatId,
                        Irc = irc
                    };
                    context.PharmacyRequests.Add(request);
                }
            }

            request.RequestExpireDate = now.AddDays(effectiveDurationDays);
            request.CountLimit = form.CountLimitCategoryId;
            request.PationRefferalView = visibilityCsv;
            request.Description = form.Description?.Trim();
            request.RegistrationDateTime = now;
            request.DrugExpireDate = null;
            request.StatusNum = OpenStatus;

            if (form.Id.HasValue && request.Irc.HasValue)
            {
                await ShortenActiveRankingWithReferralAsync(
                    request,
                    cancellationToken);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return PharmacyRequestSaveStatus.Succeeded;
    }

    public async Task<bool> DeleteAsync(
        int pharmacyAccountId,
        int requestId,
        CancellationToken cancellationToken = default)
    {
        var referralCategory = await GetPatientReferralCategoryAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);
        var request = await context.PharmacyRequests.SingleOrDefaultAsync(
            item => item.Id == requestId &&
                    item.PharmacyAccountIdFk == pharmacyAccountId &&
                    item.RequestTypeFromCategoryIdFk == referralCategory.CatId,
            cancellationToken);

        if (request is null)
        {
            return false;
        }

        if (request.Irc.HasValue)
        {
            var now = DateTime.Now;
            var activeRank = await context.RankingTransactions
                .Where(rank =>
                    rank.Irc == request.Irc.Value &&
                    rank.PharmacyAccountId == pharmacyAccountId &&
                    rank.StartDate <= now &&
                    rank.EndDate >= now)
                .OrderByDescending(rank => rank.StartDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (activeRank is not null)
            {
                var drugName = await context.DrugsNames
                    .AsNoTracking()
                    .Where(drug => drug.Irc == request.Irc && drug.DrugNameLanguage == true)
                    .OrderBy(drug => drug.Id)
                    .Select(drug => drug.DrugName)
                    .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;
                var elapsedDays = Math.Max(
                    1,
                    (int)Math.Ceiling((now - activeRank.StartDate).TotalDays));
                activeRank.ModifyDescription =
                    $"{activeRank.ModifyDescription} اعتبار رتبه {activeRank.RankNumber} در تاریخ " +
                    $"{FormatPersianDate(now, includeTime: true)} به علت حذف درخواست داروی {drugName} پایان یافت.";
                activeRank.DurationInDays = elapsedDays;
                activeRank.EndDate = now;
            }
        }

        context.PharmacyRequests.Remove(request);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    private async Task ShortenActiveRankingWithReferralAsync(
        PharmacyRequest request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var activeRank = await context.RankingTransactions
            .Where(rank =>
                rank.Irc == request.Irc &&
                rank.PharmacyAccountId == request.PharmacyAccountIdFk &&
                rank.StartDate <= now &&
                rank.EndDate >= now)
            .OrderByDescending(rank => rank.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
        if (activeRank is null ||
            !request.RequestExpireDate.HasValue ||
            request.RequestExpireDate.Value.Date >= activeRank.EndDate.Date)
        {
            return;
        }

        var requestedDays = (request.RequestExpireDate.Value - activeRank.StartDate).Days;
        var drugName = await context.DrugsNames
            .AsNoTracking()
            .Where(drug => drug.Irc == request.Irc && drug.DrugNameLanguage == true)
            .OrderBy(drug => drug.Id)
            .Select(drug => drug.DrugName)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;
        activeRank.ModifyDescription =
            $"{activeRank.ModifyDescription} {FormatPersianDate(now, includeTime: true)} " +
            $"به دلیل ویرایش درخواست برند {drugName}، مدت رتبه از " +
            $"{activeRank.DurationInDays} روز به {requestedDays} روز تغییر کرد.";
        activeRank.DurationInDays = requestedDays;
        activeRank.EndDate = request.RequestExpireDate.Value;
    }

    private async Task<Category> GetPatientReferralCategoryAsync(CancellationToken cancellationToken)
    {
        return await context.Categories.SingleOrDefaultAsync(
                   category => category.Type == PatientReferralCategoryType,
                   cancellationToken)
               ?? throw new InvalidOperationException(
                   "The patient referral category is missing from the configured database.");
    }

    private static int[] ParseCategoryIds(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        return value.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(item => int.TryParse(item, out var id) ? id : 0)
            .Where(id => id > 0)
            .Distinct()
            .ToArray();
    }

    private static int GetRemainingPeriod(DateTime? expiresAt)
    {
        if (!expiresAt.HasValue)
        {
            return 20;
        }

        return Math.Clamp(
            (int)Math.Ceiling((expiresAt.Value - DateTime.Now).TotalDays),
            1,
            60);
    }

    private static string FormatPersianDate(DateTime? value, bool includeTime)
    {
        if (!value.HasValue)
        {
            return "نامشخص";
        }

        var calendar = new PersianCalendar();
        var date = value.Value;
        var dateText = string.Create(
            CultureInfo.InvariantCulture,
            $"{calendar.GetYear(date):0000}/{calendar.GetMonth(date):00}/{calendar.GetDayOfMonth(date):00}");
        return includeTime
            ? $"{dateText} {date.Hour:00}:{date.Minute:00}:{date.Second:00}"
            : dateText;
    }
}
