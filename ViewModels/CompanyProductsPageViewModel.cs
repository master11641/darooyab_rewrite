namespace darooyab_rewrite.ViewModels;

public sealed record CompanyProductListItem(
    int Id,
    int DrugId,
    long Irc,
    string BrandName,
    string? PersianName,
    string? ManufacturerName,
    string? GenericName,
    string? ImageUrl);

public sealed record CompanyOrganizationDetails(
    string? LogoUrl,
    string? WebsiteUrl,
    string? Email,
    string? Phone,
    string? Fax,
    string? Address);

public sealed record CompanyProductsPageViewModel(
    short? CompanyId,
    string? CompanyName,
    string? CountryName,
    string DirectoryPath,
    CompanyOrganizationDetails? Organization,
    IReadOnlyList<CompanyProductListItem> Products,
    bool DataUnavailable = false);
