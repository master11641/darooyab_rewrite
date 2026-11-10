#nullable enable

using darooyab.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRateLimiter(options =>
{
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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
