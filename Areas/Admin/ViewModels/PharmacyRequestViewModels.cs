using System.ComponentModel.DataAnnotations;

namespace darooyab_rewrite.Areas.Admin.ViewModels;

public sealed class PharmacyRequestPageViewModel
{
    public PharmacyRequestFormViewModel Form { get; set; } = new();
    public IReadOnlyList<PharmacyRequestCategoryOption> CountLimitOptions { get; set; } = [];
    public IReadOnlyList<PharmacyRequestCategoryOption> VisibilityOptions { get; set; } = [];
    public IReadOnlyList<PharmacyRequestDrugOption> SelectedBrands { get; set; } = [];
    public PharmacyRequestBrandDetailsViewModel? BrandDetails { get; set; }
    public IReadOnlyList<int> AvailablePeriods { get; set; } = [];
    public IReadOnlyList<PharmacyRequestListItem> Requests { get; set; } = [];
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public string? SearchString { get; set; }
    public bool EditRequestNotFound { get; set; }
    public bool IsEditMode => Form.Id.HasValue;
}

public sealed class PharmacyRequestFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "حداقل یک برند را انتخاب کنید.")]
    [MinLength(1, ErrorMessage = "حداقل یک برند را انتخاب کنید.")]
    public long[] SelectedIrcs { get; set; } = [];

    [Range(1, 60, ErrorMessage = "مدت نمایش باید بین ۱ تا ۶۰ روز باشد.")]
    public int DurationDays { get; set; } = 20;

    [Required(ErrorMessage = "نوع محدودیت تعداد را انتخاب کنید.")]
    public int? CountLimitCategoryId { get; set; }

    [MinLength(1, ErrorMessage = "حداقل یک گروه مشاهده‌کننده را انتخاب کنید.")]
    public int[] VisibilityCategoryIds { get; set; } = [];

    [StringLength(1000, ErrorMessage = "توضیحات نمی‌تواند بیش از ۱۰۰۰ نویسه باشد.")]
    public string? Description { get; set; }

    public bool BuyRanking { get; set; }

    [Range(1, 10, ErrorMessage = "رتبه‌ی انتخاب‌شده معتبر نیست.")]
    public short? PurchaseRankNumber { get; set; }

    [Range(1, 60, ErrorMessage = "مدت خرید رتبه باید بین ۱ تا ۶۰ روز باشد.")]
    public int? PurchaseRankDays { get; set; }

    public bool UpdateBrandPrice { get; set; }

    [Range(typeof(decimal), "0", "999999999999999999", ErrorMessage = "قیمت باید عددی صحیح و معتبر باشد.")]
    public decimal? BrandPrice { get; set; }

    [Range(1, 2, ErrorMessage = "واحد قیمت معتبر نیست.")]
    public byte? BrandPriceUnitNumber { get; set; }
}

public sealed record PharmacyRequestCategoryOption(int Id, string Name);

public sealed record PharmacyRequestDrugOption(long Irc, string Name);

public sealed class PharmacyRequestBrandDetailsViewModel
{
    public long Irc { get; init; }
    public string BrandName { get; init; } = string.Empty;
    public string ProvinceName { get; init; } = string.Empty;
    public IReadOnlyList<PharmacyRequestActiveRankViewModel> ActiveRankings { get; init; } = [];
    public short? AvailableRankNumber { get; init; }
    public int? DailyPriceToman { get; init; }
    public short? OwnedRankNumber { get; init; }
    public string? OwnedRankExpiresAtDisplay { get; init; }
    public long WalletBalanceToman { get; init; }
    public decimal? CurrentPrice { get; init; }
    public byte? CurrentPriceUnitNumber { get; init; }
    public string? LastPriceEditorName { get; init; }
    public string? LastPriceEditorCity { get; init; }
    public string? LastPriceEditDateDisplay { get; init; }
    public bool IsEditMode { get; init; }
    public bool BuyRanking { get; init; }
    public int? PurchaseRankDays { get; init; }
    public bool UpdateBrandPrice { get; init; }
    public decimal? SubmittedBrandPrice { get; init; }
    public byte? SubmittedBrandPriceUnitNumber { get; init; }
}

public sealed record PharmacyRequestActiveRankViewModel(
    short RankNumber,
    string PharmacyName,
    string ExpiresAtDisplay,
    int DailyPriceToman);

public sealed record PharmacyRequestListItem(
    int Id,
    string DrugName,
    DateTime? RegisteredAt,
    DateTime? ExpiresAt,
    string RegisteredAtDisplay,
    string ExpiresAtDisplay,
    string CountLimit,
    int RemainingDays);

public enum PharmacyRequestSaveStatus
{
    Succeeded,
    InvalidInput,
    NotFound,
    RestrictedBrand,
    Conflict,
    InsufficientBalance,
    RankUnavailable,
    RankAlreadyOwned,
    InvalidBrandPrice
}
