# هسته‌ی احراز هویت

این راهنما رفتار و نحوه‌ی گسترش هسته‌ی احراز هویت پیاده‌شده در برنامه را توضیح می‌دهد. ورود عملیاتی فعلی فقط برای مدیران داروخانه و فقط از جدول موجود `PharmacyAccount` است؛ هیچ جدول کاربر یا Provider دیگری ایجاد نشده است.

## مسیرها و جریان ورود

| عملیات | مسیر | روش | توضیح |
|---|---|---|---|
| نمایش فرم ورود داروخانه | `/ph/login` | GET | فرم RTL با CAPTCHA موجود |
| بررسی اطلاعات ورود | `/ph/login` | POST | Anti-Forgery، اعتبارسنجی سمت سرور و Rate Limiting |
| خروج | `/ph/logout` | POST | نیازمند احراز هویت و Anti-Forgery |

ورود موفق به Cookie استاندارد ASP.NET Core منجر می‌شود. در صورت وجود `returnUrl`، مقصد تنها وقتی پذیرفته می‌شود که `Url.IsLocalUrl` آن را محلی تشخیص دهد؛ در غیر این صورت کاربر به `/Admin/Dashboard` منتقل می‌شود. داشبورد نام داروخانه را از دیتابیس می‌خواند و فعال بودن حساب را دوباره بررسی می‌کند. نام کاربری/رمز نادرست، کاربر ناشناخته و حساب غیرفعال پاسخ عمومی یکسان دریافت می‌کنند. خطای دسترسی به دیتابیس یا CAPTCHA پاسخ موقتاً unavailable می‌دهد و با اطلاعات فنی غیرحساس در log ثبت می‌شود.

## ساختار و مسئولیت اجزا

هسته‌ی عمومی در `Services/Authentication` از Entity و schema مستقل است:

| نوع | مسئولیت |
|---|---|
| `IAuthenticationService` | ورود/خروج با Cookie، دریافت principal و کاربر جاری، وضعیت احراز هویت |
| `AuthenticationService` | انتخاب Provider، ساخت Claims حداقلی و صدور/ابطال Cookie |
| `IUserAuthenticationProvider` | قرارداد عمومی اعتبارسنجی credential در یک منبع |
| `AuthenticationCredentials` | نام کاربری و رمز ورودی؛ در Claims ذخیره نمی‌شود |
| `AuthenticatedUser` | مدل عمومی نتیجه‌ی Provider شامل شناسه، نوع، نام، Provider و Roleهای اختیاری |
| `AuthenticationResult` | نتیجه‌ی موفق/ناموفق بدون افشای جزئیات حساب |
| `AuthenticationCoreOptions` | Options مربوط به Cookie و اعتبارسنجی تنظیمات |
| `AuthenticationClaimTypes` | نام Claimهای توسعه‌یافته‌ی هسته |

فقط `Services/Authentication/Pharmacy/PharmacyAuthenticationProvider` به `PharmacyAccount` و `darooConContext` وابسته است. مسئولیت‌های مخصوص این Provider، شامل قواعد credential، PasswordHasher و Policy خودش، در همان زیرپوشه ثبت شده‌اند.

## اعتبارسنجی رمز و سازگاری جدول مشترک با پروژه‌ی قدیمی

پروژه‌ی قدیمی و جدید هر دو به جدول موجود `PharmacyAccount` و ستون `PasswordHash` دسترسی دارند. پروژه‌ی قدیمی برای ورود داروخانه، مقدار رمز پس از تبدیل ارقام فارسی به انگلیسی را با هر یک از این دو مقدار تطبیق می‌دهد:

1. SHA-256 روی بایت‌های UTF-8 رمز نرمال‌شده، با خروجی Hex بزرگ (uppercase). پیاده‌سازی مرجع در `darooyab_build_expired/Common/Extensions.cs`, `ToSHA256` است.
2. خود رمز نرمال‌شده به‌صورت متن، برای سازگاری با رکوردهای قدیمی که مقدار خام در ستون ذخیره شده است.

