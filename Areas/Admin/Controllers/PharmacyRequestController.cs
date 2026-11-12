using System.Globalization;
using darooyab.Models;
using darooyab_rewrite.Areas.Admin.ViewModels;
using darooyab_rewrite.Services.Authentication;
using darooyab_rewrite.Services.Authentication.Pharmacy;
using darooyab_rewrite.Services.PharmacyPrescriptions;
using darooyab_rewrite.Services.PharmacyRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace darooyab_rewrite.Areas.Admin.Controllers;

[Area("Admin")]
[Route("PharmacyRequest")]
[Authorize(Policy = PharmacyAuthenticationDefaults.AuthorizationPolicy)]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class PharmacyRequestController(
    darooConContext context,
    IAuthenticationService authenticationService,
    IPharmacyRequestService pharmacyRequestService,
    IPharmacyExchangeService pharmacyExchangeService,
    IPharmacyPrescriptionService pharmacyPrescriptionService) : Controller
{
    [HttpGet("DarooyabPrescription")]
    public async Task<IActionResult> DarooyabPrescription(
        string? trackingCode,
        bool? isReviewed = false,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        await SetPharmacyNameAsync(pharmacyAccountId.Value, cancellationToken);
        return View(new PharmacyPrescriptionPageViewModel
        {
            TrackingCode = trackingCode,
            IsReviewed = isReviewed,
            Page = Math.Max(1, page)
        });
    }

    [HttpGet("SearchPrescriptions")]
    public async Task<IActionResult> SearchPrescriptions(
        string? trackingCode,
        bool? isReviewed,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        var model = await pharmacyPrescriptionService.GetPageAsync(
            pharmacyAccountId.Value,
            trackingCode,
            isReviewed,
            page,
            cancellationToken);
        return Json(new
        {
            items = model.Items,
            currentPage = model.Page,
            totalPages = model.TotalPages,
            totalRecords = model.TotalRecords
        });
    }

    [HttpPost("UpdateIsReviewed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateIsReviewed(
        int id,
        bool isReviewed,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        var updated = await pharmacyPrescriptionService.UpdateReviewedAsync(
            pharmacyAccountId.Value,
            id,
            isReviewed,
            cancellationToken);
        return updated
            ? Json(new { success = true })
            : NotFound(new { success = false, message = "نسخه‌ی موردنظر در دسترس نیست." });
    }

    [HttpGet("SendSMS")]
    public async Task<IActionResult> SendSMS(
        int prescriptionId,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        var statusMessage = TempData["PharmacyPrescriptionSmsMessage"] as string;
        var model = await pharmacyPrescriptionService.GetSmsPageAsync(
            pharmacyAccountId.Value,
            prescriptionId,
            statusMessage,
            cancellationToken);
        if (model is null)
        {
            return NotFound();
        }

        ViewData["PharmacyName"] = model.PharmacyName;
        return View("SendSMS", model);
    }

    [HttpPost("SendSMSPost")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendSMSPost(
        int prescriptionId,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        var result = await pharmacyPrescriptionService.SendSmsAsync(
            pharmacyAccountId.Value,
            prescriptionId,
            cancellationToken);
        if (result.Status == PharmacyPrescriptionSmsSendStatus.NotFound)
        {
            return NotFound();
        }

        TempData["PharmacyPrescriptionSmsMessage"] = result.Message;
        return RedirectToAction(nameof(SendSMS), new { prescriptionId });
    }

    [HttpGet("AddOrEdit/{id:int?}")]
    public async Task<IActionResult> AddOrEdit(
        int? id,
        string? searchString,
        int page = 1,
        int? pageExdrug = null,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        var model = await pharmacyRequestService.GetPageAsync(
            pharmacyAccountId.Value,
            editId: id,
            searchString: searchString,
            page: pageExdrug ?? page,
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
        PharmacyRequestFormViewModel form,
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
                cancellationToken);
        }

        var result = await pharmacyRequestService.SaveAsync(
            pharmacyAccountId.Value,
            form,
            cancellationToken);
        if (result != PharmacyRequestSaveStatus.Succeeded)
        {
            ModelState.AddModelError(
                string.Empty,
                result switch
                {
                    PharmacyRequestSaveStatus.InvalidInput => "اطلاعات فرم معتبر نیست؛ لطفاً انتخاب‌ها را بررسی کنید.",
                    PharmacyRequestSaveStatus.NotFound => "درخواست موردنظر در دسترس نیست.",
                    PharmacyRequestSaveStatus.RestrictedBrand => "ثبت موجودی این برند مجاز نیست.",
                    PharmacyRequestSaveStatus.Conflict => "برای این برند چند درخواست فعال وجود دارد؛ برای اصلاح اطلاعات با پشتیبانی تماس بگیرید.",
                    PharmacyRequestSaveStatus.InsufficientBalance => "موجودی کیف پول برای خرید این رتبه کافی نیست؛ درخواست و تغییر قیمت ذخیره نشد.",
                    PharmacyRequestSaveStatus.RankUnavailable => "این رتبه هم‌زمان رزرو شده یا دیگر قابل خرید نیست؛ اطلاعات را تازه کنید.",
                    PharmacyRequestSaveStatus.RankAlreadyOwned => "برای این برند در استان شما یک رتبه‌ی فعال دارید.",
                    PharmacyRequestSaveStatus.InvalidBrandPrice => "قیمت یا واحد قیمت معتبر نیست.",
                    _ => "ثبت اطلاعات انجام نشد."
                });
            return await ReturnFormWithErrorsAsync(
                pharmacyAccountId.Value,
                form,
                cancellationToken);
        }

        TempData["PharmacyRequestMessage"] = form.Id.HasValue
            ? "درخواست با موفقیت ویرایش شد."
            : form.BuyRanking && form.UpdateBrandPrice
                ? "درخواست ثبت شد، رتبه خریداری شد و آخرین قیمت برند به‌روزرسانی شد."
                : form.BuyRanking
                    ? "درخواست ثبت شد و رتبه با موفقیت خریداری شد."
                    : form.UpdateBrandPrice
                        ? "درخواست ثبت شد و آخرین قیمت برند به‌روزرسانی شد."
                        : "درخواست با موفقیت ثبت شد.";
        return RedirectToAction(nameof(AddOrEdit));
    }

    [HttpGet("SearchBrands")]
    public async Task<IActionResult> SearchBrands(
        string? query,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        if (string.IsNullOrWhiteSpace(query))
        {
            return Json(Array.Empty<PharmacyRequestDrugOption>());
        }

        var results = await pharmacyRequestService.SearchBrandsAsync(query, cancellationToken);
        return Json(results);
    }

    [HttpGet("BrandDetails/{irc:long}")]
    public async Task<IActionResult> BrandDetails(
        long irc,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        var model = await pharmacyRequestService.GetBrandDetailsAsync(
            pharmacyAccountId.Value,
            irc,
            cancellationToken: cancellationToken);
        return model is null
            ? NotFound()
            : PartialView("_BrandDetails", model);
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        int id,
        string? searchString,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        var deleted = await pharmacyRequestService.DeleteAsync(
            pharmacyAccountId.Value,
            id,
            cancellationToken);
        TempData["PharmacyRequestMessage"] = deleted
            ? "درخواست با موفقیت حذف شد."
            : "درخواست موردنظر در دسترس نیست.";

        return RedirectToAction(nameof(AddOrEdit), new { searchString, page });
    }

    [HttpGet("TabadolManage")]
    public async Task<IActionResult> TabadolManage(
        int? id,
        string? searchString,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        var model = await pharmacyExchangeService.GetPageAsync(
            pharmacyAccountId.Value,
            editId: id,
            searchString: searchString,
            page: page,
            cancellationToken: cancellationToken);
        if (model.EditRequestNotFound)
        {
            return NotFound();
        }

        await SetPharmacyNameAsync(pharmacyAccountId.Value, cancellationToken);
        return View(model);
    }

    [HttpGet("TabadolManage/List")]
    public async Task<IActionResult> TabadolManageList(
        string? searchString,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        var model = await pharmacyExchangeService.GetPageAsync(
            pharmacyAccountId.Value,
            searchString: searchString,
            page: page,
            cancellationToken: cancellationToken);
        return PartialView("_TabadolManageList", model);
    }

    [HttpPost("TabadolManage")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveTabadolManage(
        [Bind(Prefix = "Form")] PharmacyExchangeCreateFormViewModel form,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        var result = ModelState.IsValid
            ? await pharmacyExchangeService.SaveBasicAsync(
                pharmacyAccountId.Value,
                form,
                cancellationToken)
            : PharmacyExchangeSaveStatus.InvalidInput;
        if (result == PharmacyExchangeSaveStatus.NotFound)
        {
            return NotFound();
        }

        if (result != PharmacyExchangeSaveStatus.Succeeded)
        {
            ModelState.AddModelError(
                string.Empty,
                result switch
                {
                    PharmacyExchangeSaveStatus.InvalidInput =>
                        "اطلاعات فرم معتبر نیست؛ لطفاً فیلدها و برندهای انتخاب‌شده را بررسی کنید.",
                    PharmacyExchangeSaveStatus.RestrictedBrand =>
                        "ثبت مازاد برای یکی از برندهای انتخاب‌شده مجاز نیست.",
                    PharmacyExchangeSaveStatus.Conflict =>
                        "برای یکی از برندها چند درخواست فعال وجود دارد؛ برای اصلاح اطلاعات با پشتیبانی تماس بگیرید.",
                    PharmacyExchangeSaveStatus.ExchangesExceedQuantity =>
                        "تعداد واردشده از مقدار تبادل‌شده کمتر است.",
                    _ => "ثبت اطلاعات انجام نشد."
                });
            var model = await pharmacyExchangeService.GetPageAsync(
                pharmacyAccountId.Value,
                form: new PharmacyExchangeFormViewModel
                {
                    SelectedIrcs = form.SelectedIrcs,
                    ExchangeLimitNum = form.ExchangeLimitNum,
                    DrugExpireMonth = form.DrugExpireMonth,
                    DrugExpireYear = form.DrugExpireYear
                },
                cancellationToken: cancellationToken);
            await SetPharmacyNameAsync(pharmacyAccountId.Value, cancellationToken);
            return View("TabadolManage", model);
        }

        TempData["PharmacyExchangeMessage"] = "مازاد دارویی با موفقیت ثبت شد.";
        return RedirectToAction(nameof(TabadolManage));
    }

    [HttpPost("TabadolManage/{id:int}/Expiry")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateTabadolExpiry(
        int id,
        [Bind(Prefix = "Form")] PharmacyExchangeExpiryFormViewModel form,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        var result = ModelState.IsValid
            ? await pharmacyExchangeService.UpdateExpiryAsync(
                pharmacyAccountId.Value,
                id,
                form.DrugExpireMonth,
                form.DrugExpireYear,
                cancellationToken)
            : PharmacyExchangeExpiryUpdateStatus.InvalidInput;

        if (result == PharmacyExchangeExpiryUpdateStatus.NotFound)
        {
            return NotFound();
        }

        if (result != PharmacyExchangeExpiryUpdateStatus.Succeeded)
        {
            if (ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "ماه یا سال انقضا معتبر نیست.");
            }

            var model = await pharmacyExchangeService.GetPageAsync(
                pharmacyAccountId.Value,
                editId: id,
                cancellationToken: cancellationToken);
            if (model.EditRequestNotFound)
            {
                return NotFound();
            }

            await SetPharmacyNameAsync(pharmacyAccountId.Value, cancellationToken);
            return View("TabadolManage", model);
        }

        TempData["PharmacyExchangeMessage"] = "تاریخ انقضا با موفقیت به‌روزرسانی شد.";
        return RedirectToAction(nameof(TabadolManage));
    }

    [HttpPost("TabadolManage/{id:int}/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTabadolManage(
        int id,
        string? searchString,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        var result = await pharmacyExchangeService.DeleteAsync(
            pharmacyAccountId.Value,
            id,
            cancellationToken);
        TempData["PharmacyExchangeMessage"] = result switch
        {
            PharmacyExchangeOperationStatus.Succeeded => "مازاد با موفقیت حذف شد.",
            PharmacyExchangeOperationStatus.HasExchanges =>
                "این مازاد سابقه تبادل دارد و قابل حذف نیست؛ در صورت نیاز آن را خاتمه دهید.",
            PharmacyExchangeOperationStatus.Conflict => "حذف به دلیل وابستگی ثبت‌شده انجام نشد.",
            _ => "مازاد موردنظر در دسترس نیست."
        };
        return RedirectToAction(nameof(TabadolManage), new { searchString, page });
    }

    [HttpPost("TabadolManage/{id:int}/Finish")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FinishTabadolManage(
        int id,
        string? searchString,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        var result = await pharmacyExchangeService.FinishAsync(
            pharmacyAccountId.Value,
            id,
            cancellationToken);
        TempData["PharmacyExchangeMessage"] = result == PharmacyExchangeOperationStatus.Succeeded
            ? "مازاد با موفقیت خاتمه یافت."
            : "مازاد موردنظر در دسترس نیست.";
        return RedirectToAction(nameof(TabadolManage), new { searchString, page });
    }

    private async Task<IActionResult> ReturnFormWithErrorsAsync(
        int pharmacyAccountId,
        PharmacyRequestFormViewModel form,
        CancellationToken cancellationToken)
    {
        var model = await pharmacyRequestService.GetPageAsync(
            pharmacyAccountId,
            form: form,
            cancellationToken: cancellationToken);
        await SetPharmacyNameAsync(pharmacyAccountId, cancellationToken);
        return View(nameof(AddOrEdit), model);
    }

    private async Task<int?> GetCurrentPharmacyAccountIdAsync(CancellationToken cancellationToken)
    {
        var user = authenticationService.GetCurrentUser();
        if (user is null ||
            user.UserType != PharmacyAuthenticationDefaults.UserType ||
            !int.TryParse(user.UserId, NumberStyles.None, CultureInfo.InvariantCulture, out var pharmacyAccountId))
        {
            return null;
        }

        var now = DateTime.Now;
        var isActive = await context.PharmacyAccounts
            .AsNoTracking()
            .AnyAsync(account =>
                account.Id == pharmacyAccountId &&
                account.AccountStatus == "2" &&
                account.DarooyabAdminApproved != false &&
                (account.AccountExpireDate == null || account.AccountExpireDate >= now),
                cancellationToken);

        return isActive ? pharmacyAccountId : null;
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
