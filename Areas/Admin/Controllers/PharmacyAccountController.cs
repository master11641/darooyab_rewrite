using System.Globalization;
using darooyab.Models;
using darooyab_rewrite.Services.Authentication;
using darooyab_rewrite.Services.Authentication.Pharmacy;
using darooyab_rewrite.Services.PharmacyDrugDistributions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace darooyab_rewrite.Areas.Admin.Controllers;

[Area("Admin")]
[Route("/PharmacyAccount")]
[Authorize(Policy = PharmacyAuthenticationDefaults.AuthorizationPolicy)]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class PharmacyAccountController(
    darooConContext context,
    IAuthenticationService authenticationService,
    IPharmacyDrugDistributionService drugDistributionService) : Controller
{
    [HttpGet("DrugDistribution")]
    public async Task<IActionResult> DrugDistribution(
        int page = 1,
        string? drugName = "",
        int factoryId = 0,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        ViewData["PharmacyName"] = await context.PharmacyAccounts
            .AsNoTracking()
            .Where(account => account.Id == pharmacyAccountId.Value)
            .Select(account => account.PharmacyIdFkNavigation.Name)
            .SingleAsync(cancellationToken);

        var model = await drugDistributionService.GetPageAsync(
            drugName,
            factoryId,
            page,
            cancellationToken);
        return View(model);
    }

    private async Task<int?> GetCurrentPharmacyAccountIdAsync(
        CancellationToken cancellationToken)
    {
        var user = authenticationService.GetCurrentUser();
        if (user is null ||
            user.UserType != PharmacyAuthenticationDefaults.UserType ||
            !int.TryParse(user.UserId, NumberStyles.None, CultureInfo.InvariantCulture, out var id))
        {
            return null;
        }

        var active = await context.PharmacyAccounts
            .AsNoTracking()
            .AnyAsync(account =>
                account.Id == id &&
                account.AccountStatus == "2" &&
                account.DarooyabAdminApproved != false &&
                (account.AccountExpireDate == null || account.AccountExpireDate >= DateTime.Now),
                cancellationToken);
        return active ? id : null;
    }
}