نام کاربری پروژه‌ی قدیمی و جدید `FounderMobileNum` است؛ ارقام فارسی به انگلیسی تبدیل می‌شوند و اگر شماره با `0` شروع نشده باشد، صفر به ابتدا افزوده می‌شود. منطق پروژه‌ی جدید در `LegacyPharmacyCredentialCompatibility` همین تبدیل و مقایسه‌ی SHA-256/متن خام را انجام می‌دهد. مقایسه‌ی رشته‌ی ذخیره‌شده با SHA-256/رمز خام در زمان بررسی با مقایسه‌ی زمان‌ثابت انجام می‌شود.

### انتخاب روش بررسی بر اساس قالب مقدار ذخیره‌شده

| مقدار `PasswordHash` | روش بررسی در پروژه‌ی جدید | سازگاری با ورود قدیمی |
|---|---|---|
| SHA-256 با Hex بزرگ (قالب Legacy) | محاسبه و مقایسه با SHA-256 رمز نرمال‌شده | بله؛ همان قالب مورد بررسی پروژه‌ی قدیمی است |
| رمز خام قدیمی | مقایسه با رمز نرمال‌شده | بله؛ پروژه‌ی قدیمی نیز مقدار خام را می‌پذیرد |
| قالب ASP.NET Core Identity با پیشوند `AQAAAA` | `PasswordHasher<PharmacyAccount>.VerifyHashedPassword` | معمولاً خیر؛ پروژه‌ی قدیمی این قالب را نمی‌خواند |
| مقدار نامعتبر/قالب ناشناخته | ورود رد می‌شود | نامشخص؛ بازیابی باید از مسیر رسمی reset انجام شود |

هش Identity شامل salt و خروجی مشتق‌شده است و رمز اصلی یا هش Legacy پیشین را در خود نگه نمی‌دارد. از تبدیل رشته‌ی Identity به SHA-256 یا هش‌کردن خودِ مقدار Identity خودداری کنید؛ این کار معادل رمز کاربر نیست و سازگاری را برنمی‌گرداند.

### رفتار Login و تضمین عدم تغییر حساب

جریان Login در پروژه‌ی جدید:

1. `PharmacyAuthenticationController.Login` ورودی فرم را اعتبارسنجی می‌کند؛ اگر CAPTCHA فعال باشد، پاسخ آن را در سمت سرور و قبل از اعتبارسنجی credential بررسی می‌کند. POST دارای Anti-Forgery و Rate Limiting است.
2. `IAuthenticationService.LoginAsync` Provider مناسب را انتخاب می‌کند. `PharmacyAuthenticationProvider.AuthenticateAsync` شماره را نرمال و حساب را با Query خواندنی (`AsNoTracking`) از `PharmacyAccount` می‌خواند.
3. رمز مطابق قالب مقدار `PasswordHash` بررسی می‌شود. رمز اشتباه، حساب ناشناخته یا hash نامعتبر رد می‌شود.
4. پس از تطبیق رمز، وضعیت حساب کنترل می‌شود: `AccountStatus == "2"`، تاریخ `AccountExpireDate` نگذشته باشد و `DarooyabAdminApproved != false`. مقدار null برای انقضا یا تأیید به‌تنهایی حساب را رد نمی‌کند.
5. Rehash فقط در صورت فعال بودن صریح تنظیم اختیاری و فقط برای رمز درستِ حساب فعال بررسی می‌شود. پیش‌فرض این گزینه خاموش است.
6. پس از موفقیت Provider، سرویس مرکزی Claims را می‌سازد و Cookie را صادر می‌کند؛ صدور Cookie خود به جدول حساب دسترسی نوشتنی ندارد.

وقتی `Authentication:EnablePasswordRehash=false` است، برای ورود موفق، ناموفق و حساب غیرفعال هیچ UPDATE یا `SaveChanges` روی `PharmacyAccount` اجرا نمی‌شود. در نتیجه در این حالت نه `PasswordHash` و نه سایر ستون‌های حساب به‌خاطر Login تغییر نمی‌کنند. پاسخ رمز نادرست، کاربر ناشناخته و حساب غیرمجاز عمومی و یکسان است. استثنای صریح، فعال‌کردن Rehash است که تنها `PasswordHash` را با UPDATE شرطی هدف می‌گیرد.

