namespace darooyab_rewrite.Services.PharmacyBrandSuggestions;

public interface IPharmacyBrandSuggestionService
{
    Task<PharmacyBrandSuggestionSubmitStatus> SubmitAsync(
        int pharmacyAccountId,
        string? brandName,
        string? remoteIpAddress,
        CancellationToken cancellationToken = default);
}

public enum PharmacyBrandSuggestionSubmitStatus
{
    Succeeded,
    InvalidInput,
    PharmacyNotFound
}
