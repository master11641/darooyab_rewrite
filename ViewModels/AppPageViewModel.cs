namespace darooyab_rewrite.ViewModels;

public sealed record AppPageViewModel(
    string Title,
    string? Subtitle,
    string ContentHtml,
    bool DataUnavailable = false);