نسخه‌ی قدیمی در `AdminController.LoginPost` پس از ورود موفق یک رکورد `logHistory` می‌ساخت؛ مسیر Login جدید چنین رکوردی ایجاد نمی‌کند. این تفاوت روی جدول حساب یا رمز اثر ندارد و در این مستند برای شفافیت رفتار ثبت شده است. در صورت استفاده‌ی هم‌زمان دو برنامه، با Rehash خاموش، Login جدید مقدار ستون‌های `PharmacyAccount` را تغییر نمی‌دهد.

## Rehash اختیاری

تنظیم در Options استاندارد احراز هویت تعریف شده است:

```json
{
  "Authentication": {
    "EnablePasswordRehash": false
  }
}
```

هم `AuthenticationCoreOptions.EnablePasswordRehash` و هم مقدار صریح `Authentication:EnablePasswordRehash` در `appsettings.json` پیش‌فرض `false` دارند. تنظیم از Configuration معمول ASP.NET Core خوانده می‌شود و می‌توان آن را در محیط اجرا override کرد:

```powershell
$env:Authentication__EnablePasswordRehash = "true"
```

| تنظیم | نتیجه |
|---|---|
| `false` (پیش‌فرض) | هش Legacy و Identity فقط بررسی می‌شوند؛ نه هش و نه فیلد دیگری از حساب تغییر نمی‌کند. `PasswordVerificationResult.SuccessRehashNeeded` نیز با همان رمز صحیح به ورود موفق منجر می‌شود و UPDATE ایجاد نمی‌کند. |
| `true` | پس از اعتبارسنجی موفق رمز و فعال‌بودن حساب، اگر مقدار Legacy باشد یا Identity نتیجه‌ی `SuccessRehashNeeded` بدهد، هش جدید Identity تولید و در همان ستون موجود نوشته می‌شود. |

در حالت `true`، UPDATE فقط `PasswordHash` را هدف می‌گیرد و به شکل شرطی بر `Id` حساب و مقدار hash خوانده‌شده انجام می‌شود؛ اگر مقدار هم‌زمان توسط عامل دیگری تغییر کرده باشد، ورود fail-closed می‌شود. هیچ schema، ستون یا جدول جدیدی لازم نیست. بااین‌حال فعال‌کردن این گزینه در حالی که نسخه‌ی قدیمی همان جدول را مصرف می‌کند **توصیه نمی‌شود**: پس از تبدیل به Identity، برنامه‌ی قدیمی دیگر SHA-256 یا متن رمز خام را در آن ردیف پیدا نمی‌کند. برای همزیستی دو نسخه، گزینه را خاموش نگه دارید.

خاموش‌بودن Rehash فقط نوشتن خودکار را متوقف می‌کند؛ روش اعتبارسنجی، تبدیل ارقام، Encoding، salt یا الگوریتم‌های موجود را تغییر نمی‌دهد. هش SHA-256 و متن خام Legacy همچنان پذیرفته می‌شوند و قالب Identity همچنان با `PasswordHasher` فعلی بررسی می‌شود. همچنین این گزینه جایگزین مسیرهای مستقل تغییر رمز، بازیابی رمز یا ثبت‌نام نیست و هیچ‌یک از آن مسیرها را خودکار بازنویسی نمی‌کند.

### حساب‌هایی که قبلاً Rehash شده‌اند و بازیابی دسترسی Legacy

خاموش‌کردن Rehash مانع تبدیل‌های آینده می‌شود؛ مقدار حساب‌هایی که قبلاً در Loginهای موفق به قالب Identity تبدیل شده‌اند را به حالت قبل برنمی‌گرداند. برای این حساب‌ها:

