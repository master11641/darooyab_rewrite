namespace darooyab_rewrite.Services.Authentication;

public sealed class AuthenticationCoreOptions
{
    public const string SectionName = "Authentication";

    public string CookieName { get; set; } = "Darooyab.Authentication";
    public TimeSpan ExpireTimeSpan { get; set; } = TimeSpan.FromMinutes(45);
    public bool SlidingExpiration { get; set; } = true;
    public bool EnablePasswordRehash { get; set; }
    public string LoginPath { get; set; } = "/ph/login";
    public string LogoutPath { get; set; } = "/ph/logout";
    public int MaxLoginAttemptsPerIp { get; set; } = 5;
    public TimeSpan LoginRateLimitWindow { get; set; } = TimeSpan.FromMinutes(10);

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(CookieName) &&
               ExpireTimeSpan >= TimeSpan.FromMinutes(5) &&
               ExpireTimeSpan <= TimeSpan.FromDays(7) &&
               IsLocalPath(LoginPath) &&
               IsLocalPath(LogoutPath) &&
               MaxLoginAttemptsPerIp is >= 1 and <= 100 &&
               LoginRateLimitWindow >= TimeSpan.FromMinutes(1) &&
               LoginRateLimitWindow <= TimeSpan.FromHours(1);
    }

    private static bool IsLocalPath(string? path)
    {
        return !string.IsNullOrWhiteSpace(path) &&
               path.StartsWith("/", StringComparison.Ordinal) &&
               !path.StartsWith("//", StringComparison.Ordinal) &&
               !path.Contains('\\');
    }
}
