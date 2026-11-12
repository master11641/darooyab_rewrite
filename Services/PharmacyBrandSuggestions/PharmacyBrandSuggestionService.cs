using darooyab.Models;
using Microsoft.EntityFrameworkCore;

namespace darooyab_rewrite.Services.PharmacyBrandSuggestions;

public sealed class PharmacyBrandSuggestionService(darooConContext context)
    : IPharmacyBrandSuggestionService
{
    private const int OtherContactCategoryId = 53;
    private const short UnseenByManagerStatus = 2;
    private const int MaximumStoredIpLength = 25;

    public async Task<PharmacyBrandSuggestionSubmitStatus> SubmitAsync(
        int pharmacyAccountId,
        string? brandName,
        string? remoteIpAddress,
        CancellationToken cancellationToken = default)
    {
        var normalizedBrandName = brandName?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedBrandName))
        {
            return PharmacyBrandSuggestionSubmitStatus.InvalidInput;
        }

        var pharmacyName = await context.PharmacyAccounts
            .AsNoTracking()
            .Where(account => account.Id == pharmacyAccountId)
            .Select(account => account.PharmacyIdFkNavigation.Name)
            .SingleOrDefaultAsync(cancellationToken);
        if (pharmacyName is null)
        {
            return PharmacyBrandSuggestionSubmitStatus.PharmacyNotFound;
        }

        if (remoteIpAddress?.Length > MaximumStoredIpLength)
        {
            remoteIpAddress = null;
        }

        context.TblContactus.Add(new TblContactu
        {
            TblContactusNameAndFamily = "تکمیل برند",
            TblContactusCategoryIdFk = OtherContactCategoryId,
            TblContactusOrganizationOrCompanyName = $"داروخانه {pharmacyName}",
            TblContactusRole = string.Empty,
            TblContactusTel = string.Empty,
            TblContactusEmail = string.Empty,
            TblContactusTextMessage = normalizedBrandName,
            TblContactusViewByManagerStatusNum = UnseenByManagerStatus,
            TblContactusRegisterDate = DateTime.Now.Date,
            TblContactusViewByManagerDate = null,
            TblContactusIpsystem = remoteIpAddress ?? string.Empty
        });

        await context.SaveChangesAsync(cancellationToken);
        return PharmacyBrandSuggestionSubmitStatus.Succeeded;
    }
}
