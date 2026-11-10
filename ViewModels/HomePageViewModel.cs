#nullable enable

namespace darooyab_rewrite.ViewModels;

public sealed record HomePageViewModel(
    IReadOnlyList<LatestNewsItem> LatestNews,
    bool NewsUnavailable = false);
