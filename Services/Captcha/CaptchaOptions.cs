namespace darooyab_rewrite.Services.Captcha;

public sealed class CaptchaOptions
{
    public bool Enabled { get; set; } = true;
    public int TextLength { get; set; } = 5;
    public int ImageWidth { get; set; } = 180;
    public int ImageHeight { get; set; } = 64;
    public TimeSpan ChallengeLifetime { get; set; } = TimeSpan.FromMinutes(5);
    public int MaxFailedAttempts { get; set; } = 5;
    public int MaxActiveChallengesPerClient { get; set; } = 20;
    public TimeSpan RateLimitWindow { get; set; } = TimeSpan.FromMinutes(10);
    public int MaxGenerateRequestsPerClient { get; set; } = 20;
    public int MaxGenerateRequestsPerIp { get; set; } = 120;
    public int MaxValidationRequestsPerClient { get; set; } = 20;
    public int MaxValidationRequestsPerIp { get; set; } = 120;
    public int MaxApiRequestsPerIp { get; set; } = 240;
    public TimeSpan ApiRateLimitWindow { get; set; } = TimeSpan.FromMinutes(10);
    public int MaxPublicFeedbackRequestsPerIp { get; set; } = 10;
    public TimeSpan PublicFeedbackRateLimitWindow { get; set; } = TimeSpan.FromMinutes(10);
    public int NoisePixelCount { get; set; } = 180;
    public int InterferenceLineCount { get; set; } = 4;
    public string StorageMode { get; set; } = "Memory";
}
