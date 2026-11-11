namespace darooyab_rewrite.Services.Captcha;

public interface ICaptchaChallengeStore
{
    Task<bool> TryCreateAsync(
        StoredCaptchaChallenge challenge,
        string clientKey,
        string ipKey,
        CaptchaStorePolicy policy,
        CancellationToken cancellationToken);

    Task<StoredCaptchaChallenge?> GetAsync(
        string challengeId,
        CancellationToken cancellationToken);

    Task<bool> TryAcquireValidationAsync(
        string clientKey,
        string ipKey,
        CaptchaStorePolicy policy,
        CancellationToken cancellationToken);

    Task<CaptchaCommitStatus> TryCommitValidationAsync(
        string challengeId,
        string clientBindingHash,
        int expectedFailedAttempts,
        bool answerMatches,
        CaptchaStorePolicy policy,
        CancellationToken cancellationToken);
}
