using System.Globalization;
using darooyab.Models;
using darooyab_rewrite.Areas.Admin.ViewModels;
using darooyab_rewrite.Utilities;
using Microsoft.EntityFrameworkCore;

namespace darooyab_rewrite.Services.PharmacyDrugShortages;

public sealed class PharmacyDrugShortageService(darooConContext context) : IPharmacyDrugShortageService
{
    private const string ShortageCategoryType = "drug_shortages";
    private const string ExcessCategoryType = "excess_drugs";
    private const string ProvinceCategoryType = "province";
    private const string TownshipCategoryType = "township";
    private const string OpenStatus = "1";
    private const string NearDateExchangeLimit = "3";
    private const int PageSize = 10;

    public async Task<PharmacyDrugShortagePageViewModel> GetPageAsync(
        int pharmacyAccountId,
        PharmacyDrugShortageFormViewModel? form = null,
        int? editId = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var shortageCategoryId = await GetShortageCategoryIdAsync(cancellationToken);
        var pharmacyLocation = await context.PharmacyAccounts
            .AsNoTracking()
            .Where(account => account.Id == pharmacyAccountId)
            .Select(account => new
            {
                ProvinceId = account.PharmacyIdFkNavigation.ProvinceIdFk,
                TownshipId = account.PharmacyIdFkNavigation.CityIdFk
            })
            .SingleAsync(cancellationToken);

        var provinces = await context.Categories
            .AsNoTracking()
            .Where(category => category.Type != null &&
                               category.Type.ToLower() == ProvinceCategoryType &&
                               category.Name != null)
            .Select(category => new PharmacyDrugShortageOption(category.CatId, category.Name!))
            .ToListAsync(cancellationToken);

        var townships = await GetTownshipsForProvinceAsync(
            pharmacyLocation.ProvinceId,
            cancellationToken);

        var currentForm = form ?? new PharmacyDrugShortageFormViewModel();
        PharmacyDrugShortageOption? selectedDrug = null;
        var editRequestNotFound = false;
        if (editId.HasValue)
        {
            var request = await context.PharmacyRequests
                .AsNoTracking()
                .Where(item => item.Id == editId.Value &&
                               item.PharmacyAccountIdFk == pharmacyAccountId &&
                               item.RequestTypeFromCategoryIdFk == shortageCategoryId)
                .Select(item => new
                {
                    item.Id,
                    item.Irc,
                    item.DrugShapFromDosageIdFk
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (request is not null)
            {
                currentForm = new PharmacyDrugShortageFormViewModel
                {
                    Id = request.Id,
                    SearchType = request.DrugShapFromDosageIdFk.HasValue ? 1 : 2,
                    SearchProperty = request.DrugShapFromDosageIdFk.HasValue
                        ? request.DrugShapFromDosageIdFk.Value
                        : request.Irc
                };
            }
            else
            {
                editRequestNotFound = true;
            }
        }

        currentForm.SearchType = currentForm.SearchType is 1 or 2 ? currentForm.SearchType : 2;
        if (currentForm.SearchProperty is > 0)
        {
            selectedDrug = await GetSelectedDrugAsync(
                currentForm.SearchType,
                currentForm.SearchProperty.Value,
                cancellationToken);
        }

        var requestsQuery = context.PharmacyRequests
            .AsNoTracking()
            .Where(item =>
                item.PharmacyAccountIdFk == pharmacyAccountId &&
                item.RequestTypeFromCategoryIdFk == shortageCategoryId &&
                item.StatusNum == OpenStatus);

        var totalCount = await requestsQuery.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
        var currentPage = Math.Clamp(page, 1, totalPages);
        var requestRows = await requestsQuery
            .OrderByDescending(item => item.Id)
            .Skip((currentPage - 1) * PageSize)
            .Take(PageSize)
            .Select(item => new
            {
                item.Id,
                item.Irc,
                item.DrugShapFromDosageIdFk,
                item.RegistrationDateTime
            })
            .ToListAsync(cancellationToken);

        var ircs = requestRows
            .Where(item => item.Irc.HasValue)
            .Select(item => item.Irc!.Value)
            .Distinct()
            .ToArray();
        var dosageFormIds = requestRows
            .Where(item => item.DrugShapFromDosageIdFk.HasValue)
            .Select(item => item.DrugShapFromDosageIdFk!.Value)
            .Distinct()
            .ToArray();

        var brandNameRows = await context.DrugsNames
            .AsNoTracking()
            .Where(drug => drug.Irc.HasValue &&
                           ircs.Contains(drug.Irc.Value) &&
                           drug.DrugNameLanguage == true)
            .Select(drug => new { Irc = drug.Irc!.Value, drug.DrugName })
            .ToListAsync(cancellationToken);
        var brandNames = brandNameRows
            .GroupBy(item => item.Irc)
            .ToDictionary(group => group.Key, group => group.First().DrugName ?? string.Empty);

        var dosageFormNameRows = await context.DosageForms
            .AsNoTracking()
            .Where(dosage => dosageFormIds.Contains(dosage.DosageFormId))
            .Select(dosage => new { dosage.DosageFormId, dosage.DosageFormName })
            .ToListAsync(cancellationToken);
        var dosageFormNames = dosageFormNameRows
            .ToDictionary(item => item.DosageFormId, item => item.DosageFormName ?? string.Empty);

        return new PharmacyDrugShortagePageViewModel
        {
            Form = currentForm,
            SelectedDrug = selectedDrug,
            Requests = requestRows.Select(item =>
            {
                var name = item.Irc.HasValue
                    ? brandNames.GetValueOrDefault(item.Irc.Value)
                    : item.DrugShapFromDosageIdFk.HasValue
                        ? dosageFormNames.GetValueOrDefault(item.DrugShapFromDosageIdFk.Value)
                        : null;
                return new PharmacyDrugShortageListItem(
                    item.Id,
                    name ?? "نام دارو در دسترس نیست",
                    item.RegistrationDateTime,
                    PersianDateFormatter.Format(item.RegistrationDateTime));
            }).ToArray(),
            Provinces = provinces,
            Townships = townships,
            SelectedProvinceId = pharmacyLocation.ProvinceId,
            SelectedTownshipId = pharmacyLocation.TownshipId,
            Page = currentPage,
            TotalPages = totalPages,
            EditRequestNotFound = editRequestNotFound
        };
    }

    public async Task<IReadOnlyList<PharmacyDrugShortageOption>> SearchAsync(
        int searchType,
        string query,
        CancellationToken cancellationToken = default)
    {
        var normalizedQuery = NormalizeSearch(query);
        if (normalizedQuery.Length is < 2 or > 80 || searchType is not (1 or 2))
        {
            return [];
        }

        if (searchType == 1)
        {
            return await context.DosageForms
                .AsNoTracking()
                .Where(dosage =>
                    dosage.DosageFormName != null &&
                    ((dosage.DosageFormNameFa != null &&
                      dosage.DosageFormNameFa.Contains(normalizedQuery)) ||
                     dosage.DosageFormName.Contains(normalizedQuery) ||
                     (dosage.FdoGenerationCode.HasValue &&
                      dosage.FdoGenerationCode.Value.ToString().Contains(normalizedQuery))))
                .Select(dosage => new PharmacyDrugShortageOption(
                    dosage.DosageFormId,
                    dosage.DosageFormName ?? dosage.DosageFormNameFa ?? string.Empty))
                .Take(25)
                .ToListAsync(cancellationToken);
        }

        var matchingIrcs = context.DrugsNames
            .AsNoTracking()
            .Where(drug => drug.Irc.HasValue &&
                           drug.DrugName != null &&
                           drug.DrugName.ToLower().Contains(normalizedQuery.ToLower()))
            .Select(drug => drug.Irc!.Value)
            .Distinct();

        var brandRows = await context.DrugsNames
            .AsNoTracking()
            .Where(drug =>
                drug.Irc.HasValue &&
                drug.DrugNameLanguage == true &&
                matchingIrcs.Contains(drug.Irc.Value) &&
                !context.DarooyabBrandRestrictions.Any(restriction =>
                    restriction.IsActive &&
                    restriction.BrandRestrictionType == BrandRestrictionTypeEnum.FullyProhibited &&
                    restriction.IRC == drug.Irc.Value))
            .Select(drug => new PharmacyDrugShortageOption(
                drug.Irc!.Value,
                drug.DrugName ?? string.Empty))
            .Take(100)
            .ToListAsync(cancellationToken);

        return brandRows
            .GroupBy(option => option.Id)
            .Select(group => group.First())
            .Take(25)
            .ToArray();
    }

    public async Task<IReadOnlyList<PharmacyDrugShortageOption>> GetTownshipsAsync(
        int provinceId,
        CancellationToken cancellationToken = default)
    {
        var isProvince = await context.Categories
            .AsNoTracking()
            .AnyAsync(category =>
                category.CatId == provinceId &&
                category.Type != null &&
                category.Type.ToLower() == ProvinceCategoryType,
                cancellationToken);
        return isProvince
            ? await GetTownshipsForProvinceAsync(provinceId, cancellationToken)
            : [];
    }

    public async Task<PharmacyNearDatePageViewModel> GetNearDatePageAsync(
        int pharmacyAccountId,
        int provinceId,
        int townshipId,
        int page,
        CancellationToken cancellationToken = default)
    {
        var provinces = await context.Categories
            .AsNoTracking()
            .Where(category => category.Type != null &&
                               category.Type.ToLower() == ProvinceCategoryType &&
                               category.Name != null)
            .Select(category => new PharmacyDrugShortageOption(category.CatId, category.Name!))
            .ToListAsync(cancellationToken);

        var invalidLocationFilter = false;
        if (provinceId < 0)
        {
            provinceId = 0;
            invalidLocationFilter = true;
        }

        if (townshipId < 0)
        {
            townshipId = 0;
            invalidLocationFilter = true;
        }

        if (provinceId > 0 && !provinces.Any(province => province.Id == provinceId))
        {
            provinceId = 0;
            townshipId = 0;
            invalidLocationFilter = true;
        }

        var townships = provinceId > 0
            ? await GetTownshipsForProvinceAsync(provinceId, cancellationToken)
            : [];
        if (townshipId > 0 && !townships.Any(township => township.Id == townshipId))
        {
            townshipId = 0;
            invalidLocationFilter = true;
        }

        var today = DateTime.Today;
        var requestsQuery = context.PharmacyRequests
            .AsNoTracking()
            .Where(request =>
                request.RequestTypeFromCategoryIdFkNavigation != null &&
                request.RequestTypeFromCategoryIdFkNavigation.Type == ExcessCategoryType &&
                request.PharmacyAccountIdFk.HasValue &&
                request.PharmacyAccountIdFk.Value != pharmacyAccountId &&
                request.ExchangeLimitNum == NearDateExchangeLimit &&
                request.DrugExpireDate.HasValue &&
                request.DrugExpireDate.Value >= today &&
                request.StatusNum == OpenStatus);

        if (townshipId > 0)
        {
            requestsQuery = requestsQuery.Where(request =>
                request.PharmacyAccountIdFkNavigation!.PharmacyIdFkNavigation.CityIdFk == townshipId);
        }
        else if (provinceId > 0)
        {
            requestsQuery = requestsQuery.Where(request =>
                request.PharmacyAccountIdFkNavigation!.PharmacyIdFkNavigation.ProvinceIdFk == provinceId);
        }

        var totalCount = await requestsQuery.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
        var currentPage = totalPages == 0 ? 1 : Math.Clamp(page, 1, totalPages);

        var rows = await (
                from request in requestsQuery
                join drug in context.DrugsNames.AsNoTracking()
                    .Where(item => item.DrugNameLanguage == true)
                    on request.Irc equals drug.Irc
                orderby request.RegistrationDateTime descending
                select new
                {
                    request.Id,
                    DrugName = drug.DrugName,
                    PharmacyName = request.PharmacyAccountIdFkNavigation!.PharmacyIdFkNavigation.Name,
                    ProvinceName = request.PharmacyAccountIdFkNavigation!.PharmacyIdFkNavigation.ProvinceIdFkNavigation.Name,
                    TownshipName = request.PharmacyAccountIdFkNavigation!.PharmacyIdFkNavigation.CityIdFkNavigation.Name,
                    Address = request.PharmacyAccountIdFkNavigation!.PharmacyIdFkNavigation.Address,
                    Phone = request.PharmacyAccountIdFkNavigation!.PharmacyIdFkNavigation.Phone,
                    request.RegistrationDateTime,
                    request.DrugExpireDate,
                    RemainingCount = request.DrugCount.HasValue
                        ? (int?)(request.DrugCount.Value -
                          (context.PharmacyExchanges
                              .Where(exchange => exchange.ExcessPhReqIdFk == request.Id)
                              .Select(exchange => (int?)exchange.RequireCount)
                              .Sum() ?? 0))
                        : null,
                    request.DrugUnitNum,
                    request.UnitPrice
                })
                .Skip((currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync(cancellationToken);

        var items = rows
            .Select(row => new PharmacyNearDateItem(
                row.Id,
                row.DrugName ?? string.Empty,
                row.PharmacyName ?? string.Empty,
                row.ProvinceName,
                row.TownshipName,
                row.Address,
                row.Phone,
                row.RegistrationDateTime,
                FormatRelativeTime(row.RegistrationDateTime),
                row.DrugExpireDate,
                row.RemainingCount,
                row.DrugUnitNum?.Trim() switch
                {
                    "1" => "عدد",
                    "2" => "بسته",
                    _ => string.Empty
                },
                row.UnitPrice))
            .ToArray();

        return new PharmacyNearDatePageViewModel
        {
            Provinces = provinces,
            Townships = townships,
            Items = items,
            SelectedProvinceId = provinceId,
            SelectedTownshipId = townshipId,
            Page = currentPage,
            TotalPages = totalPages,
            TotalCount = totalCount,
            InvalidLocationFilter = invalidLocationFilter
        };
    }

    public async Task<PharmacyDrugShortageSaveStatus> SaveAsync(
        int pharmacyAccountId,
        PharmacyDrugShortageFormViewModel form,
        CancellationToken cancellationToken = default)
    {
        if (form.SearchType is not (1 or 2) || form.SearchProperty is not > 0)
        {
            return PharmacyDrugShortageSaveStatus.InvalidInput;
        }

        var shortageCategoryId = await GetShortageCategoryIdAsync(cancellationToken);
        var existing = form.Id.HasValue
            ? await context.PharmacyRequests.SingleOrDefaultAsync(
                item => item.Id == form.Id.Value &&
                        item.PharmacyAccountIdFk == pharmacyAccountId &&
                        item.RequestTypeFromCategoryIdFk == shortageCategoryId,
                cancellationToken)
            : null;
        if (form.Id.HasValue && existing is null)
        {
            return PharmacyDrugShortageSaveStatus.NotFound;
        }

        if (form.SearchType == 1)
        {
            if (form.SearchProperty.Value > int.MaxValue)
            {
                return PharmacyDrugShortageSaveStatus.InvalidInput;
            }

            var dosageFormExists = await context.DosageForms
                .AnyAsync(dosage => dosage.DosageFormId == form.SearchProperty.Value, cancellationToken);
            if (!dosageFormExists)
            {
                return PharmacyDrugShortageSaveStatus.InvalidInput;
            }
        }
        else
        {
            var brandExists = await context.DrugsNames
                .AnyAsync(drug =>
                    drug.Irc == form.SearchProperty.Value &&
                    drug.DrugNameLanguage == true,
                    cancellationToken);
            if (!brandExists)
            {
                return PharmacyDrugShortageSaveStatus.InvalidInput;
            }

            var isProhibited = await context.DarooyabBrandRestrictions.AnyAsync(
                restriction =>
                    restriction.IsActive &&
                    restriction.BrandRestrictionType == BrandRestrictionTypeEnum.FullyProhibited &&
                    restriction.IRC == form.SearchProperty.Value,
                cancellationToken);
            if (isProhibited)
            {
                return PharmacyDrugShortageSaveStatus.RestrictedBrand;
            }
        }

        var request = existing ?? new PharmacyRequest();
        request.PharmacyAccountIdFk = pharmacyAccountId;
        request.RequestTypeFromCategoryIdFk = shortageCategoryId;
        request.RequestView = 0;
        request.RegistrationDateTime = DateTime.Now;
        request.RequestExpireDate = null;
        request.DrugExpireDate = null;
        request.StatusNum = OpenStatus;
        request.Irc = form.SearchType == 2 ? form.SearchProperty.Value : null;
        request.DrugShapFromDosageIdFk = form.SearchType == 1
            ? checked((int)form.SearchProperty.Value)
            : null;

        if (existing is null)
        {
            await context.PharmacyRequests.AddAsync(request, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
        return PharmacyDrugShortageSaveStatus.Succeeded;
    }

    public async Task<PharmacyDrugShortageDeleteStatus> DeleteAsync(
        int pharmacyAccountId,
        int requestId,
        CancellationToken cancellationToken = default)
    {
        var shortageCategoryId = await GetShortageCategoryIdAsync(cancellationToken);
        var request = await context.PharmacyRequests.SingleOrDefaultAsync(
            item => item.Id == requestId &&
                    item.PharmacyAccountIdFk == pharmacyAccountId &&
                    item.RequestTypeFromCategoryIdFk == shortageCategoryId,
            cancellationToken);
        if (request is null)
        {
            return PharmacyDrugShortageDeleteStatus.NotFound;
        }

        var hasExchanges = await context.PharmacyExchanges.AnyAsync(
            exchange => exchange.ShortagePhReqIdFk == requestId ||
                        exchange.ExcessPhReqIdFk == requestId,
            cancellationToken);
        if (hasExchanges)
        {
            return PharmacyDrugShortageDeleteStatus.HasExchanges;
        }

        context.PharmacyRequests.Remove(request);
        await context.SaveChangesAsync(cancellationToken);
        return PharmacyDrugShortageDeleteStatus.Succeeded;
    }

    public async Task<PharmacyDrugShortageOffersViewModel?> GetOffersAsync(
        int pharmacyAccountId,
        PharmacyDrugShortageSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.DisplayType is not ("NormalExchange" or "StarExchange" or "NearDate") ||
            request.CityId > 0 && request.ProvinceId == 0)
        {
            return null;
        }

        var shortageCategoryId = await GetShortageCategoryIdAsync(cancellationToken);
        var shortage = await context.PharmacyRequests
            .AsNoTracking()
            .Where(item =>
                item.Id == request.PharmacyShortageId &&
                item.PharmacyAccountIdFk == pharmacyAccountId &&
                item.RequestTypeFromCategoryIdFk == shortageCategoryId &&
                item.StatusNum == OpenStatus)
            .Select(item => new { item.Irc, item.DrugShapFromDosageIdFk })
            .SingleOrDefaultAsync(cancellationToken);
        if (shortage is null)
        {
            return null;
        }

        if (request.ProvinceId > 0)
        {
            var isProvince = await context.Categories
                .AsNoTracking()
                .AnyAsync(category =>
                    category.CatId == request.ProvinceId &&
                    category.Type != null &&
                    category.Type.ToLower() == ProvinceCategoryType,
                    cancellationToken);
            if (!isProvince)
            {
                return null;
            }
        }

        if (request.CityId > 0)
        {
            var isTownship = await context.Categories
                .AsNoTracking()
                .AnyAsync(category =>
                    category.CatId == request.CityId &&
                    category.ParentId == request.ProvinceId &&
                    category.Type != null &&
                    category.Type.ToLower() == TownshipCategoryType,
                    cancellationToken);
            if (!isTownship)
            {
                return null;
            }
        }

        var ircs = shortage.Irc.HasValue
            ? new[] { shortage.Irc.Value }
            : shortage.DrugShapFromDosageIdFk.HasValue
                ? await context.DrugsNames
                    .AsNoTracking()
                    .Where(drug =>
                        drug.IrgenericId == shortage.DrugShapFromDosageIdFk.Value &&
                        drug.DrugNameLanguage == true &&
                        drug.Irc.HasValue)
                    .Select(drug => drug.Irc!.Value)
                    .Distinct()
                    .ToArrayAsync(cancellationToken)
                : Array.Empty<long>();

        if (request.DisplayType == "NearDate")
        {
            if (ircs.Length == 0)
            {
                return new PharmacyDrugShortageOffersViewModel { Offers = [] };
            }

            var excessCategoryId = await context.Categories
                .AsNoTracking()
                .Where(category => category.Type == ExcessCategoryType)
                .Select(category => (int?)category.CatId)
                .SingleOrDefaultAsync(cancellationToken);
            if (!excessCategoryId.HasValue)
            {
                throw new InvalidOperationException(
                    $"The '{ExcessCategoryType}' category is missing from the configured database.");
            }

            var today = DateTime.Today;
            var nearDateRows = await (
                from excess in context.PharmacyRequests.AsNoTracking()
                join drug in context.DrugsNames.AsNoTracking()
                    .Where(item => item.DrugNameLanguage == true)
                    on excess.Irc equals drug.Irc
                where excess.RequestTypeFromCategoryIdFk == excessCategoryId.Value &&
                      excess.PharmacyAccountIdFk.HasValue &&
                      excess.PharmacyAccountIdFk.Value != pharmacyAccountId &&
                      excess.ExchangeLimitNum == NearDateExchangeLimit &&
                      excess.DrugExpireDate.HasValue &&
                      excess.DrugExpireDate.Value >= today &&
                      excess.Irc.HasValue &&
                      ircs.Contains(excess.Irc.Value) &&
                      excess.StatusNum == OpenStatus &&
                      (request.CityId > 0
                          ? excess.PharmacyAccountIdFkNavigation!.PharmacyIdFkNavigation.CityIdFk == request.CityId
                          : request.ProvinceId > 0
                              ? excess.PharmacyAccountIdFkNavigation!.PharmacyIdFkNavigation.ProvinceIdFk == request.ProvinceId
                              : true)
                select new
                {
                    PharmacyAccountId = excess.PharmacyAccountIdFk!.Value,
                    excess.Id,
                    PharmacyName = excess.PharmacyAccountIdFkNavigation!.PharmacyIdFkNavigation.Name,
                    ProvinceName = excess.PharmacyAccountIdFkNavigation.PharmacyIdFkNavigation.ProvinceIdFkNavigation.Name,
                    TownshipName = excess.PharmacyAccountIdFkNavigation.PharmacyIdFkNavigation.CityIdFkNavigation.Name,
                    Address = excess.PharmacyAccountIdFkNavigation.PharmacyIdFkNavigation.Address,
                    Phone = excess.PharmacyAccountIdFkNavigation.PharmacyIdFkNavigation.Phone,
                    Mobile = excess.PharmacyAccountIdFkNavigation.PharmacyManagerMobile,
                    DrugName = drug.DrugName,
                    excess.DrugExpireDate,
                    excess.RegistrationDateTime
                })
                .ToListAsync(cancellationToken);

            var nearDateOffers = nearDateRows
                .GroupBy(row => row.PharmacyAccountId)
                .Select(group => group.First())
                .Select(row => new PharmacyDrugShortageOffer(
                    row.Id,
                    row.PharmacyName ?? string.Empty,
                    row.ProvinceName,
                    row.TownshipName,
                    row.Address,
                    row.Phone,
                    row.Mobile,
                    row.DrugName ?? string.Empty,
                    row.DrugExpireDate,
                    FormatRelativeTime(row.RegistrationDateTime),
                    []))
                .ToArray();

            return new PharmacyDrugShortageOffersViewModel { Offers = nearDateOffers };
        }

        var exchangeLimitNumbers = request.DisplayType == "NormalExchange" ? "2,3" : "1";
        var ircList = string.Join(",", ircs.Select(irc => irc.ToString(CultureInfo.InvariantCulture)));
        var rows = await context.Set<PharmacyDrugShortageExchangeRow>()
            .FromSqlInterpolated($"""
                EXEC dbo.SP_GetOtherPharmacyExcess_ForThisShortage
                    @MyPhAccountId = {pharmacyAccountId},
                    @splitLst_DrugExchangeLimitNums = {exchangeLimitNumbers},
                    @splitLst_IrcMyShortage = {ircList},
                    @CityId = {request.CityId},
                    @ProvinceId = {request.ProvinceId}
                """)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var offers = rows
            .GroupBy(row => row.Id)
            .Select(group => MapOffer(group.First(), group))
            .ToArray();

        return new PharmacyDrugShortageOffersViewModel { Offers = offers };
    }

    private async Task<int> GetShortageCategoryIdAsync(CancellationToken cancellationToken)
    {
        var categoryId = await context.Categories
            .AsNoTracking()
            .Where(category => category.Type == ShortageCategoryType)
            .Select(category => (int?)category.CatId)
            .SingleOrDefaultAsync(cancellationToken);
        return categoryId ?? throw new InvalidOperationException(
            $"The '{ShortageCategoryType}' category is missing from the configured database.");
    }

    private async Task<IReadOnlyList<PharmacyDrugShortageOption>> GetTownshipsForProvinceAsync(
        int provinceId,
        CancellationToken cancellationToken)
    {
        return await context.Categories
            .AsNoTracking()
            .Where(category =>
                category.ParentId == provinceId &&
                category.Type != null &&
                category.Type.ToLower() == TownshipCategoryType &&
                category.Name != null)
            .Select(category => new PharmacyDrugShortageOption(category.CatId, category.Name!))
            .ToListAsync(cancellationToken);
    }

    private async Task<PharmacyDrugShortageOption?> GetSelectedDrugAsync(
        int searchType,
        long id,
        CancellationToken cancellationToken)
    {
        if (searchType == 1)
        {
            return await context.DosageForms
                .AsNoTracking()
                .Where(dosage => dosage.DosageFormId == id)
                .Select(dosage => new PharmacyDrugShortageOption(
                    dosage.DosageFormId,
                    dosage.DosageFormName ?? dosage.DosageFormNameFa ?? string.Empty))
                .SingleOrDefaultAsync(cancellationToken);
        }

        return await context.DrugsNames
            .AsNoTracking()
            .Where(drug => drug.Irc == id && drug.DrugNameLanguage == true)
            .Select(drug => new PharmacyDrugShortageOption(
                drug.Irc!.Value,
                drug.DrugName ?? string.Empty))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static PharmacyDrugShortageOffer MapOffer(
        PharmacyDrugShortageExchangeRow row,
        IEnumerable<PharmacyDrugShortageExchangeRow> matchedRows)
    {
        var myExcessDrugNames = matchedRows
            .Select(item => item.ShortageThatMyExcessDrugName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .ToArray();

        return new PharmacyDrugShortageOffer(
            row.Id,
            row.ExcessPharmacyName ?? string.Empty,
            row.ExcessPharmacyProvinceName,
            row.ExcessPharmacyTownshipName,
            row.ExcessPharmacyAddress,
            row.ExcessPharmacyTell,
            row.ExcessPharmacyMobile,
            row.DrugName ?? string.Empty,
            row.DrugExpireDate,
            FormatRelativeTime(row.RegistrationDateTime),
            myExcessDrugNames);
    }

    private static string NormalizeSearch(string query) =>
        query.Trim().Replace('ي', 'ی').Replace('ك', 'ک');

    private static string FormatRelativeTime(DateTime? value)
    {
        if (!value.HasValue)
        {
            return string.Empty;
        }

        var elapsed = DateTime.Now - value.Value;
        if (elapsed.TotalSeconds < 60)
        {
            return "لحظاتی پیش";
        }

        if (elapsed.TotalMinutes < 60)
        {
            return $"{ToPersianNumber((int)elapsed.TotalMinutes)} دقیقه پیش";
        }

        if (elapsed.TotalHours < 24)
        {
            return $"{ToPersianNumber((int)elapsed.TotalHours)} ساعت پیش";
        }

        if (elapsed.TotalDays < 30)
        {
            return $"{ToPersianNumber((int)elapsed.TotalDays)} روز پیش";
        }

        if (elapsed.TotalDays < 365)
        {
            return $"{ToPersianNumber((int)(elapsed.TotalDays / 30))} ماه پیش";
        }

        return $"{ToPersianNumber((int)(elapsed.TotalDays / 365))} سال پیش";
    }

    private static string ToPersianNumber(int number) =>
        number.ToString(CultureInfo.InvariantCulture)
            .Replace('0', '۰')
            .Replace('1', '۱')
            .Replace('2', '۲')
            .Replace('3', '۳')
            .Replace('4', '۴')
            .Replace('5', '۵')
            .Replace('6', '۶')
            .Replace('7', '۷')
            .Replace('8', '۸')
            .Replace('9', '۹');
}
