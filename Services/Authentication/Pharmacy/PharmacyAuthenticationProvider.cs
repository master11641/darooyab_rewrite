using System.Globalization;
using darooyab.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace darooyab_rewrite.Services.Authentication.Pharmacy;

public sealed class PharmacyAuthenticationProvider(
    darooConContext context,
    IPasswordHasher<PharmacyAccount> passwordHasher,
    TimeProvider timeProvider,
    IOptions<AuthenticationCoreOptions> authenticationOptions) : IUserAuthenticationProvider
{
    private const string ActiveAccountStatus = "2";

    public string Name => PharmacyAuthenticationDefaults.ProviderName;

    public bool CanHandle(string userName)
    {
        return LegacyPharmacyCredentialCompatibility.NormalizeMobile(userName) is not null;
    }

    public async Task<AuthenticatedUser?> AuthenticateAsync(
        AuthenticationCredentials credentials,
        CancellationToken cancellationToken)
    {
        var mobile = LegacyPharmacyCredentialCompatibility.NormalizeMobile(credentials.UserName);
        if (mobile is null || string.IsNullOrEmpty(credentials.Password))
        {
            return null;
        }

        var account = await context.PharmacyAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                item => item.FounderMobileNum == mobile,
                cancellationToken);
        if (account is null || string.IsNullOrEmpty(account.PasswordHash))
        {
            return null;
        }

        var normalizedPassword =
            LegacyPharmacyCredentialCompatibility.NormalizePassword(credentials.Password);
        var verification = account.PasswordHash.StartsWith("AQAAAA", StringComparison.Ordinal)
            ? passwordHasher.VerifyHashedPassword(account, account.PasswordHash, normalizedPassword)
            : PasswordVerificationResult.Failed;
        var isLegacyPassword = verification == PasswordVerificationResult.Failed &&
                               LegacyPharmacyCredentialCompatibility.VerifyPassword(
                                   account.PasswordHash,
                                   normalizedPassword);
        if ((verification == PasswordVerificationResult.Failed && !isLegacyPassword) ||
            !IsAccountActive(account))
        {
            return null;
        }

        if (authenticationOptions.Value.EnablePasswordRehash &&
            (isLegacyPassword || verification == PasswordVerificationResult.SuccessRehashNeeded))
        {
            var upgradedHash = passwordHasher.HashPassword(account, normalizedPassword);
            var updatedRows = await context.PharmacyAccounts
                .Where(item => item.Id == account.Id && item.PasswordHash == account.PasswordHash)
                .ExecuteUpdateAsync(
                    updates => updates.SetProperty(item => item.PasswordHash, upgradedHash),
                    cancellationToken);
            if (updatedRows != 1)
            {
                return null;
            }
        }

        return new AuthenticatedUser(
            account.Id.ToString(CultureInfo.InvariantCulture),
            PharmacyAuthenticationDefaults.UserType,
            mobile);
    }

    private bool IsAccountActive(PharmacyAccount account)
    {
        var localNow = timeProvider.GetLocalNow().DateTime;
        return account.AccountStatus == ActiveAccountStatus &&
               account.DarooyabAdminApproved != false &&
               (account.AccountExpireDate is null || account.AccountExpireDate >= localNow);
    }
}
