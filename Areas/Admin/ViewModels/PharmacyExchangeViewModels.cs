using System.ComponentModel.DataAnnotations;

namespace darooyab_rewrite.Areas.Admin.ViewModels;

public sealed class PharmacyExchangePageViewModel
{
    public PharmacyExchangeFormViewModel Form { get; set; } = new();
    public IReadOnlyList<PharmacyRequestDrugOption> SelectedBrands { get; set; } = [];
    public IReadOnlyList<PharmacyExchangeListItem> Requests { get; set; } = [];
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public string? SearchString { get; set; }
    public bool EditRequestNotFound { get; set; }
    public bool IsEditMode => Form.Id.HasValue;
    public int CurrentYear { get; set; }
}

public sealed class PharmacyExchangeFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "حداقل یک برند را انتخاب کنید.")]
    public long[] SelectedIrcs { get; set; } = [];

    [Required(ErrorMessage = "نوع مازاد را انتخاب کنید.")]
    [RegularExpression("^[1-3]$", ErrorMessage = "نوع مازاد معتبر نیست.")]
    public string? ExchangeLimitNum { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "تعداد باید حداقل یک باشد.")]
    public int DrugCount { get; set; }

    [Required(ErrorMessage = "واحد دارو را انتخاب کنید.")]
    [RegularExpression("^[1-2]$", ErrorMessage = "واحد دارو معتبر نیست.")]
    public string? DrugUnitNum { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "قیمت معتبر نیست.")]
    public double? UnitPrice { get; set; }

    [Range(1, 12, ErrorMessage = "ماه انقضا معتبر نیست.")]
    public int DrugExpireMonth { get; set; }

    [Range(2000, 2200, ErrorMessage = "سال انقضا معتبر نیست.")]
    public int DrugExpireYear { get; set; }

    [StringLength(50, ErrorMessage = "شماره بچ نمی‌تواند بیش از ۵۰ نویسه باشد.")]
    public string? PatchNumber { get; set; }

    [StringLength(50, ErrorMessage = "شماره فاکتور نمی‌تواند بیش از ۵۰ نویسه باشد.")]
    public string? PurchaseInvoiceNumber { get; set; }

    public bool SellToColleague { get; set; }

    [Range(0, 999999999999999, ErrorMessage = "قیمت فروش به همکار معتبر نیست.")]
    public double? ColleaguePrice { get; set; }

    [Range(0, 90, ErrorMessage = "تخفیف باید بین صفر تا ۹۰ درصد باشد.")]
    public int? DiscountPercentage { get; set; }

    [Range(0, 3, ErrorMessage = "تسهیلات تبادل معتبر نیست.")]
    public byte? ExchangeFacility { get; set; }
}

public sealed class PharmacyExchangeCreateFormViewModel
{
    [Required(ErrorMessage = "حداقل یک برند را انتخاب کنید.")]
    public long[] SelectedIrcs { get; set; } = [];

    [Required(ErrorMessage = "نوع مازاد را انتخاب کنید.")]
    [RegularExpression("^[1-3]$", ErrorMessage = "نوع مازاد معتبر نیست.")]
    public string? ExchangeLimitNum { get; set; }

    [Range(1, 12, ErrorMessage = "ماه انقضا معتبر نیست.")]
    public int DrugExpireMonth { get; set; }

    [Range(2000, 2200, ErrorMessage = "سال انقضا معتبر نیست.")]
    public int DrugExpireYear { get; set; }
}

public sealed class PharmacyExchangeExpiryFormViewModel
{
    [Range(1, 12, ErrorMessage = "ماه انقضا معتبر نیست.")]
    public int DrugExpireMonth { get; set; }

    [Range(2000, 2200, ErrorMessage = "سال انقضا معتبر نیست.")]
    public int DrugExpireYear { get; set; }
}

public sealed record PharmacyExchangeListItem(
    int Id,
    string DrugName,
    int ExchangeRandomCode,
    string ExchangeLimitLabel,
    string DrugExpireDateDisplay,
    int? DrugCount,
    string? DrugUnitLabel,
    long ExchangedCount,
    string RegistrationDateDisplay,
    double? UnitPrice,
    string? PatchNumber,
    string? PurchaseInvoiceNumber,
    bool SellToColleague,
    double? ColleaguePrice,
    int? DiscountPercentage,
    string? ExchangeFacilityLabel)
{
    public long? RemainingCount => DrugCount is > 0
        ? Math.Max(0, DrugCount.Value - ExchangedCount)
        : null;
}

public enum PharmacyExchangeSaveStatus
{
    Succeeded,
    InvalidInput,
    NotFound,
    RestrictedBrand,
    Conflict,
    ExchangesExceedQuantity
}

public enum PharmacyExchangeOperationStatus
{
    Succeeded,
    NotFound,
    HasExchanges,
    Conflict
}

public enum PharmacyExchangeExpiryUpdateStatus
{
    Succeeded,
    InvalidInput,
    NotFound
}
