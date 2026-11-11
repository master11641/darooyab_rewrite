namespace darooyab_rewrite.Services.Authentication;

public interface IUserAuthenticationProvider
{
    string Name { get; }

    bool CanHandle(string userName);

    Task<AuthenticatedUser?> AuthenticateAsync(
        AuthenticationCredentials credentials,
        CancellationToken cancellationToken);
}
