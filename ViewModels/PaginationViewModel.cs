namespace darooyab_rewrite.ViewModels;

public sealed record PaginationViewModel(
    int PageNumber,
    int TotalPages,
    string RouteName,
    string PageParameterName,
    Dictionary<string, string> RouteValues);
