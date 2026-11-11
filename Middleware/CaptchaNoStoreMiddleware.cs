namespace darooyab_rewrite.Middleware;

public sealed class CaptchaNoStoreMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments(
                "/api/captcha",
                StringComparison.OrdinalIgnoreCase))
        {
            context.Response.OnStarting(static state =>
            {
                var response = (HttpResponse)state;
                response.Headers.CacheControl = "no-store, no-cache, max-age=0, must-revalidate";
                response.Headers.Pragma = "no-cache";
                response.Headers.Expires = "0";
                response.Headers["X-Content-Type-Options"] = "nosniff";
                return Task.CompletedTask;
            }, context.Response);
        }

        await next(context);
    }
}
