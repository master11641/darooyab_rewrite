using System.Globalization;
using darooyab.Models;
using darooyab_rewrite.Areas.Admin.ViewModels;
using darooyab_rewrite.Services.Authentication;
using darooyab_rewrite.Services.Authentication.Pharmacy;
using darooyab_rewrite.Services.PharmacyBrandSuggestions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace darooyab_rewrite.Areas.Admin.Controllers;

[Area("Admin")]
[Route("/Brand")]
[Authorize(Policy = PharmacyAuthenticationDefaults.AuthorizationPolicy)]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class BrandController(
    darooConContext context,
    IAuthenticationService authenticationService,
    IPharmacyBrandSuggestionService brandSuggestionService) : Controller
{
    [HttpGet("SubmitBrandNameByPharmacy")]
    public async Task<IActionResult> SubmitBrandNameByPharmacy(
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        await SetPharmacyNameAsync(pharmacyAccountId.Value, cancellationToken);
        return View(new PharmacyBrandSuggestionViewModel());
    }

    [HttpPost("SubmitBrandNameInTblContactus")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitBrandNameInTblContactus(
        PharmacyBrandSuggestionViewModel input,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "نام فارسی برند را وارد کنید." });
        }

        var brandName = input.PersianBrandName?.Trim();
        var result = await brandSuggestionService.SubmitAsync(
            pharmacyAccountId.Value,
            brandName,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            cancellationToken);
        return result switch
        {
            PharmacyBrandSuggestionSubmitStatus.Succeeded =>
                Ok(new
                {
                    success = true,
                    message = $"برند {brandName} جهت بررسی و ثبت در سامانه به واحد مدیریت ارجاع شد."
                }),
            PharmacyBrandSuggestionSubmitStatus.InvalidInput =>
                BadRequest(new { message = "نام فارسی برند را وارد کنید." }),
            PharmacyBrandSuggestionSubmitStatus.PharmacyNotFound =>
                NotFound(new { message = "اطلاعات داروخانه در دسترس نیست." }),
            _ => StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "ارسال پیشنهاد انجام نشد؛ لطفاً دوباره تلاش کنید." })
        };
    }

    private async Task<int?> GetCurrentPharmacyAccountIdAsync(CancellationToken cancellationToken)
    {
        var pharmacyAccountId = GetCurrentUserPharmacyAccountId();
        if (!pharmacyAccountId.HasValue)
        {
            return null;
        }

        var now = DateTime.Now;
        var isActive = await context.PharmacyAccounts
            .AsNoTracking()
            .AnyAsync(account =>
                account.Id == pharmacyAccountId.Value &&
                account.AccountStatus == "2" &&
                account.DarooyabAdminApproved != false &&
                (account.AccountExpireDate == null || account.AccountExpireDate >= now),
                cancellationToken);
        return isActive ? pharmacyAccountId : null;
    }

    private int? GetCurrentUserPharmacyAccountId()
    {
        var user = authenticationService.GetCurrentUser();
        return user is not null &&
               user.UserType == PharmacyAuthenticationDefaults.UserType &&
               int.TryParse(user.UserId, NumberStyles.None, CultureInfo.InvariantCulture, out var id)
            ? id
            : null;
    }

    private async Task SetPharmacyNameAsync(
        int pharmacyAccountId,
        CancellationToken cancellationToken)
    {
        ViewData["PharmacyName"] = await context.PharmacyAccounts
            .AsNoTracking()
            .Where(account => account.Id == pharmacyAccountId)
            .Select(account => account.PharmacyIdFkNavigation.Name)
            .SingleAsync(cancellationToken);
    }
}
