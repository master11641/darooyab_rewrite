using System.ComponentModel.DataAnnotations;

namespace darooyab_rewrite.ViewModels;

public sealed record PublicCommentSubmission
{
    [Required, StringLength(50, MinimumLength = 3)]
    public required string Name { get; init; }

    [EmailAddress, StringLength(100)]
    public string? Email { get; init; }

    [StringLength(50)]
    public string? Phone { get; init; }

    [Required, StringLength(4000, MinimumLength = 3)]
    public required string Content { get; init; }

    [Required, RegularExpression("^(generic|brand|doctor)$")]
    public required string TargetType { get; init; }

    [Range(1, long.MaxValue)]
    public long TargetId { get; init; }

    [StringLength(43, MinimumLength = 43)]
    public string? CaptchaChallengeId { get; init; }

    [StringLength(8, MinimumLength = 4)]
    public string? CaptchaAnswer { get; init; }

    public string? Website { get; init; }

    [Required, StringLength(2048)]
    public required string ReturnUrl { get; init; }
}

public sealed record CompanyMessageSubmission
{
    [StringLength(255)]
    public string? Name { get; init; }

    [StringLength(50)]
    public string? Phone { get; init; }

    [Required, StringLength(4000, MinimumLength = 3)]
    public required string Content { get; init; }

    [Range(1, short.MaxValue)]
    public short CompanyId { get; init; }

    [StringLength(43, MinimumLength = 43)]
    public string? CaptchaChallengeId { get; init; }

    [StringLength(8, MinimumLength = 4)]
    public string? CaptchaAnswer { get; init; }

    public string? Website { get; init; }

    [Required, StringLength(2048)]
    public required string ReturnUrl { get; init; }
}

public sealed record PublicCommentFormViewModel(
    string TargetType,
    long TargetId,
    string Subject,
    string ReturnUrl,
    bool CaptchaEnabled);
