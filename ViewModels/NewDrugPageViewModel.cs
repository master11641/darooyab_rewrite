namespace darooyab_rewrite.ViewModels;

public sealed record NewDrugListItem(
    int Id,
    long Irc,
    int DrugId,
    string PersianName,
    string EnglishName,
    string GenericEnglishName,
    string? GenericPersianName,
    string? TherapeuticApplication,
    string? ImageUrl,
    string? FactoryName);

public sealed record NewDrugPageViewModel(
    PagedResult<NewDrugListItem> PagedDrugs,
    bool DataUnavailable)
{
    public const int PageSize = 10;
    public IReadOnlyList<NewDrugListItem> Items => PagedDrugs.Items;
}
