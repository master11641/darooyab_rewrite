namespace darooyab_rewrite.ViewModels;

public sealed record DrugSearchResult(string Name, string GenericName, string Url);

public sealed record DrugSearchPageViewModel(
    string SearchText,
    PagedResult<DrugSearchResult> PagedResults,
    bool SearchUnavailable)
{
    public const int PageSize = 10;
    public const int SuggestionLimit = 10;

    public IReadOnlyList<DrugSearchResult> Results => PagedResults.Items;
    public int PageNumber => PagedResults.PageNumber;
    public int PageCount => PagedResults.TotalPages;
    public int TotalCount => PagedResults.TotalCount;
}
