using darooyab.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace darooyab_rewrite.Services.Authentication.Pharmacy;

public static class PharmacyAuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddPharmacyAuthenticationProvider(
        this IServiceCollection services)
    {
        services.TryAddScoped<IPasswordHasher<PharmacyAccount>, PasswordHasher<PharmacyAccount>>();
        services.TryAddEnumerable(
            ServiceDescriptor.Scoped<IUserAuthenticationProvider, PharmacyAuthenticationProvider>());
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PharmacyAuthenticationDefaults.AuthorizationPolicy, policy =>
                policy.RequireAuthenticatedUser()
                    .RequireClaim(
                        AuthenticationClaimTypes.UserType,
                        PharmacyAuthenticationDefaults.UserType));
        });

        return services;
    }
}
