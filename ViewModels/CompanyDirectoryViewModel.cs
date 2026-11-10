namespace darooyab_rewrite.ViewModels;

public sealed record PharmaceuticalCompanyDirectoryItem(
    short Id,
    string Name,
    string Country,
    int ProductCount,
    string? LogoUrl);

public sealed record CompanyDirectoryViewModel(
    IReadOnlyList<PharmaceuticalCompanyDirectoryItem> Companies,
    int ProducerType,
    bool DataUnavailable = false,
    bool IsImporterDirectory = false,
    bool IsDistributorDirectory = false);
