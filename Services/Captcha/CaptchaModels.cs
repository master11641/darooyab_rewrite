namespace darooyab_rewrite.Services.Captcha;

public sealed record CaptchaChallenge(
    string ChallengeId,
    byte[] ImageBytes,
    DateTimeOffset ExpiresAt);

public enum CaptchaValidationStatus
{
    Valid,
    Invalid,
    Expired,
    AlreadyConsumed,
    AttemptsExceeded,
    RateLimited
}

public sealed record CaptchaValidationResult(CaptchaValidationStatus Status)
{
    public bool IsValid => Status == CaptchaValidationStatus.Valid;
}

public sealed record StoredCaptchaChallenge(
    string ChallengeId,
    string ClientBindingHash,
    string AnswerHash,
    long CreatedAtUnixMilliseconds,
    long ExpiresAtUnixMilliseconds,
    int FailedAttempts,
    bool Consumed);

public sealed record CaptchaStorePolicy(
    int MaxActiveChallengesPerClient,
    int MaxGenerateRequestsPerClient,
    int MaxGenerateRequestsPerIp,
    int MaxValidationRequestsPerClient,
    int MaxValidationRequestsPerIp,
    long RateWindowNumber,
    long RateWindowTtlMilliseconds,
    long NowUnixMilliseconds,
    long ChallengeTtlMilliseconds,
    int MaxFailedAttempts);

public enum CaptchaCommitStatus
{
    Accepted,
    Rejected,
    AttemptsExceeded,
    Invalid,
    Expired,
    AlreadyConsumed,
    Retry
}

public sealed class CaptchaStoreUnavailableException : Exception
{
    public CaptchaStoreUnavailableException(Exception innerException)
        : base("The CAPTCHA challenge store is unavailable.", innerException)
    {
    }
}
