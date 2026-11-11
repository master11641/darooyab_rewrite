using System.Security.Claims;

namespace darooyab_rewrite.Services.Authentication;

public interface IAuthenticationService
{
    Task<AuthenticationResult> LoginAsync(
        AuthenticationCredentials credentials,
        CancellationToken cancellationToken = default);

    Task LogoutAsync();

    AuthenticatedUser? GetCurrentUser();

    ClaimsPrincipal? GetCurrentPrincipal();

    bool IsAuthenticated { get; }
}
