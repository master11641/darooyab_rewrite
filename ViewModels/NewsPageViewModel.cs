#nullable enable

namespace darooyab_rewrite.ViewModels;

public sealed record NewsPageViewModel(
    PagedResult<LatestNewsItem> PagedNews,
    bool DataUnavailable);
