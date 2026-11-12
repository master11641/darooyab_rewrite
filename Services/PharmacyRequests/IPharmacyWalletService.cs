using darooyab_rewrite.Areas.Admin.ViewModels;

namespace darooyab_rewrite.Services.PharmacyRequests;

public interface IPharmacyWalletService
{
    Task<long> GetBalanceInRialAsync(
        int pharmacyAccountId,
        CancellationToken cancellationToken = default);

    Task<PharmacyWalletPageViewModel> GetPageAsync(
        int pharmacyAccountId,
        int page = 1,
        CancellationToken cancellationToken = default);

    Task<PharmacyPurchasedRankingsPageViewModel> GetPurchasedRankingsPageAsync(
        int pharmacyAccountId,
        int page = 1,
        CancellationToken cancellationToken = default);

    Task<PharmacyWalletPaymentStartResult> StartTopUpAsync(
        int pharmacyAccountId,
        long amountToman,
        CancellationToken cancellationToken = default);

    Task<PharmacyWalletPaymentVerificationStatus> VerifyTopUpAsync(
        int pharmacyAccountId,
        int transactionId,
        CancellationToken cancellationToken = default);

    Task<PharmacyWalletPaymentVerificationStatus> VerifyCallbackAsync(
        int pharmacyAccountId,
        string authority,
        CancellationToken cancellationToken = default);
}
