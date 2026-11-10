namespace darooyab_rewrite.ViewModels;

public sealed record DrugGroupDirectoryItem(
    byte Id,
    string EnglishName,
    string PersianName);

public sealed record DrugGroupDirectoryViewModel(
    IReadOnlyList<DrugGroupDirectoryItem> Groups,
    bool DataUnavailable = false);

public sealed record DrugGroupDrugItem(
    int DrugId,
    string EnglishName,
    string? PersianName,
    string? ImageUrl,
    long? Irc = null,
    string? ManufacturerName = null);

public sealed record DrugGroupDetailsViewModel(
    DrugGroupDirectoryItem Group,
    IReadOnlyList<DrugGroupDrugItem> Drugs,
    bool DataUnavailable = false,
    bool IsHerbalMedicines = false,
    bool IsHerbalPlants = false);
