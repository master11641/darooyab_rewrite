#nullable enable

using darooyab.Models;
using darooyab_rewrite.Middleware;
using darooyab_rewrite.Services.Authentication;
using darooyab_rewrite.Services.Authentication.Pharmacy;
using darooyab_rewrite.Services.Captcha;
using darooyab_rewrite.Services.Comments;
using darooyab_rewrite.Services.PharmacyRequests;
using darooyab_rewrite.Services.PharmacyDrugDistributions;
using darooyab_rewrite.Services.PharmacyBrandSuggestions;
using darooyab_rewrite.Services.PharmacyDrugShortages;
using darooyab_rewrite.Services.PharmacyPrescriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using darooyab_rewrite.Services.PharmacySpecialties;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
var captchaOptions = builder.Configuration.GetSection("Captcha").Get<CaptchaOptions>() ?? new CaptchaOptions();
var authenticationOptions = builder.Configuration
    .GetSection(AuthenticationCoreOptions.SectionName)
    .Get<AuthenticationCoreOptions>() ?? new AuthenticationCoreOptions();

builder.Services.AddControllersWithViews();
builder.Services.AddCaptcha(builder.Configuration);
builder.Services.AddAuthenticationCore(builder.Configuration, builder.Environment);
builder.Services.AddPharmacyAuthenticationProvider();
builder.Services.AddScoped<IPharmacyRequestService, PharmacyRequestService>();
builder.Services.AddScoped<IPharmacyDrugDistributionService, PharmacyDrugDistributionService>();
builder.Services.AddScoped<IPharmacyBrandSuggestionService, PharmacyBrandSuggestionService>();
builder.Services.AddScoped<IPharmacyDrugShortageService, PharmacyDrugShortageService>();
builder.Services.AddScoped<IPharmacyExchangeService, PharmacyExchangeService>();
builder.Services.AddScoped<IPharmacyPrescriptionService, PharmacyPrescriptionService>();
builder.Services.AddScoped<PharmacySpecialtiesService>();
builder.Services.Configure<PrescriptionSmsOptions>(
    builder.Configuration.GetSection(PrescriptionSmsOptions.SectionName));
builder.Logging.AddFilter(
    "System.Net.Http.HttpClient.IPharmacyPrescriptionSmsSender",
    LogLevel.None);
builder.Services.AddHttpClient<IPharmacyPrescriptionSmsSender, KavenegarPrescriptionSmsSender>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(15);
});
builder.Services.Configure<ZarinPalOptions>(
    builder.Configuration.GetSection(ZarinPalOptions.SectionName));
builder.Services.AddHttpClient<IPharmacyWalletService, PharmacyWalletService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(15);
});
builder.Services.AddScoped<IPublicCommentService, PublicCommentService>();
builder.Services.AddScoped<IPublicCompanyMessageService, PublicCompanyMessageService>();
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("captcha-api", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = captchaOptions.MaxApiRequestsPerIp,
                Window = captchaOptions.ApiRateLimitWindow,
                QueueLimit = 0,
                AutoReplenishment = true
            }));
    options.AddPolicy("contact-submission", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
    options.AddPolicy("public-feedback", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = captchaOptions.MaxPublicFeedbackRequestsPerIp,
                Window = captchaOptions.PublicFeedbackRateLimitWindow,
                QueueLimit = 0,
                AutoReplenishment = true
            }));
    options.AddPolicy("pharmacy-login", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = authenticationOptions.MaxLoginAttemptsPerIp,
                Window = authenticationOptions.LoginRateLimitWindow,
                QueueLimit = 0,
                AutoReplenishment = true
            }));
    options.AddPolicy("wallet-payment", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 4,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});
builder.Services.AddDbContext<darooConContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Darooyab");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "Configure the Darooyab connection string using ConnectionStrings:Darooyab before accessing the database.");
    }

    options.UseSqlServer(connectionString);
});

var app = builder.Build();

app.UseMiddleware<CaptchaNoStoreMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "G-{DrugId}/{EngGenericName}",
    pattern: "G-{DrugId:int}/{EngGenericName?}",
    defaults: new
    {
        controller = "Home",
        action = "DrugInfo"
    });

app.MapControllerRoute(
    name: "BrandInfo",
    pattern: "B-{IRC:long}/{BrandEngName?}",
    defaults: new
    {
        controller = "Home",
        action = "BrandInfo"
    });

app.MapControllerRoute(
    name: "NewsApp/{id}",
    pattern: "NewsApp/{articleId:int}",
    defaults: new
    {
        controller = "Home",
        action = "Section",
        id = "news"
    });

app.MapControllerRoute(
    name: "AllNews",
    pattern: "NewsApp",
    defaults: new
    {
        controller = "Home",
        action = "Section",
        id = "news"
    });

app.MapControllerRoute(
    name: "DrugHerbal",
    pattern: "DrugHerbal",
    defaults: new
    {
        controller = "Home",
        action = "DrugHerbal"
    });

app.MapControllerRoute(
    name: "AvailabilityByProvinceIRC",
    pattern: "availability/IRC/{IRC:long}/{ProvinceId:int}/{ProvinceSlug?}",
    defaults: new
    {
        controller = "Home",
        action = "AvailabilityBrand"
    });

app.MapControllerRoute(
    name: "AvailabilityIRC",
    pattern: "availability/IRC/{IRC:long}",
    defaults: new
    {
        controller = "Home",
        action = "AvailabilityBrand"
    });

app.MapControllerRoute(
    name: "AvailabilityByProvince",
    pattern: "availability/{DrugId:int}/{ProvinceId:int}/{ProvinceSlug?}",
    defaults: new
    {
        controller = "Home",
        action = "AvailabilityGeneric"
    });

app.MapControllerRoute(
    name: "Availability",
    pattern: "availability/{DrugId:int}",
    defaults: new
    {
        controller = "Home",
        action = "AvailabilityGeneric"
    });

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
