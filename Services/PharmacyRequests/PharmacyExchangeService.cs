using System.Data;
using System.Globalization;
using System.Security.Cryptography;
using darooyab.Models;
using darooyab_rewrite.Areas.Admin.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace darooyab_rewrite.Services.PharmacyRequests;

public sealed class PharmacyExchangeService(
    darooConContext context,
    ILogger<PharmacyExchangeService> logger) : IPharmacyExchangeService
{
    private const string ExcessDrugCategoryType = "excess_drugs";
    private const string OpenStatus = "1";
    private const string ClosedByPharmacyStatus = "2";
    private const int PageSize = 30;

    public async Task<PharmacyExchangePageViewModel> GetPageAsync(
        int pharmacyAccountId,
        PharmacyExchangeFormViewModel? form = null,
        int? editId = null,
        string? searchString = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var excessCategoryId = await GetExcessDrugCategoryIdAsync(cancellationToken);
        var currentForm = form ?? new PharmacyExchangeFormViewModel();
        var editRequestNotFound = false;
        if (editId.HasValue)
        {
            var request = await context.PharmacyRequests
                .AsNoTracking()
                .Where(item =>
                    item.Id == editId.Value &&
                    item.PharmacyAccountIdFk == pharmacyAccountId &&
                    item.RequestTypeFromCategoryIdFk == excessCategoryId &&
                    item.StatusNum == OpenStatus)
                .Select(item => new
                {
                    item.Id,
                    item.Irc,
                    item.ExchangeLimitNum,
                    item.DrugCount,
                    item.DrugUnitNum,
                    item.UnitPrice,
                    item.DrugExpireDate,
                    item.PatchNumber,
                    item.PurchaseInvoiceNumber,
                    item.SellToColleague,
                    item.ColleaguePrice,
                    item.DiscountPercentage,
                    item.ExchangeFacility
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (request is null)
            {
                editRequestNotFound = true;
            }
            else
            {
                var expiration = request.DrugExpireDate ?? DateTime.Today;
                currentForm = new PharmacyExchangeFormViewModel
                {
                    Id = request.Id,
                    SelectedIrcs = request.Irc.HasValue ? [request.Irc.Value] : [],
                    ExchangeLimitNum = request.ExchangeLimitNum,
                    DrugCount = request.DrugCount ?? 0,
                    DrugUnitNum = request.DrugUnitNum,
                    UnitPrice = request.UnitPrice,
                    DrugExpireMonth = expiration.Month,
                    DrugExpireYear = expiration.Year,
                    PatchNumber = request.PatchNumber,
                    PurchaseInvoiceNumber = request.PurchaseInvoiceNumber,
                    SellToColleague = request.SellToColleague == true,
                    ColleaguePrice = request.ColleaguePrice,
                    DiscountPercentage = request.DiscountPercentage,
                    ExchangeFacility = request.ExchangeFacility.HasValue
                        ? (byte)request.ExchangeFacility.Value
                        : null
                };
            }
        }

        currentForm.SelectedIrcs = (currentForm.SelectedIrcs ?? [])
            .Where(irc => irc > 0)
            .Distinct()
            .Take(25)
            .ToArray();

        var selectedBrands = currentForm.SelectedIrcs.Length == 0
            ? []
            : await context.DrugsNames
                .AsNoTracking()
                .Where(drug =>
                    drug.Irc.HasValue &&
                    currentForm.SelectedIrcs.Contains(drug.Irc.Value) &&
                    drug.DrugNameLanguage == true)
                .OrderBy(drug => drug.Id)
                .Select(drug => new PharmacyRequestDrugOption(
                    drug.Irc!.Value,
                    drug.DrugName ?? string.Empty))
                .ToListAsync(cancellationToken);
        selectedBrands = selectedBrands
            .GroupBy(brand => brand.Irc)
            .Select(group => group.First())
            .ToList();

        var normalizedSearch = searchString?.Trim();
        var requestsQuery = context.PharmacyRequests
            .AsNoTracking()
            .Where(request =>
                request.PharmacyAccountIdFk == pharmacyAccountId &&
                request.RequestTypeFromCategoryIdFk == excessCategoryId &&
                request.StatusNum == OpenStatus);
        if (!string.IsNullOrEmpty(normalizedSearch))
        {
            requestsQuery = requestsQuery.Where(request =>
                context.DrugsNames.Any(drug =>
                    drug.Irc == request.Irc &&
                    drug.DrugNameLanguage == true &&
                    drug.DrugName != null &&
                    drug.DrugName.Contains(normalizedSearch)));
        }

        var totalCount = await requestsQuery.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
        var currentPage = Math.Clamp(page, 1, totalPages);
        var requestRows = await requestsQuery
            .OrderByDescending(request => request.RegistrationDateTime)
            .ThenByDescending(request => request.Id)
            .Skip((currentPage - 1) * PageSize)
            .Take(PageSize)
            .Select(request => new
            {
                request.Id,
                request.Irc,
                request.ExchangeRandomCode,
                request.ExchangeLimitNum,
                request.DrugExpireDate,
                request.DrugCount,
                request.DrugUnitNum,
                request.RegistrationDateTime,
                request.UnitPrice,
                request.PatchNumber,
                request.PurchaseInvoiceNumber,
                request.SellToColleague,
                request.ColleaguePrice,
                request.DiscountPercentage,
                request.ExchangeFacility,
                DrugName = context.DrugsNames
                    .Where(drug =>
                        drug.Irc == request.Irc &&
                        drug.DrugNameLanguage == true)
                    .OrderBy(drug => drug.Id)
                    .Select(drug => drug.DrugName)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);
        var requestIds = requestRows.Select(row => row.Id).ToArray();
        var exchangedCounts = requestIds.Length == 0
            ? new Dictionary<int, long>()
            : await context.PharmacyExchanges
                .AsNoTracking()
                .Where(exchange => requestIds.Contains(exchange.ExcessPhReqIdFk))
                .GroupBy(exchange => exchange.ExcessPhReqIdFk)
                .Select(group => new
                {
                    RequestId = group.Key,
                    Count = group.Sum(exchange => (long)exchange.RequireCount)
                })
                .ToDictionaryAsync(item => item.RequestId, item => item.Count, cancellationToken);

        return new PharmacyExchangePageViewModel
        {
            Form = currentForm,
            SelectedBrands = selectedBrands,
            Requests = requestRows.Select(row => new PharmacyExchangeListItem(
                row.Id,
                row.DrugName ?? "نام برند در دسترس نیست",
                row.ExchangeRandomCode ?? 0,
                GetExchangeLimitLabel(row.ExchangeLimitNum),
                row.DrugExpireDate?.ToString("yyyy/MM", CultureInfo.InvariantCulture) ?? "ثبت نشده",
                row.DrugCount,
                row.DrugUnitNum == "1" ? "عدد" : row.DrugUnitNum == "2" ? "بسته" : null,
                exchangedCounts.GetValueOrDefault(row.Id),
                FormatPersianDate(row.RegistrationDateTime),
                row.UnitPrice,
                row.PatchNumber,
                row.PurchaseInvoiceNumber,
                row.SellToColleague == true,
                row.ColleaguePrice,
                row.DiscountPercentage,
                GetExchangeFacilityLabel(row.ExchangeFacility)))
                .ToArray(),
            Page = currentPage,
            TotalPages = totalPages,
            SearchString = normalizedSearch,
            EditRequestNotFound = editRequestNotFound,
            CurrentYear = DateTime.Today.Year
        };
    }

    public async Task<PharmacyExchangeSaveStatus> SaveAsync(
        int pharmacyAccountId,
        PharmacyExchangeFormViewModel form,
        CancellationToken cancellationToken = default)
    {
        var ircs = (form.SelectedIrcs ?? [])
            .Where(irc => irc > 0)
            .Distinct()
            .ToArray();
        var now = DateTime.Now;
        if (ircs.Length == 0 ||
            ircs.Length > 25 ||
            (form.Id.HasValue && ircs.Length != 1) ||
            !IsValidForm(form, now.Year))
        {
            return PharmacyExchangeSaveStatus.InvalidInput;
        }

        var excessCategoryId = await GetExcessDrugCategoryIdAsync(cancellationToken);
        var validBrands = await context.DrugsNames
            .AsNoTracking()
            .Where(drug =>
                drug.Irc.HasValue &&
                ircs.Contains(drug.Irc.Value) &&
                drug.DrugNameLanguage == true)
            .Select(drug => drug.Irc!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);
        if (validBrands.Count != ircs.Length)
        {
            return PharmacyExchangeSaveStatus.InvalidInput;
        }

        var hasProhibitedBrand = await context.DarooyabBrandRestrictions.AnyAsync(
            restriction =>
                restriction.IsActive &&
                restriction.BrandRestrictionType == BrandRestrictionTypeEnum.FullyProhibited &&
                ircs.Contains(restriction.IRC),
            cancellationToken);
        if (hasProhibitedBrand)
        {
            return PharmacyExchangeSaveStatus.RestrictedBrand;
        }

        if (!await context.PharmacyAccounts
                .AsNoTracking()
                .AnyAsync(account => account.Id == pharmacyAccountId, cancellationToken))
        {
            return PharmacyExchangeSaveStatus.NotFound;
        }

        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var targetRequests = new List<PharmacyRequest>();
        if (form.Id.HasValue)
        {
            var editedRequest = await context.PharmacyRequests.SingleOrDefaultAsync(
                request =>
                    request.Id == form.Id.Value &&
                    request.PharmacyAccountIdFk == pharmacyAccountId &&
                    request.RequestTypeFromCategoryIdFk == excessCategoryId &&
                    request.StatusNum == OpenStatus &&
                    request.Irc == ircs[0],
                cancellationToken);
            if (editedRequest is null)
            {
                return PharmacyExchangeSaveStatus.NotFound;
            }
            if (await context.PharmacyRequests.AnyAsync(
                    request =>
                        request.Id != editedRequest.Id &&
                        request.PharmacyAccountIdFk == pharmacyAccountId &&
                        request.RequestTypeFromCategoryIdFk == excessCategoryId &&
                        request.StatusNum == OpenStatus &&
                        request.Irc == ircs[0],
                    cancellationToken))
            {
                return PharmacyExchangeSaveStatus.Conflict;
            }
            targetRequests.Add(editedRequest);
        }
        else
        {
            var existingRequests = await context.PharmacyRequests
                .Where(request =>
                    request.PharmacyAccountIdFk == pharmacyAccountId &&
                    request.RequestTypeFromCategoryIdFk == excessCategoryId &&
                    request.StatusNum == OpenStatus &&
                    request.Irc.HasValue &&
                    ircs.Contains(request.Irc.Value))
                .ToListAsync(cancellationToken);
            if (existingRequests.GroupBy(request => request.Irc).Any(group => group.Count() > 1))
            {
                return PharmacyExchangeSaveStatus.Conflict;
            }
            targetRequests.AddRange(existingRequests);
        }

        var existingRequestIds = targetRequests
            .Where(request => request.Id != 0)
            .Select(request => request.Id)
            .ToArray();
        var exchangedCounts = existingRequestIds.Length == 0
            ? new Dictionary<int, long>()
            : await context.PharmacyExchanges
                .AsNoTracking()
                .Where(exchange => existingRequestIds.Contains(exchange.ExcessPhReqIdFk))
                .GroupBy(exchange => exchange.ExcessPhReqIdFk)
                .Select(group => new
                {
                    RequestId = group.Key,
                    Count = group.Sum(exchange => (long)exchange.RequireCount)
                })
                .ToDictionaryAsync(item => item.RequestId, item => item.Count, cancellationToken);

        var existingIrcs = targetRequests
            .Where(request => request.Irc.HasValue)
            .Select(request => request.Irc!.Value)
            .ToHashSet();
        foreach (var irc in ircs.Where(irc => !existingIrcs.Contains(irc)))
        {
            targetRequests.Add(new PharmacyRequest
            {
                PharmacyAccountIdFk = pharmacyAccountId,
                RequestTypeFromCategoryIdFk = excessCategoryId,
                Irc = irc,
                ExchangeRandomCode = RandomNumberGenerator.GetInt32(1000, 9999)
            });
        }

        var expiryDate = new DateTime(
            form.DrugExpireYear,
            form.DrugExpireMonth,
            DateTime.DaysInMonth(form.DrugExpireYear, form.DrugExpireMonth));
        foreach (var request in targetRequests)
        {
            var exchangedCount = exchangedCounts.GetValueOrDefault(request.Id);
            if (request.Id != 0 && form.DrugCount < exchangedCount)
            {
                return PharmacyExchangeSaveStatus.ExchangesExceedQuantity;
            }

            request.RequestTypeFromCategoryIdFk = excessCategoryId;
            request.PharmacyAccountIdFk = pharmacyAccountId;
            request.ExchangeLimitNum = form.ExchangeLimitNum;
            request.DrugCount = form.DrugCount;
            request.DrugUnitNum = form.DrugUnitNum;
            request.DrugShapFromDosageIdFk = null;
            request.DrugExpireDate = expiryDate;
            request.UnitPrice = form.UnitPrice;
            request.PatchNumber = NormalizeOptional(form.PatchNumber);
            request.PurchaseInvoiceNumber = NormalizeOptional(form.PurchaseInvoiceNumber);
            request.CompanyDistributorIdFk = null;
            request.SellToColleague = form.SellToColleague;
            request.ColleaguePrice = form.SellToColleague ? form.ColleaguePrice : null;
            request.DiscountPercentage = form.DiscountPercentage;
            request.ExchangeFacility = form.ExchangeFacility.HasValue
                ? (ExchangeFacility)form.ExchangeFacility.Value
                : null;
            request.RequestExpireDate = null;
            request.CountLimit = null;
            request.PationRefferalView = null;
            request.Description = null;
            request.RequestView = null;
            request.RegistrationDateTime = now;
            request.StatusNum = OpenStatus;

            if (request.Id == 0)
            {
                context.PharmacyRequests.Add(request);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return PharmacyExchangeSaveStatus.Succeeded;
    }

    public async Task<PharmacyExchangeSaveStatus> SaveBasicAsync(
        int pharmacyAccountId,
        PharmacyExchangeCreateFormViewModel form,
        CancellationToken cancellationToken = default)
    {
        var ircs = (form.SelectedIrcs ?? [])
            .Where(irc => irc > 0)
            .Distinct()
            .ToArray();
        var now = DateTime.Now;
        if (ircs.Length == 0 ||
            ircs.Length > 25 ||
            form.ExchangeLimitNum is not ("1" or "2" or "3") ||
            form.DrugExpireMonth is < 1 or > 12 ||
            form.DrugExpireYear < now.Year ||
            form.DrugExpireYear > now.Year + 11)
        {
            return PharmacyExchangeSaveStatus.InvalidInput;
        }

        var excessCategoryId = await GetExcessDrugCategoryIdAsync(cancellationToken);
        var validBrands = await context.DrugsNames
            .AsNoTracking()
            .Where(drug =>
                drug.Irc.HasValue &&
                ircs.Contains(drug.Irc.Value) &&
                drug.DrugNameLanguage == true)
            .Select(drug => drug.Irc!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);
        if (validBrands.Count != ircs.Length)
        {
            return PharmacyExchangeSaveStatus.InvalidInput;
        }

        var hasProhibitedBrand = await context.DarooyabBrandRestrictions.AnyAsync(
            restriction =>
                restriction.IsActive &&
                restriction.BrandRestrictionType == BrandRestrictionTypeEnum.FullyProhibited &&
                ircs.Contains(restriction.IRC),
            cancellationToken);
        if (hasProhibitedBrand)
        {
            return PharmacyExchangeSaveStatus.RestrictedBrand;
        }

        if (!await context.PharmacyAccounts
                .AsNoTracking()
                .AnyAsync(account => account.Id == pharmacyAccountId, cancellationToken))
        {
            return PharmacyExchangeSaveStatus.NotFound;
        }

        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);
        var existingRequests = await context.PharmacyRequests
            .Where(request =>
                request.PharmacyAccountIdFk == pharmacyAccountId &&
                request.RequestTypeFromCategoryIdFk == excessCategoryId &&
                request.StatusNum == OpenStatus &&
                request.Irc.HasValue &&
                ircs.Contains(request.Irc.Value))
            .ToListAsync(cancellationToken);
        if (existingRequests.GroupBy(request => request.Irc).Any(group => group.Count() > 1))
        {
            return PharmacyExchangeSaveStatus.Conflict;
        }

        var existingByIrc = existingRequests.ToDictionary(request => request.Irc!.Value);
        var expiryDate = new DateTime(
            form.DrugExpireYear,
            form.DrugExpireMonth,
            DateTime.DaysInMonth(form.DrugExpireYear, form.DrugExpireMonth));
        foreach (var irc in ircs)
        {
            if (existingByIrc.TryGetValue(irc, out var existingRequest))
            {
                existingRequest.ExchangeLimitNum = form.ExchangeLimitNum;
                existingRequest.DrugExpireDate = expiryDate;
                existingRequest.RegistrationDateTime = now;
                continue;
            }

            context.PharmacyRequests.Add(new PharmacyRequest
            {
                PharmacyAccountIdFk = pharmacyAccountId,
                RequestTypeFromCategoryIdFk = excessCategoryId,
                Irc = irc,
                ExchangeRandomCode = RandomNumberGenerator.GetInt32(1000, 9999),
                ExchangeLimitNum = form.ExchangeLimitNum,
                DrugCount = 0,
                DrugUnitNum = null,
                DrugExpireDate = expiryDate,
                RegistrationDateTime = now,
                StatusNum = OpenStatus
            });
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return PharmacyExchangeSaveStatus.Succeeded;
    }

    public async Task<PharmacyExchangeExpiryUpdateStatus> UpdateExpiryAsync(
        int pharmacyAccountId,
        int requestId,
        int expiryMonth,
        int expiryYear,
        CancellationToken cancellationToken = default)
    {
        var currentYear = DateTime.Today.Year;
        if (expiryMonth is < 1 or > 12 ||
            expiryYear < currentYear ||
            expiryYear > currentYear + 11)
        {
            return PharmacyExchangeExpiryUpdateStatus.InvalidInput;
        }

        var excessCategoryId = await GetExcessDrugCategoryIdAsync(cancellationToken);
        var request = await context.PharmacyRequests.SingleOrDefaultAsync(
            item =>
                item.Id == requestId &&
                item.PharmacyAccountIdFk == pharmacyAccountId &&
                item.RequestTypeFromCategoryIdFk == excessCategoryId &&
                item.StatusNum == OpenStatus,
            cancellationToken);
        if (request is null)
        {
            return PharmacyExchangeExpiryUpdateStatus.NotFound;
        }

        request.DrugExpireDate = new DateTime(
            expiryYear,
            expiryMonth,
            DateTime.DaysInMonth(expiryYear, expiryMonth));
        await context.SaveChangesAsync(cancellationToken);
        return PharmacyExchangeExpiryUpdateStatus.Succeeded;
    }

    public async Task<PharmacyExchangeOperationStatus> DeleteAsync(
        int pharmacyAccountId,
        int requestId,
        CancellationToken cancellationToken = default)
    {
        var excessCategoryId = await GetExcessDrugCategoryIdAsync(cancellationToken);
        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);
        var request = await context.PharmacyRequests.SingleOrDefaultAsync(
            item =>
                item.Id == requestId &&
                item.PharmacyAccountIdFk == pharmacyAccountId &&
                item.RequestTypeFromCategoryIdFk == excessCategoryId &&
                item.StatusNum == OpenStatus,
            cancellationToken);
        if (request is null)
        {
            return PharmacyExchangeOperationStatus.NotFound;
        }

        if (await context.PharmacyExchanges.AnyAsync(
                exchange => exchange.ExcessPhReqIdFk == requestId,
                cancellationToken))
        {
            return PharmacyExchangeOperationStatus.HasExchanges;
        }

        context.PharmacyRequests.Remove(request);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            logger.LogWarning(
                exception,
                "Could not delete pharmacy exchange request {RequestId}; a dependent record may have been added concurrently.",
                requestId);
            return PharmacyExchangeOperationStatus.Conflict;
        }
        await transaction.CommitAsync(cancellationToken);
        return PharmacyExchangeOperationStatus.Succeeded;
    }

    public async Task<PharmacyExchangeOperationStatus> FinishAsync(
        int pharmacyAccountId,
        int requestId,
        CancellationToken cancellationToken = default)
    {
        var excessCategoryId = await GetExcessDrugCategoryIdAsync(cancellationToken);
        var request = await context.PharmacyRequests.SingleOrDefaultAsync(
            item =>
                item.Id == requestId &&
                item.PharmacyAccountIdFk == pharmacyAccountId &&
                item.RequestTypeFromCategoryIdFk == excessCategoryId &&
                item.StatusNum == OpenStatus,
            cancellationToken);
        if (request is null)
        {
            return PharmacyExchangeOperationStatus.NotFound;
        }

        request.StatusNum = ClosedByPharmacyStatus;
        await context.SaveChangesAsync(cancellationToken);
        return PharmacyExchangeOperationStatus.Succeeded;
    }

    private async Task<int> GetExcessDrugCategoryIdAsync(CancellationToken cancellationToken)
    {
        return await context.Categories
                   .AsNoTracking()
                   .Where(category => category.Type == ExcessDrugCategoryType)
                   .Select(category => (int?)category.CatId)
                   .SingleOrDefaultAsync(cancellationToken)
               ?? throw new InvalidOperationException(
                   "The excess-drug category is missing from the configured database.");
    }

    private static bool IsValidForm(PharmacyExchangeFormViewModel form, int currentYear)
    {
        return form.DrugCount > 0 &&
               form.DrugExpireMonth is >= 1 and <= 12 &&
               form.DrugExpireYear >= currentYear &&
               form.DrugExpireYear <= currentYear + 11 &&
               form.ExchangeLimitNum is "1" or "2" or "3" &&
               form.DrugUnitNum is "1" or "2" &&
               (!form.UnitPrice.HasValue || double.IsFinite(form.UnitPrice.Value) && form.UnitPrice.Value >= 0) &&
               (!form.ColleaguePrice.HasValue ||
                double.IsFinite(form.ColleaguePrice.Value) && form.ColleaguePrice.Value >= 0) &&
               (!form.DiscountPercentage.HasValue ||
                form.DiscountPercentage is >= 0 and <= 90 &&
                form.DiscountPercentage.Value % 5 == 0) &&
               (!form.ExchangeFacility.HasValue || form.ExchangeFacility is <= 3) &&
               form.PatchNumber?.Length <= 50 &&
               form.PurchaseInvoiceNumber?.Length <= 50;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string GetExchangeLimitLabel(string? value) => value switch
    {
        "1" => "ویژه، کمیاب یا لوکس",
        "2" => "معمولی",
        "3" => "تاریخ نزدیک",
        _ => "ثبت نشده"
    };

    private static string? GetExchangeFacilityLabel(ExchangeFacility? value) => value switch
    {
        ExchangeFacility.Cash => "فروش نقدی",
        ExchangeFacility.InteractionMedication => "تبادل با داروی مصرفی",
        ExchangeFacility.SpecialConditions => "پذیرش شرایط خاص",
        (ExchangeFacility)3 => "شرایط عادی",
        _ => null
    };

    private static string FormatPersianDate(DateTime? value)
    {
        if (!value.HasValue)
        {
            return "نامشخص";
        }

        var date = value.Value;
        var calendar = new PersianCalendar();
        return string.Create(
            CultureInfo.InvariantCulture,
            $"{calendar.GetYear(date):0000}/{calendar.GetMonth(date):00}/{calendar.GetDayOfMonth(date):00} {date.Hour:00}:{date.Minute:00}");
    }
}