1. اگر کاربر رمز فعلی را می‌داند، پروژه‌ی جدید می‌تواند hash Identity را بررسی کند و با Rehash خاموش وارد کند؛ برنامه‌ی قدیمی احتمالاً با همان رمز شکست می‌خورد، چون فقط SHA-256 یا متن خام را می‌شناسد.
2. هش قبلی از روی hash Identity قابل بازسازی نیست. هیچ کار خودکار، SQL تبدیل هش یا هش‌کردن دوباره‌ی hash ذخیره‌شده نمی‌تواند رمز اصلی را بازیابی کند.
3. برای شناسایی احتمالی، با دسترسی محدود و در محیط امن می‌توان ردیف‌های دارای پیشوند متداول Identity یعنی `AQAAAA` را بررسی کرد و در صورت دسترسی، با backup یا audit پیش از زمان مهاجرت مقایسه کرد. این پیشوند فقط یک قرینه‌ی قالب است، نه به‌تنهایی اثبات تاریخچه یا مجوز تغییر حساب. Hashها را در log، گزارش عمومی یا ticket قرار ندهید.
4. اگر لازم است همان حساب دوباره با پروژه‌ی قدیمی وارد شود، راهکار امن، reset رسمی رمز با احراز هویت کاربر و ثبت رمز جدید طبق قرارداد Legacy است: تبدیل ارقام و SHA-256 روی UTF-8 با Hex بزرگ. اگر جریان تغییر رمز موجود این قرارداد را تضمین نمی‌کند، آن را بدون بررسی دست‌کاری نکنید و reset را از مسیر عملیاتی مورد تأیید انجام دهید.

در تغییر فعلی هیچ داده‌ی کاربری، `PasswordHash` موجود یا ساختار دیتابیس خوانده/اصلاح/بازنشانی نشده است. بازیابی حساب‌های قبلاً تبدیل‌شده باید برای هر کاربر مجاز و جداگانه انجام شود؛ از بازگردانی انبوه backup به جدول زنده بدون بررسی تغییرات بعدی خودداری کنید.

### نقاط پیاده‌سازی و تنظیم

مسیرهای جدول نسبت به ریشه‌ی فضای کاری هستند.

| فایل/عضو | مسئولیت مرتبط با رمز |
|---|---|
| `darooyab_rewrite/Services/Authentication/Pharmacy/LegacyPharmacyCredentialCompatibility.cs` | تبدیل رقم‌های فارسی، نرمال‌سازی شماره و بررسی رمز Legacy به‌صورت SHA-256 UTF-8/Hex بزرگ یا متن خام |
| `darooyab_rewrite/Services/Authentication/Pharmacy/PharmacyAuthenticationProvider.cs`, `AuthenticateAsync` | خواندن بدون tracking حساب، انتخاب اعتبارسنجی Identity یا Legacy، کنترل وضعیت حساب و اجرای Rehash تنها در صورت مجاز بودن تنظیم |
| `darooyab_rewrite/Services/Authentication/AuthenticationCoreOptions.cs`, `EnablePasswordRehash` | تعریف گزینه‌ی typed با مقدار پیش‌فرض `false` |
| `darooyab_rewrite/appsettings.json`, `Authentication:EnablePasswordRehash` | مقدار صریح پیش‌فرض محیط برنامه (`false`) |
| `darooyab_rewrite.Tests/LegacyPharmacyCredentialCompatibilityTests.cs` | آزمون الگوریتم‌های Legacy/Identity، نتیجه‌ی نیازمند Rehash و مقدار پیش‌فرض تنظیم |
| `darooyab_build_expired/Controllers/AdminController.cs`, `LoginPost` و `darooyab_build_expired/Common/Extensions.cs`, `ToSHA256` | مرجع رفتار ورود و قالب Legacy؛ فقط برای مطالعه، پروژه‌ی قدیمی در این تغییر ویرایش نشده است |

## Claims و Authorization

Cookie پس از ورود شامل این Claims است:

| Claim | مقدار |
|---|---|
| `ClaimTypes.NameIdentifier` | شناسه‌ی ترکیبی `{UserType}:{UserId}` برای جلوگیری از تداخل شناسه‌ها |
| `darooyab:user_id` | شناسه‌ی منبعی کاربر |
| `darooyab:user_type` | نوع کاربر؛ برای Provider حاضر `Pharmacy` |
| `ClaimTypes.Name` | نام استاندارد کاربر؛ در این Provider شماره‌ی همراه |
| `darooyab:authentication_provider` | نام Provider؛ در این Provider `Pharmacy` |
| `ClaimTypes.Role` | فقط Roleهای ارائه‌شده توسط Provider، در صورت وجود |

