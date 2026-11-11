using darooyab_rewrite.Areas.Admin.ViewModels;

namespace darooyab_rewrite.Services.PharmacyRequests;

public interface IPharmacyRequestService
{
    Task<PharmacyRequestPageViewModel> GetPageAsync(
        int pharmacyAccountId,
        PharmacyRequestFormViewModel? form = null,
        int? editId = null,
        string? searchString = null,
        int page = 1,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PharmacyRequestDrugOption>> SearchBrandsAsync(
        string query,
        CancellationToken cancellationToken = default);

    Task<PharmacyRequestBrandDetailsViewModel?> GetBrandDetailsAsync(
        int pharmacyAccountId,
        long irc,
        PharmacyRequestFormViewModel? form = null,
        CancellationToken cancellationToken = default);

    Task<PharmacyRequestSaveStatus> SaveAsync(
        int pharmacyAccountId,
        PharmacyRequestFormViewModel form,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int pharmacyAccountId,
        int requestId,
        CancellationToken cancellationToken = default);
}
