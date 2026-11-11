namespace darooyab_rewrite.Services.Captcha;

public interface ICaptchaService
{
    bool IsEnabled { get; }

    Task<CaptchaChallenge> GenerateAsync(
        string clientBinding,
        string? remoteIpAddress,
        CancellationToken cancellationToken);

    Task<CaptchaValidationResult> ValidateAsync(
        string? challengeId,
        string? answer,
        string clientBinding,
        string? remoteIpAddress,
        CancellationToken cancellationToken);
}
