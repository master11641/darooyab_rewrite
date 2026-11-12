using darooyab_rewrite.Areas.Admin.ViewModels;

namespace darooyab_rewrite.Services.PharmacyDrugDistributions;

public interface IPharmacyDrugDistributionService
{
    Task<PharmacyDrugDistributionPageViewModel> GetPageAsync(
        string? drugName,
        int factoryId,
        int page = 1,
        CancellationToken cancellationToken = default);
}