صرف وجود `UserType=Pharmacy`، مجوز عمومی برای دسترسی به همه‌ی عملیات یا داده‌ها نیست. Policy فعلی `PharmacyAccount` احراز هویت و نوع کاربر Pharmacy را الزام می‌کند؛ برای مجوزهای دقیق‌تر باید Policy یا Role مناسب هر قابلیت تعریف شود.

نمونه‌ی مصرف هویت جاری در یک Controller:

```csharp
using darooyab_rewrite.Services.Authentication;
using darooyab_rewrite.Services.Authentication.Pharmacy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Policy = PharmacyAuthenticationDefaults.AuthorizationPolicy)]
public sealed class PharmacyPanelController(
    IAuthenticationService authentication) : Controller
{
    public IActionResult Index()
    {
        var user = authentication.GetCurrentUser();
        if (user is null)
        {
            return Challenge();
        }

        ViewData["UserId"] = user.UserId;
        ViewData["UserType"] = user.UserType;
        ViewData["Provider"] = user.AuthenticationProvider;
        return View();
    }
}
```

خروج از UI باید فرم POST داشته باشد تا Tag Helper، Anti-Forgery Token را اضافه کند:

```html
<form asp-controller="PharmacyAuthentication" asp-action="Logout" method="post">
    <button type="submit">خروج</button>
</form>
```

## Cookie و Configuration

تنظیمات پیش‌فرض در `appsettings.json` هستند:

```json
{
  "Authentication": {
    "CookieName": "Darooyab.Authentication",
    "ExpireTimeSpan": "00:45:00",
    "SlidingExpiration": true,
    "EnablePasswordRehash": false,
    "LoginPath": "/ph/login",
    "LogoutPath": "/ph/logout",
    "MaxLoginAttemptsPerIp": 5,
    "LoginRateLimitWindow": "00:10:00"
  }
}
```

`CookieName`، `ExpireTimeSpan`، `SlidingExpiration`، `LoginPath` و `LogoutPath` به Cookie middleware داده می‌شوند. `EnablePasswordRehash` فقط در `PharmacyAuthenticationProvider` برای کنترل نوشتن هش کاربرد دارد؛ مقدار آن به‌صورت پیش‌فرض false است و در تولیدِ دارای دیتابیس مشترک با پروژه‌ی قدیمی باید خاموش بماند. مسیرها باید محلی باشند؛ Options در آغاز برنامه اعتبارسنجی می‌شوند. عمر مجاز Cookie بین ۵ دقیقه و ۷ روز است. نمونه‌های override در محیط اجرا: `Authentication__ExpireTimeSpan=01:00:00` و `Authentication__EnablePasswordRehash=false`.

Cookie با نام تنظیم‌شده، `HttpOnly` و `SameSite=Lax` است؛ در محیط غیر Development، `Secure` اجباری است. Cookie به‌صورت persistent صادر نمی‌شود و Logout آن را از scheme استاندارد خارج می‌کند. در استقرار چندنمونه‌ای، key ring مربوط به Data Protection باید بین نمونه‌ها مشترک، پایدار و محافظت‌شده باشد.

در `Program.cs` سرویس‌ها و route مربوط به Area به این شکل ثبت شده‌اند:

