using System.ComponentModel.DataAnnotations;

namespace darooyab_rewrite.ViewModels;

public sealed class PharmacyLoginViewModel
{
    [Required(ErrorMessage = "نام کاربری را وارد کنید.")]
    [StringLength(32)]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "کلمه‌ی عبور را وارد کنید.")]
    [StringLength(256)]
    public string Password { get; set; } = string.Empty;

    [StringLength(2048)]
    public string? ReturnUrl { get; set; }

    [StringLength(43, MinimumLength = 43)]
    public string? CaptchaChallengeId { get; set; }

    [StringLength(8, MinimumLength = 4)]
    public string? CaptchaAnswer { get; set; }
}
