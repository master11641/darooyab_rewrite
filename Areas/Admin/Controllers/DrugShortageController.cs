using System.Globalization;
using darooyab.Models;
using darooyab_rewrite.Areas.Admin.ViewModels;
using darooyab_rewrite.Services.Authentication;
using darooyab_rewrite.Services.Authentication.Pharmacy;
using darooyab_rewrite.Services.PharmacyDrugShortages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace darooyab_rewrite.Areas.Admin.Controllers;

[Area("Admin")]
[Route("/DrugShortage")]
[Authorize(Policy = PharmacyAuthenticationDefaults.AuthorizationPolicy)]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class DrugShortageController(
    darooConContext context,
    IAuthenticationService authenticationService,
    IPharmacyDrugShortageService drugShortageService) : Controller
{
    [HttpGet("AddOrEdit")]
    public async Task<IActionResult> AddOrEdit(
        int? id,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        var model = await drugShortageService.GetPageAsync(
            pharmacyAccountId.Value,
            editId: id,
            page: page,
            cancellationToken: cancellationToken);
        if (model.EditRequestNotFound)
        {
            return NotFound();
        }

        await SetPharmacyNameAsync(pharmacyAccountId.Value, cancellationToken);
        return View(model);
    }

    [HttpPost("AddOrEdit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOrEdit(
        PharmacyDrugShortageFormViewModel form,
        int page = 1,
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
            return await ReturnFormWithErrorsAsync(
                pharmacyAccountId.Value,
                form,
                page,
                cancellationToken);
        }

        var result = await drugShortageService.SaveAsync(
            pharmacyAccountId.Value,
            form,
            cancellationToken);
        if (result != PharmacyDrugShortageSaveStatus.Succeeded)
        {
            ModelState.AddModelError(
                string.Empty,
                result switch
                {
                    PharmacyDrugShortageSaveStatus.InvalidInput =>
                        "اطلاعات فرم معتبر نیست؛ لطفاً داروی انتخاب‌شده را بررسی کنید.",
                    PharmacyDrugShortageSaveStatus.NotFound =>
                        "درخواست موردنظر در دسترس نیست یا متعلق به این داروخانه نیست.",
                    PharmacyDrugShortageSaveStatus.RestrictedBrand =>
                        "ثبت کسری برای این برند مجاز نیست.",
                    _ => "ثبت اطلاعات انجام نشد."
                });
            return await ReturnFormWithErrorsAsync(
                pharmacyAccountId.Value,
                form,
                page,
                cancellationToken);
        }

        TempData["PharmacyDrugShortageMessage"] = form.Id.HasValue
            ? "کسری داروخانه با موفقیت ویرایش شد."
            : "کسری داروخانه با موفقیت ثبت شد.";
        return RedirectToAction(nameof(AddOrEdit));
    }

    [HttpGet("Search")]
    public async Task<IActionResult> Search(
        int searchType,
        string? query,
        CancellationToken cancellationToken = default)
    {
        if (!await EnsureActiveAccountAsync(cancellationToken))
        {
            return Challenge();
        }

        if (searchType is not (1 or 2))
        {
            return BadRequest();
        }

        var results = string.IsNullOrWhiteSpace(query)
            ? Array.Empty<PharmacyDrugShortageOption>()
            : await drugShortageService.SearchAsync(searchType, query, cancellationToken);
        return Json(results);
    }

    [HttpGet("Townships")]
    public async Task<IActionResult> Townships(
        int provinceId,
        CancellationToken cancellationToken = default)
    {
        if (!await EnsureActiveAccountAsync(cancellationToken))
        {
            return Challenge();
        }

        if (provinceId <= 0)
        {
            return BadRequest();
        }

        var townships = await drugShortageService.GetTownshipsAsync(provinceId, cancellationToken);
        return Json(townships);
    }

    [HttpPost("ExchangeOffers")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExchangeOffers(
        PharmacyDrugShortageSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await EnsureActiveAccountAsync(cancellationToken))
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var pharmacyAccountId = GetCurrentUserPharmacyAccountId();
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        var model = await drugShortageService.GetOffersAsync(
            pharmacyAccountId.Value,
            request,
            cancellationToken);
        return model is null
            ? NotFound()
            : PartialView("_ExchangeOffers", model);
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        int id,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        var result = await drugShortageService.DeleteAsync(
            pharmacyAccountId.Value,
            id,
            cancellationToken);
        var message = result switch
        {
            PharmacyDrugShortageDeleteStatus.Succeeded => "کسری با موفقیت حذف شد.",
            PharmacyDrugShortageDeleteStatus.HasExchanges =>
                "این کسری سابقه تبادل دارد و قابل حذف نیست.",
            _ => "درخواست موردنظر در دسترس نیست."
        };

        if (IsAjaxRequest())
        {
            return result == PharmacyDrugShortageDeleteStatus.Succeeded
                ? Json(new { success = true, message })
                : Conflict(new { success = false, message });
        }

        TempData["PharmacyDrugShortageMessage"] = message;
        return RedirectToAction(nameof(AddOrEdit), new { page });
    }

    private async Task<IActionResult> ReturnFormWithErrorsAsync(
        int pharmacyAccountId,
        PharmacyDrugShortageFormViewModel form,
        int page,
        CancellationToken cancellationToken)
    {
        var model = await drugShortageService.GetPageAsync(
            pharmacyAccountId,
            form: form,
            page: page,
            cancellationToken: cancellationToken);
        await SetPharmacyNameAsync(pharmacyAccountId, cancellationToken);
        return View(nameof(AddOrEdit), model);
    }

    private async Task<bool> EnsureActiveAccountAsync(CancellationToken cancellationToken)
    {
        if (await GetCurrentPharmacyAccountIdAsync(cancellationToken) is not null)
        {
            return true;
        }

        await authenticationService.LogoutAsync();
        return false;
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

    private bool IsAjaxRequest() =>
        string.Equals(
            Request.Headers["X-Requested-With"],
            "XMLHttpRequest",
            StringComparison.OrdinalIgnoreCase);
}
