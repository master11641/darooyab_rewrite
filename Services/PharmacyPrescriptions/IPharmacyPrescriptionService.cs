using darooyab_rewrite.Areas.Admin.ViewModels;

namespace darooyab_rewrite.Services.PharmacyPrescriptions;

public interface IPharmacyPrescriptionService
{
    Task<PharmacyPrescriptionPageViewModel> GetPageAsync(
        int pharmacyAccountId,
        string? trackingCode,
        bool? isReviewed,
        int page,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateReviewedAsync(
        int pharmacyAccountId,
        int prescriptionId,
        bool isReviewed,
        CancellationToken cancellationToken = default);

    Task<PharmacyPrescriptionSmsViewModel?> GetSmsPageAsync(
        int pharmacyAccountId,
        int prescriptionId,
        string? statusMessage,
        CancellationToken cancellationToken = default);

    Task<PharmacyPrescriptionSmsSendResult> SendSmsAsync(
        int pharmacyAccountId,
        int prescriptionId,
        CancellationToken cancellationToken = default);
}
