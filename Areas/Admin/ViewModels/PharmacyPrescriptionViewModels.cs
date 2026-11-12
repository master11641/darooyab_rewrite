namespace darooyab_rewrite.Areas.Admin.ViewModels;

public sealed class PharmacyPrescriptionPageViewModel
{
    public string? TrackingCode { get; init; }
    public bool? IsReviewed { get; init; }
    public int Page { get; init; }
    public int TotalPages { get; init; }
    public int TotalRecords { get; init; }
    public IReadOnlyList<PharmacyPrescriptionItemViewModel> Items { get; init; } =
        Array.Empty<PharmacyPrescriptionItemViewModel>();
}

public sealed record PharmacyPrescriptionItemViewModel(
    int Id,
    string TrackingCode,
    string DrugName,
    string RegistrationUserName,
    string NationalCode,
    string MobileNumber,
    string InsuranceTypeTitle,
    string PersianRegistrationDate,
    string SendOriginTitle,
    bool? IsReviewed,
    string? UploadImageUrl,
    bool IsSendSmsBefore);

public sealed class PharmacyPrescriptionSmsViewModel
{
    public int PrescriptionId { get; init; }
    public string PharmacyName { get; init; } = string.Empty;
    public string Recipient { get; init; } = string.Empty;
    public string DrugName { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public long BalanceInRial { get; init; }
    public bool SmsConfigured { get; init; }
    public bool HasValidRecipient { get; init; }
    public string? StatusMessage { get; init; }
}

public enum PharmacyPrescriptionSmsSendStatus
{
    Succeeded,
    Failed,
    NotFound,
    InsufficientBalance,
    NotConfigured,
    InvalidRecipient
}

public sealed record PharmacyPrescriptionSmsSendResult(
    PharmacyPrescriptionSmsSendStatus Status,
    string Message);
