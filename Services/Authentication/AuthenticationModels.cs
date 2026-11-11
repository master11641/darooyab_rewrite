namespace darooyab_rewrite.Services.Authentication;

public sealed record AuthenticationCredentials(string UserName, string Password);

public sealed record AuthenticatedUser(
    string UserId,
    string UserType,
    string UserName,
    string AuthenticationProvider = "",
    IReadOnlyCollection<string>? Roles = null);

public sealed record AuthenticationResult(bool Succeeded)
{
    public static AuthenticationResult Success { get; } = new(true);
    public static AuthenticationResult Failure { get; } = new(false);
}

public static class AuthenticationClaimTypes
{
    public const string UserId = "darooyab:user_id";
    public const string UserType = "darooyab:user_type";
    public const string AuthenticationProvider = "darooyab:authentication_provider";
}
