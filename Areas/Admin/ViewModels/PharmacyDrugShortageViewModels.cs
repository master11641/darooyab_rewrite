using System.ComponentModel.DataAnnotations;

namespace darooyab_rewrite.Areas.Admin.ViewModels;

public sealed class PharmacyDrugShortagePageViewModel
{
    public PharmacyDrugShortageFormViewModel Form { get; set; } = new();
    public PharmacyDrugShortageOption? SelectedDrug { get; set; }
    public IReadOnlyList<PharmacyDrugShortageListItem> Requests { get; set; } = [];
    public IReadOnlyList<PharmacyDrugShortageOption> Provinces { get; set; } = [];
    public IReadOnlyList<PharmacyDrugShortageOption> Townships { get; set; } = [];
    public int? SelectedProvinceId { get; set; }
    public int? SelectedTownshipId { get; set; }
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public bool EditRequestNotFound { get; set; }
    public bool IsEditMode => Form.Id.HasValue;
}

public sealed class PharmacyDrugShortageFormViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "شناسه درخواست معتبر نیست.")]
    public int? Id { get; set; }

    [Range(1, 2, ErrorMessage = "نوع جستجوی دارو معتبر نیست.")]
    public int SearchType { get; set; } = 2;

    [Required(ErrorMessage = "یک برند یا شکل دارویی انتخاب کنید.")]
    [Range(1, long.MaxValue, ErrorMessage = "داروی انتخاب‌شده معتبر نیست.")]
    public long? SearchProperty { get; set; }
}

public sealed record PharmacyDrugShortageOption(long Id, string Name);

public sealed record PharmacyDrugShortageListItem(
    int Id,
    string DrugName,
    DateTime? RegisteredAt,
    string RegisteredAtDisplay);

public sealed class PharmacyDrugShortageOffersViewModel
{
    public IReadOnlyList<PharmacyDrugShortageOffer> Offers { get; init; } = [];
}

public sealed record PharmacyDrugShortageOffer(
    int Id,
    string PharmacyName,
    string? ProvinceName,
    string? TownshipName,
    string? Address,
    string? Phone,
    string? Mobile,
    string DrugName,
    DateTime? DrugExpireDate,
    string RegisteredAtDisplay,
    IReadOnlyList<string> MyExcessDrugNames);

public sealed class PharmacyDrugShortageSearchRequest
{
    [Range(1, int.MaxValue)]
    public int PharmacyShortageId { get; set; }

    [RegularExpression("^(NormalExchange|StarExchange|NearDate)$")]
    public string DisplayType { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int ProvinceId { get; set; }

    [Range(0, int.MaxValue)]
    public int CityId { get; set; }
}

public enum PharmacyDrugShortageSaveStatus
{
    Succeeded,
    InvalidInput,
    NotFound,
    RestrictedBrand
}

public enum PharmacyDrugShortageDeleteStatus
{
    Succeeded,
    NotFound,
    HasExchanges
}
