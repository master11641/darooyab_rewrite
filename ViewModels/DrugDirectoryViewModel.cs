namespace darooyab_rewrite.ViewModels;

public sealed record DrugDirectoryItem(
    int? DrugId,
    string PersianName,
    string EnglishName,
    long? Irc);

public sealed record DrugDirectoryViewModel(
    IReadOnlyList<DrugDirectoryItem> PopularDrugs,
    IReadOnlyList<DrugDirectoryItem> NewestBrands,
    bool DataUnavailable);
