using darooyab_rewrite.Services.Authentication;
using darooyab_rewrite.Services.Captcha;
using darooyab_rewrite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Data.SqlClient;

namespace darooyab_rewrite.Controllers;

[Route("ph")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class PharmacyAuthenticationController(
    IAuthenticationService authenticationService,
    ICaptchaService captchaService,
    ILogger<PharmacyAuthenticationController> logger) : Controller
{
    [HttpGet("login", Name = "PharmacyLogin")]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        var model = new PharmacyLoginViewModel { ReturnUrl = returnUrl };
        PrepareViewData();
        return View(model);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("pharmacy-login")]
    [RequestSizeLimit(16_384)]
    public async Task<IActionResult> Login(
        PharmacyLoginViewModel model,
        CancellationToken cancellationToken)
    {
        PrepareViewData();
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (captchaService.IsEnabled)
        {
            if (string.IsNullOrWhiteSpace(model.CaptchaChallengeId) ||
                string.IsNullOrWhiteSpace(model.CaptchaAnswer))
            {
                ModelState.AddModelError(string.Empty, "کد امنیتی را وارد کنید.");
                return View(model);
            }

            try
            {
                var captchaResult = await captchaService.ValidateAsync(
                    model.CaptchaChallengeId,
                    model.CaptchaAnswer,
                    CaptchaClientCookie.GetOrCreate(HttpContext),
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    cancellationToken);
                if (!captchaResult.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "کد امنیتی معتبر نیست؛ کد تازه‌ای دریافت کنید.");
                    return View(model);
                }
            }
            catch (CaptchaStoreUnavailableException exception)
            {
                logger.LogError(exception, "The CAPTCHA challenge store is unavailable during pharmacy login.");
                Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                ModelState.AddModelError(string.Empty, "ورود موقتاً امکان‌پذیر نیست؛ بعداً دوباره تلاش کنید.");
                return View(model);
            }
        }

        try
        {
            var result = await authenticationService.LoginAsync(
                new AuthenticationCredentials(model.UserName, model.Password),
                cancellationToken);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "اطلاعات ورود نامعتبر است یا امکان ورود به حساب وجود ندارد.");
                return View(model);
            }
        }
        catch (SqlException exception)
        {
            logger.LogError(
                "Pharmacy login failed because the database is unavailable. SQL error number: {SqlErrorNumber}.",
                exception.Number);
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            ModelState.AddModelError(string.Empty, "ورود موقتاً امکان‌پذیر نیست؛ بعداً دوباره تلاش کنید.");
            return View(model);
        }

        var returnUrl = !string.IsNullOrWhiteSpace(model.ReturnUrl) &&
                        Url.IsLocalUrl(model.ReturnUrl)
            ? model.ReturnUrl
            : "/Admin/Dashboard";
        return LocalRedirect(returnUrl);
    }

    [HttpPost("logout")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(string? returnUrl = null)
    {
        await authenticationService.LogoutAsync();
        return LocalRedirect(GetLocalReturnUrl(returnUrl));
    }

    private string GetLocalReturnUrl(string? returnUrl)
    {
        return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? returnUrl
            : "/";
    }

    private void PrepareViewData()
    {
        const string pageTitle = "ورود مدیر داروخانه";
        const string pageDescription = "ورود به حساب کاربری داروخانه در سامانه‌ی دارویاب.";
        const string canonicalUrl = "https://www.darooyab.ir/ph/login";

        ViewData["Title"] = pageTitle;
        ViewData["MetaDescription"] = pageDescription;
        ViewData["CanonicalUrl"] = canonicalUrl;
        ViewData["OpenGraphUrl"] = canonicalUrl;
        ViewData["OpenGraphTitle"] = pageTitle;
        ViewData["OpenGraphDescription"] = pageDescription;
        ViewData["OpenGraphType"] = "website";
        ViewData["TwitterCard"] = "summary";
        ViewData["CaptchaEnabled"] = captchaService.IsEnabled;
    }
}
