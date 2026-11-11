using System.Globalization;
using darooyab.Models;
using darooyab_rewrite.Areas.Admin.ViewModels;
using darooyab_rewrite.Services.Authentication;
using darooyab_rewrite.Services.Authentication.Pharmacy;
using darooyab_rewrite.Services.PharmacyRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace darooyab_rewrite.Areas.Admin.Controllers;

[Area("Admin")]
[Route("/Wallet")]
[Authorize(Policy = PharmacyAuthenticationDefaults.AuthorizationPolicy)]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class WalletController(
    darooConContext context,
    IAuthenticationService authenticationService,
    IPharmacyWalletService walletService) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(
            cancellationToken,
            requireActive: false);
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

        var model = await walletService.GetPageAsync(
            pharmacyAccountId.Value,
            page,
            cancellationToken);
        ViewData["WalletMessage"] = TempData["WalletMessage"] as string;
        ViewData["WalletMessageType"] = TempData["WalletMessageType"] as string;
        return View(model);
    }

    [HttpPost("Charge")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("wallet-payment")]
    public async Task<IActionResult> Charge(
        PharmacyWalletTopUpForm form,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        if (!ModelState.IsValid || !form.AmountToman.HasValue)
        {
            SetMessage("مبلغ شارژ معتبر نیست.", "error");
            return RedirectToAction(nameof(Index));
        }

        var result = await walletService.StartTopUpAsync(
            pharmacyAccountId.Value,
            form.AmountToman.Value,
            cancellationToken);
        if (result.Status == PharmacyWalletPaymentStartStatus.Started &&
            Uri.TryCreate(result.PaymentUrl, UriKind.Absolute, out var paymentUri) &&
            paymentUri.Scheme == Uri.UriSchemeHttps &&
            paymentUri.Host == "www.zarinpal.com")
        {
            return Redirect(paymentUri.ToString());
        }

        SetMessage(
            result.Status switch
            {
                PharmacyWalletPaymentStartStatus.NotConfigured =>
                    "درگاه پرداخت برای این محیط پیکربندی نشده است.",
                PharmacyWalletPaymentStartStatus.InvalidAmount =>
                    "مبلغ شارژ باید بین ۱,۱۰۰ تا ۲۰,۰۰۰,۰۰۰ تومان باشد.",
                _ => "ایجاد درخواست پرداخت ناموفق بود؛ لطفاً بعداً دوباره تلاش کنید."
            },
            "error");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Verify/{transactionId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Verify(
        int transactionId,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        var result = await walletService.VerifyTopUpAsync(
            pharmacyAccountId.Value,
            transactionId,
            cancellationToken);
        SetMessage(GetVerificationMessage(result), result is
            PharmacyWalletPaymentVerificationStatus.Succeeded or
            PharmacyWalletPaymentVerificationStatus.AlreadyPaid ? "success" : "error");
        return RedirectToAction(nameof(Index), new { page });
    }

    [HttpGet("Callback")]
    public async Task<IActionResult> Callback(
        string? Authority,
        string? Status,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        if (!string.Equals(Status, "OK", StringComparison.OrdinalIgnoreCase))
        {
            SetMessage("پرداخت لغو شد یا از سوی درگاه تأیید نشد؛ در صورت کسر وجه، وضعیت را بررسی کنید.", "error");
            return RedirectToAction(nameof(Index));
        }

        if (string.IsNullOrWhiteSpace(Authority) || Authority.Length > 128)
        {
            SetMessage("شناسه‌ی پرداخت معتبر نیست.", "error");
            return RedirectToAction(nameof(Index));
        }

        var result = await walletService.VerifyCallbackAsync(
            pharmacyAccountId.Value,
            Authority,
            cancellationToken);
        SetMessage(GetVerificationMessage(result), result is
            PharmacyWalletPaymentVerificationStatus.Succeeded or
            PharmacyWalletPaymentVerificationStatus.AlreadyPaid ? "success" : "error");
        return RedirectToAction(nameof(Index));
    }

    private async Task<int?> GetCurrentPharmacyAccountIdAsync(
        CancellationToken cancellationToken,
        bool requireActive = true)
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
            .AnyAsync(account => account.Id == id &&
                (!requireActive ||
                 (account.AccountStatus == "2" &&
                  account.DarooyabAdminApproved != false &&
                  (account.AccountExpireDate == null || account.AccountExpireDate >= DateTime.Now))),
                cancellationToken);
        return active ? id : null;
    }

    private static string GetVerificationMessage(PharmacyWalletPaymentVerificationStatus status) =>
        status switch
        {
            PharmacyWalletPaymentVerificationStatus.Succeeded =>
                "پرداخت با موفقیت تأیید و به کیف پول اضافه شد.",
            PharmacyWalletPaymentVerificationStatus.AlreadyPaid =>
                "این پرداخت قبلاً تأیید شده است.",
            PharmacyWalletPaymentVerificationStatus.NotFound =>
                "تراکنش پرداخت برای این حساب پیدا نشد.",
            PharmacyWalletPaymentVerificationStatus.NotConfigured =>
                "تنظیمات درگاه پرداخت در دسترس نیست.",
            PharmacyWalletPaymentVerificationStatus.ProviderUnavailable =>
                "ارتباط با درگاه پرداخت برقرار نشد؛ دوباره تلاش کنید.",
            PharmacyWalletPaymentVerificationStatus.NotPending =>
                "این تراکنش در وضعیت قابل بررسی نیست.",
            _ => "درگاه پرداخت، تراکنش را تأیید نکرد."
        };

    private void SetMessage(string message, string type)
    {
        TempData["WalletMessage"] = message;
        TempData["WalletMessageType"] = type;
    }
}
