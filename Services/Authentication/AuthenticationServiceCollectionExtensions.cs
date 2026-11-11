using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace darooyab_rewrite.Services.Authentication;

public static class AuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddAuthenticationCore(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services
            .AddOptions<AuthenticationCoreOptions>()
            .Bind(configuration.GetSection(AuthenticationCoreOptions.SectionName))
            .Validate(options => options.IsValid(), "Authentication options contain unsupported values.")
            .ValidateOnStart();

        var authenticationOptions = configuration
            .GetSection(AuthenticationCoreOptions.SectionName)
            .Get<AuthenticationCoreOptions>() ?? new AuthenticationCoreOptions();

        services.AddHttpContextAccessor();
        services.TryAddSingleton(TimeProvider.System);
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = authenticationOptions.CookieName;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = environment.IsDevelopment()
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.Always;
                options.LoginPath = authenticationOptions.LoginPath;
                options.LogoutPath = authenticationOptions.LogoutPath;
                options.ReturnUrlParameter = "returnUrl";
                options.ExpireTimeSpan = authenticationOptions.ExpireTimeSpan;
                options.SlidingExpiration = authenticationOptions.SlidingExpiration;
            });

        services.AddAuthorization();

        return services;
    }
}
