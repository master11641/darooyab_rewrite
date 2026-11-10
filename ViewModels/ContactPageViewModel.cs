using System.ComponentModel.DataAnnotations;

namespace darooyab_rewrite.ViewModels;

public sealed record ContactActivityOption(int Id, string Name);

public sealed record ContactMessageInput
{
    [StringLength(255)]
    public string? Name { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "نوع فعالیت را انتخاب کنید.")]
    public int ActivityId { get; init; } = 53;

    [StringLength(255)]
    public string? Organization { get; init; }

    [StringLength(255)]
    public string? Role { get; init; }

    [StringLength(255)]
    public string? Phone { get; init; }

    [EmailAddress(ErrorMessage = "رایانامه را با قالب معتبر وارد کنید.")]
    [StringLength(50)]
    public string? Email { get; init; }

    [Required(ErrorMessage = "متن پیام را وارد کنید.")]
    public string Message { get; init; } = string.Empty;

    public string? Website { get; init; }
}

public sealed record ContactPageViewModel(
    IReadOnlyList<ContactActivityOption> Activities,
    ContactMessageInput Input,
    bool DataUnavailable = false,
    bool MessageSent = false,
    bool SubmissionUnavailable = false);
