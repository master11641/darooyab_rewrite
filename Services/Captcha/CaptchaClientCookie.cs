using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace darooyab_rewrite.Services.Captcha;

public static class CaptchaClientCookie
{
    public const string CookieName = "darooyab.captcha-client";

    public static string GetOrCreate(HttpContext context)
    {
        if (context.Request.Cookies.TryGetValue(CookieName, out var existing) &&
            IsValidBinding(existing))
        {
            return existing;
        }

        var value = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
        context.Response.Cookies.Append(
            CookieName,
            value,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = context.Request.IsHttps ||
                         !context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment(),
                SameSite = SameSiteMode.Strict,
                IsEssential = true,
                Path = "/",
                MaxAge = TimeSpan.FromDays(30)
            });
        return value;
    }

    private static bool IsValidBinding(string value)
    {
        if (value.Length != 43)
        {
            return false;
        }

        try
        {
            return WebEncoders.Base64UrlDecode(value).Length == 32;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