```csharp
builder.Services.AddAuthenticationCore(builder.Configuration, builder.Environment);
builder.Services.AddPharmacyAuthenticationProvider();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

`AddAuthenticationCore`، Cookie، Options، `IAuthenticationService` و Authorization پایه را ثبت می‌کند. `AddPharmacyAuthenticationProvider`، Provider داروخانه، `IPasswordHasher<PharmacyAccount>` و Policy داروخانه را اضافه می‌کند.

Area فعلی در `Areas/Admin` است، اما Policy آن `PharmacyAccount` است؛ بنابراین فقط حساب داروخانه‌ی احراز‌شده به این داشبورد دسترسی دارد. این به معنی وجود Provider یا مجوز مدیر سامانه نیست. عنوان Admin صرفاً نام Area/رابط مدیریتی است.

محدودکننده‌ی `pharmacy-login` در حال حاضر per-process و IP-based است؛ با تنظیم پیش‌فرض حداکثر ۵ درخواست در ۱۰ دقیقه پذیرفته می‌شود. CAPTCHA موجود نیز در سمت سرور و در همان POST ورود اعتبارسنجی می‌شود. این limiter به‌تنهایی برای چند نمونه‌ی برنامه کافی نیست؛ برای استقرار چندسروری از محدودکننده‌ی توزیع‌شده در gateway یا زیرساخت اجرا استفاده کنید. در پشت reverse proxy نیز فقط پس از پیکربندی proxyهای مورداعتماد، Forwarded Headers را برای تشخیص IP به‌کار ببرید.

## افزودن Provider

Provider آینده باید قرارداد عمومی را پیاده کند، credential را در منبع خودش اعتبارسنجی کند و فقط مدل عمومی `AuthenticatedUser` را برگرداند. انتخاب Provider بر اساس `CanHandle` و به ترتیب ثبت انجام می‌شود؛ شناسه‌های Providerها نباید هم‌پوشانی غیرعمدی داشته باشند.

```csharp
using darooyab_rewrite.Services.Authentication;

public sealed class FutureProvider : IUserAuthenticationProvider
{
    public string Name => "Future";

    public bool CanHandle(string userName) =>
        userName.StartsWith("future:", StringComparison.OrdinalIgnoreCase);

    public Task<AuthenticatedUser?> AuthenticateAsync(
        AuthenticationCredentials credentials,
        CancellationToken cancellationToken)
    {
        // اعتبارسنجی در منبع اختصاصی Provider؛ credential را log نکنید.
        AuthenticatedUser? user = null;
        return Task.FromResult(user);
    }
}
```

ثبت آن در composition root:

```csharp
builder.Services.AddScoped<IUserAuthenticationProvider, FutureProvider>();
```

برای معرفی کاربر با نوع جدید، فقط همان Provider مقدار `UserType` مناسب را برمی‌گرداند. هسته تغییری نمی‌کند؛ اما Policy دسترسی به منابع را باید جداگانه و صریح تعریف کرد.

## آزمون‌ها و محدودیت‌های اعتبارسنجی

آزمون‌های `AuthenticationServiceTests` و `LegacyPharmacyCredentialCompatibilityTests` انتخاب Provider، Claims، نرمال‌سازی شماره، رمز SHA-256 با خروجی uppercase، رمز خام Legacy، تولید/اعتبارسنجی hash Identity، نتیجه‌ی `SuccessRehashNeeded` و پیش‌فرض خاموش‌بودن Rehash را پوشش می‌دهند. این‌ها آزمون‌های الگوریتم/Options هستند و **آزمون integration با ردیف واقعی `PharmacyAccount` نیستند**؛ بنابراین عدم UPDATE در دیتابیس واقعی، وضعیت غیرفعال با DB، و ورود پروژه‌ی قدیمی به حساب واقعی در این محیط مستقلاً اجرا نشده‌اند. برای تأیید عملیاتی، روی دیتابیس آزمون مجاز و حساب آزمایشی، hash قبل و بعد را بدون نمایش مقدار آن مقایسه کنید؛ ورود موفق/ناموفق و حساب غیرفعال را بررسی و سپس همان حساب Legacy را با پروژه‌ی قدیمی آزمایش کنید. از حساب یا رمز کاربران واقعی برای تست استفاده نکنید.

آخرین اجرای ثبت‌شده برای تغییر تنظیم Rehash: `dotnet build .\darooyab_rewrite\darooyab_rewrite.csproj` موفق با صفر هشدار/خطا و `dotnet test .\darooyab_rewrite.Tests\darooyab_rewrite.Tests.csproj` موفق با ۴۱ تست. Build و آزمون‌های پروژه با این فرمان‌ها اجرا می‌شوند:

```powershell
dotnet build .\darooyab_rewrite\darooyab_rewrite.csproj
dotnet test .\darooyab_rewrite.Tests\darooyab_rewrite.Tests.csproj
```

ورود واقعی و وضعیت/انقضای حساب به اتصال مجاز به پایگاه‌داده‌ی آزمون یا عملیاتی نیاز دارد و در این محیط بررسی نشده است.
