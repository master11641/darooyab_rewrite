namespace darooyab_rewrite.Services.PharmacyPrescriptions;

public interface IPharmacyPrescriptionSmsSender
{
    bool IsConfigured { get; }

    Task<PrescriptionSmsProviderResult> SendAsync(
        string recipient,
        string message,
        CancellationToken cancellationToken = default);

    Task<PrescriptionSmsProviderResult> SendVerifyLookupAsync(
        string recipient,
        string token,
        CancellationToken cancellationToken = default);
}

public sealed record PrescriptionSmsProviderResult(
    int StatusCode,
    string StatusMessage,
    bool IsSuccess);
