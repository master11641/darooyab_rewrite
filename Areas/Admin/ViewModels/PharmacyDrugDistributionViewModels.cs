namespace darooyab_rewrite.Areas.Admin.ViewModels;

public sealed class PharmacyDrugDistributionPageViewModel
{
    public IReadOnlyList<PharmacyDrugDistributionRow> Rows { get; init; } = [];
    public IReadOnlyList<PharmacyDistributionFactoryItem> Factories { get; init; } = [];
    public string DrugName { get; init; } = string.Empty;
    public int FactoryId { get; init; }
    public int Page { get; init; } = 1;
    public int TotalPages { get; init; } = 1;
}

public sealed record PharmacyDrugDistributionRow(
    int Id,
    string DrugName,
    string FactoryName,
    string AnnouncementDate,
    string SanitizedDescriptionHtml);

public sealed record PharmacyDistributionFactoryItem(short Id, string Name);
