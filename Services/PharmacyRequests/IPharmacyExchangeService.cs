using darooyab_rewrite.Areas.Admin.ViewModels;

namespace darooyab_rewrite.Services.PharmacyRequests;

public interface IPharmacyExchangeService
{
    Task<PharmacyExchangePageViewModel> GetPageAsync(
        int pharmacyAccountId,
        PharmacyExchangeFormViewModel? form = null,
        int? editId = null,
        string? searchString = null,
        int page = 1,
        CancellationToken cancellationToken = default);

    Task<PharmacyExchangeSaveStatus> SaveAsync(
        int pharmacyAccountId,
        PharmacyExchangeFormViewModel form,
        CancellationToken cancellationToken = default);

    Task<PharmacyExchangeSaveStatus> SaveBasicAsync(
        int pharmacyAccountId,
        PharmacyExchangeCreateFormViewModel form,
        CancellationToken cancellationToken = default);

    Task<PharmacyExchangeExpiryUpdateStatus> UpdateExpiryAsync(
        int pharmacyAccountId,
        int requestId,
        int expiryMonth,
        int expiryYear,
        CancellationToken cancellationToken = default);

    Task<PharmacyExchangeOperationStatus> DeleteAsync(
        int pharmacyAccountId,
        int requestId,
        CancellationToken cancellationToken = default);

    Task<PharmacyExchangeOperationStatus> FinishAsync(
        int pharmacyAccountId,
        int requestId,
        CancellationToken cancellationToken = default);
}
