using darooyab_rewrite.Areas.Admin.ViewModels;

namespace darooyab_rewrite.Services.PharmacyDrugShortages;

public interface IPharmacyDrugShortageService
{
    Task<PharmacyDrugShortagePageViewModel> GetPageAsync(
        int pharmacyAccountId,
        PharmacyDrugShortageFormViewModel? form = null,
        int? editId = null,
        int page = 1,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PharmacyDrugShortageOption>> SearchAsync(
        int searchType,
        string query,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PharmacyDrugShortageOption>> GetTownshipsAsync(
        int provinceId,
        CancellationToken cancellationToken = default);

    Task<PharmacyNearDatePageViewModel> GetNearDatePageAsync(
        int pharmacyAccountId,
        int provinceId,
        int townshipId,
        int page,
        CancellationToken cancellationToken = default);

    Task<PharmacyDrugShortageSaveStatus> SaveAsync(
        int pharmacyAccountId,
        PharmacyDrugShortageFormViewModel form,
        CancellationToken cancellationToken = default);

    Task<PharmacyDrugShortageDeleteStatus> DeleteAsync(
        int pharmacyAccountId,
        int requestId,
        CancellationToken cancellationToken = default);

    Task<PharmacyDrugShortageOffersViewModel?> GetOffersAsync(
        int pharmacyAccountId,
        PharmacyDrugShortageSearchRequest request,
        CancellationToken cancellationToken = default);
}
