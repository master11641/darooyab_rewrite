namespace darooyab_rewrite.Areas.Admin.ViewModels;

public sealed record AdminDashboardViewModel(
    string PharmacyName,
    string UserName,
    int ValidReferralCount,
    int ExpiredReferralCount,
    int PurchasedRankingCount,
    int ActiveRankingCount,
    long PurchasedRankingAmountToman,
    int TotalPrescriptionCount,
    int ReviewedPrescriptionCount,
    int UnreviewedPrescriptionCount,
    long WalletBalanceToman);
