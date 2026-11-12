using System.ComponentModel.DataAnnotations;

namespace darooyab_rewrite.Areas.Admin.ViewModels;

public sealed class PharmacyWalletPageViewModel
{
    public long PaidAmountToman { get; init; }
    public long SpentAmountToman { get; init; }
    public long BalanceToman { get; init; }
    public IReadOnlyList<PharmacyWalletTransactionItem> Transactions { get; init; } = [];
    public int Page { get; init; } = 1;
    public int TotalPages { get; init; } = 1;
    public bool CanCharge { get; init; }
}

public sealed class PharmacyPurchasedRankingsPageViewModel
{
    public IReadOnlyList<PharmacyPurchasedRankingItem> Rankings { get; init; } = [];
    public int Page { get; init; } = 1;
    public int TotalPages { get; init; } = 1;
}

public sealed record PharmacyPurchasedRankingItem(
    string DrugName,
    string StartDateDisplay,
    int DurationInDays,
    string ExpiryDateDisplay,
    short RankNumber,
    long AmountToman,
    string? ModifyDescription);

public sealed record PharmacyWalletTransactionItem(
    int Id,
    DateTime? RegisteredAt,
    string RegisteredAtDisplay,
    string TransactionType,
    long AmountToman,
    string? Authority,
    string? ReferenceId,
    DateTime? PaidAt,
    string PaidAtDisplay,
    string Status,
    bool CanVerify);

public sealed class PharmacyWalletTopUpForm
{
    [Required(ErrorMessage = "مبلغ شارژ را وارد کنید.")]
    [Range(1_100, 20_000_000, ErrorMessage = "مبلغ شارژ باید بین ۱,۱۰۰ تا ۲۰,۰۰۰,۰۰۰ تومان باشد.")]
    public long? AmountToman { get; set; }
}

public enum PharmacyWalletPaymentStartStatus
{
    Started,
    InvalidAmount,
    NotConfigured,
    ProviderUnavailable
}

public sealed record PharmacyWalletPaymentStartResult(
    PharmacyWalletPaymentStartStatus Status,
    string? PaymentUrl = null);

public enum PharmacyWalletPaymentVerificationStatus
{
    Succeeded,
    AlreadyPaid,
    NotFound,
    NotPending,
    NotConfigured,
    Rejected,
    ProviderUnavailable
}
