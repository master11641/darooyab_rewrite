using System.Globalization;
using darooyab.Models;
using darooyab_rewrite.Areas.Admin.ViewModels;
using darooyab_rewrite.Services.Authentication;
using darooyab_rewrite.Services.Authentication.Pharmacy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace darooyab_rewrite.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = PharmacyAuthenticationDefaults.AuthorizationPolicy)]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class DashboardController(
    darooConContext context,
    IAuthenticationService authenticationService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var user = authenticationService.GetCurrentUser();
        if (user is null ||
            !int.TryParse(user.UserId, NumberStyles.None, CultureInfo.InvariantCulture, out var pharmacyAccountId))
        {
            return Forbid();
        }

        var now = DateTime.Now;
        var pharmacyName = await context.PharmacyAccounts
            .AsNoTracking()
            .Where(account =>
                account.Id == pharmacyAccountId &&
                account.AccountStatus == "2" &&
                account.DarooyabAdminApproved != false &&
                (account.AccountExpireDate == null || account.AccountExpireDate >= now))
            .Select(account => account.PharmacyIdFkNavigation.Name)
            .SingleOrDefaultAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(pharmacyName))
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        ViewData["PharmacyName"] = pharmacyName;
        return View(new AdminDashboardViewModel(pharmacyName, user.UserName));
    }
}
