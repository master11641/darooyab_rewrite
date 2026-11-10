namespace darooyab_rewrite.ViewModels;

public sealed record BrandGalleryImage(string Url, string AltText);

public sealed record BrandRelatedItem(long Irc, string EnglishName, string? PersianName, string Url);

public sealed record BrandPageAttributes(
    string? AdvertisingUrl,
    string? ShopUrl,
    string? TherapeuticApplication,
    string? PersianContent,
    string? EnglishContent,
    string? StorageTemperature,
    bool? IsOtc,
    bool? IsChemotherapy,
    bool? IsUnderSupervision,
    bool? IsHospital,
    decimal? Price,
    byte? PriceUnitNumber,
    string? PricePharmacyName,
    string? PriceCityName,
    IReadOnlyList<BrandGalleryImage> Images);

public sealed class BrandInfoPageViewModel
{
    public required long Irc { get; init; }
    public required int DrugId { get; init; }
    public required string EnglishName { get; init; }
    public string PersianName { get; init; } = string.Empty;
    public string? GenericName { get; init; }
    public string? GenericPersianName { get; init; }
    public string? GenericUrl { get; init; }
    public string? ManufacturerName { get; init; }
    public string? ImporterName { get; init; }
    public string? CountryName { get; init; }
    public string? MartindaleName { get; init; }
    public short? MartindaleGroupId { get; init; }
    public IReadOnlyList<string> AhfsPath { get; init; } = [];
    public string? GenericUsageInformation { get; init; }
    public string? ManufacturerLogoUrl { get; init; }
    public string? ManufacturerWebsiteUrl { get; init; }
    public BrandPageAttributes? Attributes { get; init; }
    public IReadOnlyList<BrandRelatedItem> RelatedGenericBrands { get; init; } = [];
    public IReadOnlyList<BrandRelatedItem> RelatedCompanyBrands { get; init; } = [];
    public IReadOnlyList<DrugComment> Comments { get; init; } = [];
}
