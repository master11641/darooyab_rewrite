namespace darooyab_rewrite.ViewModels;

public sealed record DrugInfoSection(string Title, string? Content);

public sealed record DrugDosageForm(
    int? FdoGenerationCode,
    string? PersianName,
    string? EnglishName,
    bool? HighAlert,
    byte? TemperatureType,
    string? Emergency,
    bool? TopWarningLabel);

public sealed record DrugRelatedItem(int DrugId, string PersianName, string EnglishName);

public sealed record DrugCommentReply(string Name, string Content, DateTime CreatedAt);

public sealed record DrugComment(
    string Name,
    string Content,
    DateTime CreatedAt,
    IReadOnlyList<DrugCommentReply> Replies);

public sealed class DrugInfoPageViewModel
{
    public required int DrugId { get; init; }
    public required string EnglishName { get; init; }
    public string PersianName { get; init; } = string.Empty;
    public string? PageTitle { get; init; }
    public string? PageDescription { get; init; }
    public string? MartindaleNameFa { get; init; }
    public string? MartindaleNameEn { get; init; }
    public short? MartindaleGroupId { get; init; }
    public IReadOnlyList<string> AhfsPath { get; init; } = [];
    public string? GeneralInformation { get; init; }
    public IReadOnlyList<DrugInfoSection> InformationSections { get; init; } = [];
    public string? PregnancyCategoryCode { get; init; }
    public string? PregnancyCategoryDescription { get; init; }
    public IReadOnlyList<DrugDosageForm> DosageForms { get; init; } = [];
    public IReadOnlyList<string> PersianBrandNames { get; init; } = [];
    public IReadOnlyList<string> EnglishBrandNames { get; init; } = [];
    public IReadOnlyList<DrugRelatedItem> RelatedDrugs { get; init; } = [];
    public IReadOnlyList<DrugComment> Comments { get; init; } = [];
    public string? DrugsComUrl { get; init; }
    public string? MedscapeUrl { get; init; }
}

public sealed record SidebarNewsItem(int Id, string Title, string? SubTitle);

public sealed record SidebarDrugItem(int DrugId, string PersianName, string EnglishName, long? Irc = null);

public sealed record SidebarAdvertisement(
    string? ImageUrl,
    string? ImageAlt,
    string? Link,
    bool IsPublic);

public sealed class PublicRightPanelViewModel
{
    public IReadOnlyList<SidebarAdvertisement> PublicAdvertisements { get; init; } = [];
    public IReadOnlyList<SidebarAdvertisement> Advertisements { get; init; } = [];
    public IReadOnlyList<SidebarNewsItem> LatestArticles { get; init; } = [];
    public IReadOnlyList<SidebarNewsItem> LatestNews { get; init; } = [];
    public IReadOnlyList<SidebarDrugItem> NewDrugs { get; init; } = [];
    public IReadOnlyList<SidebarDrugItem> FrequentlyUsedDrugs { get; init; } = [];
    public bool DataUnavailable { get; set; }
}
