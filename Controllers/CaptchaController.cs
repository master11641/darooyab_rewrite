using System.ComponentModel.DataAnnotations;
using darooyab_rewrite.Services.Captcha;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace darooyab_rewrite.Controllers;

/// <summary>Creates and validates short-lived CAPTCHA challenges.</summary>
[ApiController]
[Route("api/captcha")]
[EnableRateLimiting("captcha-api")]
[Produces("application/json")]
public sealed class CaptchaController(
    ICaptchaService captchaService,
    ILogger<CaptchaController> logger) : ControllerBase
{
    /// <summary>Creates an image challenge bound to the current browser.</summary>
    [HttpPost("generate")]
    [RequestSizeLimit(4096)]
    [ProducesResponseType<CaptchaGenerateResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<CaptchaGenerateResponse>> Generate(
        [FromBody] CaptchaGenerateRequest request,
        CancellationToken cancellationToken)
    {
        if (!captchaService.IsEnabled)
        {
            return NotFound();
        }

        try
        {
            var challenge = await captchaService.GenerateAsync(
                CaptchaClientCookie.GetOrCreate(HttpContext),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                cancellationToken);
            return Ok(new CaptchaGenerateResponse(
                challenge.ChallengeId,
                Convert.ToBase64String(challenge.ImageBytes),
                "image/png",
                Math.Max(0, (int)Math.Ceiling(
                    (challenge.ExpiresAt - DateTimeOffset.UtcNow).TotalSeconds))));
        }
        catch (CaptchaRateLimitException)
        {
            return StatusCode(
                StatusCodes.Status429TooManyRequests,
                new ProblemDetails
                {
                    Status = StatusCodes.Status429TooManyRequests,
                    Title = "Too many CAPTCHA requests.",
                    Detail = "Wait before requesting another challenge."
                });
        }
        catch (CaptchaStoreUnavailableException exception)
        {
            logger.LogError(exception, "The CAPTCHA challenge store is unavailable.");
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new ProblemDetails
                {
                    Status = StatusCodes.Status503ServiceUnavailable,
                    Title = "CAPTCHA is temporarily unavailable.",
                    Detail = "Please try again later."
                });
        }
    }

    /// <summary>Consumes a challenge after checking the submitted answer.</summary>
    [HttpPost("validate")]
    [RequestSizeLimit(4096)]
    [ProducesResponseType<CaptchaValidationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<CaptchaValidationResponse>> Validate(
        [FromBody] CaptchaValidationRequest request,
        CancellationToken cancellationToken)
    {
        if (!captchaService.IsEnabled)
        {
            return NotFound();
        }

        try
        {
            var result = await captchaService.ValidateAsync(
                request.ChallengeId,
                request.Answer,
                CaptchaClientCookie.GetOrCreate(HttpContext),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                cancellationToken);
            if (result.Status == CaptchaValidationStatus.RateLimited)
            {
                return StatusCode(
                    StatusCodes.Status429TooManyRequests,
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status429TooManyRequests,
                        Title = "Too many CAPTCHA validation requests.",
                        Detail = "Wait before trying again."
                    });
            }

            return Ok(new CaptchaValidationResponse(result.IsValid));
        }
        catch (CaptchaStoreUnavailableException exception)
        {
            logger.LogError(exception, "The CAPTCHA challenge store is unavailable.");
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new ProblemDetails
                {
                    Status = StatusCodes.Status503ServiceUnavailable,
                    Title = "CAPTCHA is temporarily unavailable.",
                    Detail = "Please try again later."
                });
        }
    }

}

/// <summary>Image and identifier returned for a newly-created CAPTCHA challenge.</summary>
public sealed record CaptchaGenerateResponse(
    string ChallengeId,
    string ImageBase64,
    string ContentType,
    int ExpiresInSeconds);

/// <summary>Empty JSON request body for generating a CAPTCHA challenge.</summary>
public sealed record CaptchaGenerateRequest
{
}

/// <summary>Input used to validate and consume a CAPTCHA challenge.</summary>
public sealed record CaptchaValidationRequest
{
    [Required]
    [StringLength(43, MinimumLength = 43)]
    public required string ChallengeId { get; init; }

    [Required]
    [StringLength(8, MinimumLength = 4)]
    [RegularExpression("^[A-Za-z2-9]+$")]
    public required string Answer { get; init; }
}

/// <summary>Result of a CAPTCHA answer check.</summary>
public sealed record CaptchaValidationResponse(bool Valid);
