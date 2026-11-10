#nullable enable

namespace darooyab_rewrite.ViewModels;

public sealed record SiteSectionViewModel(
    string Title,
    string Description,
    string Eyebrow,
    string Icon,
    string? Query = null,
    LatestNewsItem? Article = null);
