using darooyab_rewrite.Services.Captcha;
using darooyab_rewrite.Services.Comments;
using darooyab_rewrite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Data.SqlClient;

namespace darooyab_rewrite.Controllers;

[Route("")]
public sealed class PublicFeedbackController(
    ICaptchaService captchaService,
    IPublicCommentService commentService,
    IPublicCompanyMessageService companyMessageService,
    ILogger<PublicFeedbackController> logger) : Controller
{
    [HttpPost("comments/submit")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("public-feedback")]
    [RequestSizeLimit(16_384)]
    public async Task<IActionResult> SubmitComment(
        PublicCommentSubmission submission,
        CancellationToken cancellationToken)
    {
        var returnUrl = GetReturnUrl(submission.ReturnUrl);
        if (!ModelState.IsValid || !string.IsNullOrWhiteSpace(submission.Website))
        {
            TempData["PublicFeedbackStatus"] = "invalid";
            return LocalRedirect(returnUrl);
        }

        try
        {
            var validation = await captchaService.ValidateAsync(
                submission.CaptchaChallengeId,
                submission.CaptchaAnswer,
                CaptchaClientCookie.GetOrCreate(HttpContext),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                cancellationToken);
            if (!validation.IsValid)
            {
                TempData["PublicFeedbackStatus"] = "captcha";
                return LocalRedirect(returnUrl);
            }

            var submitted = await commentService.SubmitAsync(submission, cancellationToken);
            TempData["PublicFeedbackStatus"] = submitted ? "success" : "invalid";
        }
        catch (CaptchaStoreUnavailableException exception)
        {
            logger.LogError(exception, "The CAPTCHA challenge store is unavailable for comment submission.");
            TempData["PublicFeedbackStatus"] = "unavailable";
        }
        catch (SqlException exception)
        {
            logger.LogError(
                "Could not save public comment. SQL error number: {SqlErrorNumber}.",
                exception.Number);
            TempData["PublicFeedbackStatus"] = "unavailable";
        }

        return LocalRedirect(returnUrl);
    }

    [HttpPost("companies/contact")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("public-feedback")]
    [RequestSizeLimit(16_384)]
    public async Task<IActionResult> SubmitCompanyMessage(
        CompanyMessageSubmission submission,
        CancellationToken cancellationToken)
    {
        var returnUrl = GetReturnUrl(submission.ReturnUrl);
        if (!ModelState.IsValid || !string.IsNullOrWhiteSpace(submission.Website))
        {
            TempData["PublicFeedbackStatus"] = "invalid";
            return LocalRedirect(returnUrl);
        }

        try
        {
            var validation = await captchaService.ValidateAsync(
                submission.CaptchaChallengeId,
                submission.CaptchaAnswer,
                CaptchaClientCookie.GetOrCreate(HttpContext),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                cancellationToken);
            if (!validation.IsValid)
            {
                TempData["PublicFeedbackStatus"] = "captcha";
                return LocalRedirect(returnUrl);
            }

            var submitted = await companyMessageService.SubmitAsync(
                submission,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                cancellationToken);
            TempData["PublicFeedbackStatus"] = submitted ? "success" : "invalid";
        }
        catch (CaptchaStoreUnavailableException exception)
        {
            logger.LogError(exception, "The CAPTCHA challenge store is unavailable for company messaging.");
            TempData["PublicFeedbackStatus"] = "unavailable";
        }
        catch (SqlException exception)
        {
            logger.LogError(
                "Could not save direct company message. SQL error number: {SqlErrorNumber}.",
                exception.Number);
            TempData["PublicFeedbackStatus"] = "unavailable";
        }

        return LocalRedirect(returnUrl);
    }

    private string GetReturnUrl(string? requestedUrl)
    {
        return !string.IsNullOrWhiteSpace(requestedUrl) && Url.IsLocalUrl(requestedUrl)
            ? requestedUrl
            : "/";
    }
}
