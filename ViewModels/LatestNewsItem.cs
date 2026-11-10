#nullable enable

namespace darooyab_rewrite.ViewModels;

public sealed record LatestNewsItem(
    int Id,
    string Title,
    string? SubTitle,
    string? ImageUrl,
    string? Description,
    string? PersianShowDate);
