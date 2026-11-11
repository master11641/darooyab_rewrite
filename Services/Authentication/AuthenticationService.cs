using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace darooyab_rewrite.Services.Authentication;

public sealed class AuthenticationService(
    IEnumerable<IUserAuthenticationProvider> providers,
    IHttpContextAccessor httpContextAccessor,
    IOptions<AuthenticationCoreOptions> options) : IAuthenticationService
{
    private readonly IUserAuthenticationProvider[] _providers = providers.ToArray();

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    public async Task<AuthenticationResult> LoginAsync(
        AuthenticationCredentials credentials,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(credentials.UserName) ||
            string.IsNullOrEmpty(credentials.Password))
        {
            return AuthenticationResult.Failure;
        }

        var provider = _providers.FirstOrDefault(item => item.CanHandle(credentials.UserName));
        if (provider is null)
        {
            return AuthenticationResult.Failure;
        }

        var user = await provider.AuthenticateAsync(credentials, cancellationToken);
        if (user is null ||
            string.IsNullOrWhiteSpace(user.UserId) ||
            string.IsNullOrWhiteSpace(user.UserType) ||
            string.IsNullOrWhiteSpace(user.UserName))
        {
            return AuthenticationResult.Failure;
        }

        user = user with { AuthenticationProvider = provider.Name };
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, $"{user.UserType}:{user.UserId}"),
            new(AuthenticationClaimTypes.UserId, user.UserId),
            new(AuthenticationClaimTypes.UserType, user.UserType),
            new(ClaimTypes.Name, user.UserName),
            new(AuthenticationClaimTypes.AuthenticationProvider, user.AuthenticationProvider)
        };

        if (user.Roles is not null)
        {
            claims.AddRange(user.Roles
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .Select(role => new Claim(ClaimTypes.Role, role)));
        }

        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme,
            ClaimTypes.Name,
            ClaimTypes.Role));

        var httpContext = GetHttpContext();
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false,
                AllowRefresh = options.Value.SlidingExpiration
            });

        return AuthenticationResult.Success;
    }

    public Task LogoutAsync()
    {
        return GetHttpContext().SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public AuthenticatedUser? GetCurrentUser()
    {
        var principal = GetCurrentPrincipal();
        if (principal?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var userId = principal.FindFirstValue(AuthenticationClaimTypes.UserId);
        var userType = principal.FindFirstValue(AuthenticationClaimTypes.UserType);
        var userName = principal.FindFirstValue(ClaimTypes.Name);
        var authenticationProvider = principal.FindFirstValue(
            AuthenticationClaimTypes.AuthenticationProvider);
        if (string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(userType) ||
            string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(authenticationProvider))
        {
            return null;
        }

        return new AuthenticatedUser(
            userId,
            userType,
            userName,
            authenticationProvider,
            principal.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray());
    }

    public ClaimsPrincipal? GetCurrentPrincipal()
    {
        return httpContextAccessor.HttpContext?.User;
    }

    private HttpContext GetHttpContext()
    {
        return httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException(
                "Authentication operations require an active HTTP request.");
    }
}
