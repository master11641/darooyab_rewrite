# داشبورد مستقل مدیریت داروخانه

این سند معماری، دسترسی، رفتار فعلی و روش گسترش داشبورد در پروژه‌ی `darooyab_rewrite` را شرح می‌دهد. Area فعلی با وجود نام فنی `Admin`، داشبورد مدیر داروخانه است؛ این صفحه پنل مدیر سامانه یا ابزار مدیریت سراسری نیست.

## وضعیت و محدوده

- داشبورد داروخانه در مسیر `/PharmacyAdmin/Dashboard` قرار دارد و تنها کاربر احراز هویت‌شده با Policy `PharmacyAccount` اجازه‌ی مشاهده‌ی آن را دارد. مسیر `/Admin` برای این داشبورد استفاده نمی‌شود تا از پنل‌های مدیریتی دیگر تفکیک شود.
- ورود مدیر داروخانه همچنان از `/ph/login` انجام می‌شود. در صورت نبودن `returnUrl` محلی، پس از ورود موفق به داشبورد منتقل می‌شود. Logout فقط با POST و Anti-Forgery انجام می‌شود.
- نام داروخانه از رابطه‌ی واقعی `PharmacyAccount` و `Pharmacy` خوانده می‌شود و در کارت خوشامدگویی و سربرگ نمایش داده می‌شود. شناسه‌ی حساب از Claim کاربر جاری به دست می‌آید؛ شناسه از فرم یا URL دریافت نمی‌شود.
- حساب در زمان بارگذاری داشبورد دوباره از نظر `AccountStatus == "2"`، پایان‌نیافتن `AccountExpireDate` و `DarooyabAdminApproved != false` بررسی می‌شود. اگر معتبر نباشد، Cookie خارج و درخواست دوباره به احراز هویت فرستاده می‌شود.
- چهار کارت خلاصه، آمار زنده‌ی ارجاع بیمار (معتبر/منقضی)، رتبه‌های خریداری‌شده (کل/فعال/مبلغ)، نسخه‌ها (کل/بررسی‌شده/بررسی‌نشده) و مانده‌ی کیف پول را از جدول‌ها و سرویس‌های همان پنل می‌خوانند. هیچ عدد ثابتی در آن‌ها نیست. نمودار و جریان فعالیت زمانی هنوز به منبع داده وصل نشده‌اند.
- دکمه‌ی اعلان غیرفعال است، چون قابلیت اعلان پیاده نشده است. سایدبار فعلی علاوه بر داشبورد، مسیرهای ثبت موجودی/ارجاع، ثبت کسری، مازاد برای تبادل، کیف پول، تاریخچه‌ی رتبه‌های خریداری‌شده و تازه‌های شرکت پخش را دارد؛ سایر پیوندهای قدیمی تا زمان پیاده‌سازی غیرفعال‌اند. شرح وضعیت منوها در [مستندات قابلیت‌های داروخانه](./PHARMACY-REQUESTS.md) و جدول‌های خوانده‌شده/ستون‌های تغییرکرده در [راهنمای منوها و اثر داده‌ای](./ADMIN-MENU-DATA.md) آمده است. هنگام کامل‌شدن قابلیت هر منوی Admin، همین راهنما باید در همان تغییر به‌روز شود.
- این Area فعلاً فقط حساب‌های Pharmacy را مجاز می‌کند. صرف نام Area یا Claim نوع کاربر مجوز مدیر سراسری ایجاد نمی‌کند و هیچ Provider/Role مدیر سامانه در این تغییر افزوده نشده است.

## معماری و فایل‌ها

```text
Areas/Admin/
  Controllers/DashboardController.cs
  Controllers/PharmacyAccountController.cs
  Controllers/PharmacyRequestController.cs
  ViewModels/AdminDashboardViewModel.cs
  ViewModels/PharmacyDrugDistributionViewModels.cs
  ViewModels/PharmacyRequestViewModels.cs
  Views/
    _ViewImports.cshtml
    _ViewStart.cshtml
    Dashboard/Index.cshtml
    PharmacyAccount/DrugDistribution.cshtml
    Wallet/Rankings.cshtml
    PharmacyRequest/AddOrEdit.cshtml
    PharmacyRequest/TabadolManage.cshtml
    PharmacyRequest/_TabadolManageList.cshtml
    Shared/
      _Layout.cshtml
      _Sidebar.cshtml
      _Footer.cshtml
wwwroot/
  css/admin-panel.css
  js/admin-panel.js
  js/pharmacy-requests.js
Services/PharmacyRequests/
  IPharmacyRequestService.cs
  PharmacyRequestService.cs
Services/PharmacyDrugDistributions/
  IPharmacyDrugDistributionService.cs
  PharmacyDrugDistributionService.cs
```

| فایل | مسئولیت |
|---|---|
| `Areas/Admin/Controllers/DashboardController.cs` | Route اختصاصی، اعمال Policy، اعتبارسنجی مجدد حساب و queryهای آمار داشبورد با EF Core |
| `Areas/Admin/ViewModels/AdminDashboardViewModel.cs` | نام داروخانه/کاربر و اعداد خلاصه‌ی بدون Entity |
| `Areas/Admin/Views/Dashboard/Index.cshtml` | چهار کارت آمار، دسترسی‌های سریع و اطلاعات حساب |
| `Areas/Admin/Views/Shared/_Layout.cshtml` | سند HTML مستقل، noindex، سربرگ، محتوای اصلی، Sidebar و Footer |
| `Areas/Admin/Views/Shared/_Sidebar.cshtml` | برند و ساختار توسعه‌پذیر ناوبری |
| `Areas/Admin/Controllers/WalletController.cs` و `Services/PharmacyRequests/PharmacyWalletService.cs` | کیف پول و فهرست فقط‌خواندنی رتبه‌های خریداری‌شده با قید حساب جاری |
| `Areas/Admin/Views/Wallet/*` | تاریخچه‌ی کیف پول و رتبه‌های خریداری‌شده |
| `Areas/Admin/Controllers/PharmacyAccountController.cs` و `Services/PharmacyDrugDistributions/*` | فهرست تازه‌های شرکت پخش با جستجوی نام برند، فیلتر شرکت و صفحه‌بندی |
| `Areas/Admin/Views/PharmacyAccount/DrugDistribution.cshtml` | فرم GET فیلترها، فهرست و Empty State تازه‌های شرکت پخش |
| `Areas/Admin/Controllers/PharmacyRequestController.cs` و `Services/PharmacyRequests/*` | ثبت/مدیریت ارجاع دارویی و مازاد تبادل با Policy و مالکیت حساب |
| `Areas/Admin/Views/PharmacyRequest/*` و `Areas/Admin/ViewModels/PharmacyRequestViewModels.cs` | فرم‌ها، فهرست‌ها، Partialهای AJAX و مدل‌های صفحه |
| `Areas/Admin/Views/Shared/_Footer.cshtml` | پابرگ و لینک بازگشت به سایت عمومی |
| `wwwroot/css/admin-panel.css` | متغیرهای طراحی و همه‌ی استایل‌های namespaced داشبورد |
| `wwwroot/js/admin-panel.js` | جمع/باز کردن Sidebar، Drawer موبایل و بستن با Escape |

`Areas/Admin/Views/_ViewStart.cshtml` Layout همان Area را انتخاب می‌کند؛ صفحات عمومی و Layout پنل قدیمی داروخانه از این Layout استفاده نمی‌کنند. Controller داشبورد Route صریح `/PharmacyAdmin/Dashboard` دارد؛ نام داخلی Area برای سازمان‌دهی Viewها و Authorization باقی مانده و آدرس داشبورد را تعیین نمی‌کند. Route عمومی Area با الگوی `{area:exists}/{controller=Dashboard}/{action=Index}/{id?}` همچنان برای سایر Controllerهای فاقد Route صریح در دسترس است. Controllerهای دارای مسیرهای قدیمی مانند `/PharmacyRequest/AddOrEdit` و `/PharmacyRequest/TabadolManage` مسیرهای صریح خود را حفظ می‌کنند. فایل‌های CSS/JS نیز فقط Layout این Area آن‌ها را بارگذاری می‌کند.

## دسترسی و هویت

`DashboardController` از Policy ثبت‌شده‌ی `PharmacyAccount` استفاده می‌کند. Policy نیازمند کاربر احراز هویت‌شده و Claim نوع `Pharmacy` است. سپس اکشن شناسه‌ی Claim را به عدد تبدیل و همان ردیف حساب را از دیتابیس دوباره واکشی می‌کند. نام داروخانه، نام کاربری و مقادیر شمارشیِ آمار به ViewModel منتقل می‌شوند؛ Entity، رمز و سایر داده‌های حساب به View داده نمی‌شوند.

## تعریف آمار داشبورد

- **موجودی ارجاع:** تعداد `PharmacyRequest`های حساب جاری با دسته‌ی `patient_referral` و تاریخ پایان غیرتهی؛ پایان امروز یا آینده معتبر و تاریخ گذشته منقضی است. این فیلتر با فهرست موجودی ارجاع هم‌خوان است و `StatusNum` فیلتر اضافه‌ای نیست.
- **رتبه‌ها:** تعداد و مجموع مبلغ تمام `RankingTransaction`های حساب جاری مطابق تاریخچه‌ی خرید؛ تعداد فعال با بازه‌ی `StartDate <= اکنون <= EndDate` تعیین می‌شود. `Amount` برای نمایش از ریال به تومان تبدیل می‌شود.
- **نسخه‌ها:** شمارش بر اساس `DarooyabPrescription.PharmacyId` متناظر با حساب. نسخه‌ی `IsReviewed == true` بررسی‌شده و مقدار `false/null` بررسی‌نشده است؛ مجموع دو دسته با کل نسخه‌ها برابر است.
- **کیف پول:** از `IPharmacyWalletService` گرفته می‌شود تا تعریف مانده با صفحه‌ی کیف پول یکسان بماند؛ مانده‌ی منفی برای نمایش صفر در نظر گرفته می‌شود.
- Queryها فقط‌خواندنی‌اند و به Claim حساب جاری محدود می‌شوند. فعالیت‌های زمانی، اعلان‌ها و نمودارها تا اتصال به منبع واقعی همچنان نمایش موفقیت‌نما ندارند.

برای صفحات بعدی، `[Area("Admin")]` و Policy باید روی هر Controller/اکشن محافظت‌شده اعمال شود. مخفی کردن لینک منو جایگزین بررسی Authorization سمت سرور نیست. اگر بعداً نقش‌های متفاوت داروخانه یا مدیر سامانه نیاز شد، Policyهای مجزا و Provider/Claims متناظر را تعریف کنید؛ به این Area صرفاً بر اساس نام آن دسترسی ندهید.

## رابط کاربری و Design System

همه‌ی selectorهای رابط در فضای `.admin-panel` قرار دارند و متغیرهای CSS در همان ریشه تعریف شده‌اند. فونت محلی Vazirmatn استفاده می‌شود؛ Bootstrap یا وابستگی UI جدید به Layout مدیریتی اضافه نشده است.

- سایدبار در دسکتاپ قابل جمع‌شدن است؛ در تبلت/موبایل به Drawer تبدیل می‌شود.
- تغییرات Responsive در breakpointهای 1100، 760 و 440 پیکسل اعمال می‌شوند؛ حداقل عرض هدف 320 پیکسل است.
- دکمه‌ی منو دارای `aria-expanded` و `aria-controls` است؛ لینک جاری `aria-current` دارد؛ SVGهای تزئینی از صفحه‌خوان مخفی‌اند و دکمه‌ی بدون قابلیت اعلان دارای نام/توضیح است.
- Escape منوی موبایل را می‌بندد، focus به دکمه‌ی بازکننده بازمی‌گردد و focus-visible برای کنترل‌ها تعریف شده است.
- حرکت‌ها به `prefers-reduced-motion` احترام می‌گذارند.
- داشبورد به‌صورت پیش‌فرض با تم روشن نمایش داده می‌شود. دکمه‌ی هدر بین تم روشن و تیره جابه‌جا می‌شود و انتخاب کاربر در مرورگر برای بازدیدهای بعدی ذخیره می‌شود؛ تم به تنظیمات ظاهری سیستم وابسته نیست.
- `meta name="robots"` با `noindex, nofollow, noarchive` از ایندکس‌شدن صفحه‌ی خصوصی جلوگیری می‌کند. برای درخواست‌های Dashboard نیز Response Cache غیرفعال است.
- خود داشبورد عملیات Ajax یا ذخیره‌ی فرم ندارد؛ آمار در بارگذاری صفحه از منبع واقعی خوانده می‌شود و دسترسی‌های سریع به منوهای مربوط می‌دهد. صفحه‌ی ثبت موجودی شامل جستجوی برند، دریافت Partial رتبه/قیمت پس از انتخاب یک برند، محاسبه‌ی مبلغ رتبه، loading/error/empty state، صفحه‌بندی و تأیید حذف است؛ هنوز کتابخانه‌ی کامپوننت عمومی و قابل استفاده‌ی مجدد برای DataTable، Modal، Alert یا FormControl ساخته نشده است.
- `/Wallet/Rankings` فقط فهرست خریدهای ثبت‌شده را نشان می‌دهد؛ مطابق صفحه‌ی Legacy، این View هیچ jQuery/AJAX یا Partial ندارد و خرید مجدد در آن غیرفعال است. صفحه‌بندی با GET و ۵ ردیف در هر صفحه انجام می‌شود.
- `/PharmacyAccount/DrugDistribution` فهرست read-only است؛ صفحه‌ی Legacy فقط فرم GET و صفحه‌بندی داشت و هیچ jQuery/AJAX یا Partial در این مسیر وجود ندارد. فیلترها هنگام صفحه‌بندی حفظ می‌شوند و توضیحات ذخیره‌شده پیش از نمایش sanitize می‌شوند.
- مسیرهای `/PharmacyRequest/AddOrEdit` و `/PharmacyRequest/TabadolManage` در همان پوسته‌ی مستقل Admin و با Policy حساب Pharmacy اجرا می‌شوند. هر دو از مالکیت Claim، Anti-Forgery و Entityهای موجود استفاده می‌کنند؛ TabadolManage ثبت/ویرایش مازاد، فهرست AJAX، جستجو/صفحه‌بندی، حذف و خاتمه را پوشش می‌دهد. اجرای واقعی یک معامله در مسیر قدیمی جداگانه‌ی ExchangeDrugController قرار داشت و در این View منتقل نشده است. جزئیات تفکیک و موارد باقی‌مانده در [PHARMACY-REQUESTS.md](./PHARMACY-REQUESTS.md) آمده است. Header مشترک تمام صفحات پنل، مانده‌ی قابل استفاده‌ی کیف پول را از `IPharmacyWalletService` می‌خواند و به صفحه‌ی ثبت موجودی پیوند می‌دهد.

### اجزای فعلی در برابر اجزای آینده

در نسخه‌ی فعلی، Sidebar، Header، Footer، کارت‌های آمار، دسترسی‌های سریع، Breadcrumb ساده و حالت‌های خالی پنل پیاده شده‌اند. SearchBox، Pagination، Alert، FormControl، Loading/Error state و تأیید حذف فقط داخل صفحه‌ی ثبت موجودی وجود دارند و کامپوننت عمومی مستقل نیستند. DataTable، Modal عمومی، مرکز اعلان‌ها و سیستم مدیریت منو وجود ندارند؛ در صورت نیاز واقعی، آن‌ها را در همین Area و با حفظ namespace طراحی کنید. از ساخت abstraction پیش از نیاز واقعی پرهیز شود.

## افزودن صفحه یا منو

1. Controller را زیر `Areas/Admin/Controllers` قرار دهید، `[Area("Admin")]` و Policy مناسب را اعمال کنید و داده را به ViewModel اختصاصی نگاشت کنید.
2. View را زیر `Areas/Admin/Views/<Controller>` ایجاد کنید؛ `_ViewStart.cshtml` همان Layout اختصاصی را اعمال می‌کند.
3. منوی فعال را با route جاری تعیین کنید و فقط لینک‌هایی را نمایش دهید که مقصد پیاده‌شده دارند. اگر منوها بر اساس Policy پنهان می‌شوند، Authorization سمت سرور را همچنان روی اکشن‌ها اعمال کنید.
4. برای رفتار تعاملی، کد محدود به `wwwroot/js/admin-panel.js` و کلاس‌ها/متغیرهای `admin-panel.css` اضافه کنید. از selector عمومی، inline style/script و وابستگی جدید غیرضروری پرهیز کنید.
5. اگر داده‌ی واقعی هنوز موجود نیست، Empty/Unavailable state نشان دهید؛ placeholder را به عدد یا وضعیت موفقیت‌نما تبدیل نکنید.

برای ترتیب بررسی Legacy، کنترل مالکیت و ورودی‌ها، Anti-Forgery، ترتیب queryها، تست با دیتابیس مجاز و چک UI در breakpointها، از [راهنمای توسعه و اعتبارسنجی پنل داروخانه](./ADMIN-PHARMACY-DEVELOPMENT.md) استفاده کنید. جزئیات اثر هر منو روی جدول و ستون‌ها در [ADMIN-MENU-DATA](./ADMIN-MENU-DATA.md) نگهداری می‌شود.

## اجرا و بررسی

از ریشه‌ی Workspace:

```powershell
dotnet build .\darooyab_rewrite\darooyab_rewrite.csproj
dotnet test .\darooyab_rewrite.Tests\darooyab_rewrite.Tests.csproj
dotnet run --project .\darooyab_rewrite\darooyab_rewrite.csproj
```

مسیر `/PharmacyAdmin/Dashboard` را بدون Cookie بررسی کنید: باید به `/ph/login` با `returnUrl` محلی هدایت شود؛ مسیرهای `/Admin` و `/Admin/Dashboard` نباید داشبورد داروخانه را نمایش دهند. برای مشاهده‌ی نام داروخانه باید از مسیر ورود با حساب مجاز و اتصال دیتابیس معتبر استفاده شود. در آزمون رابط، عرض‌های 320، 390، 768، 1024 و 1440 پیکسل، نبود اسکرول افقی، باز/بسته‌شدن Drawer، Escape، focus و حالت Reduced Motion را بررسی کنید. دسترسی مستقیم به route بدون ورود، بررسی مجدد انقضای حساب و خروج باید جداگانه تست شوند.

در این محیط credential مجاز داروخانه برای ورود فراهم نبود؛ بنابراین مسیر UI و حفاظت route قابل smoke-test است، اما آزمون نهایی رندر اعداد با داده‌ی زنده و انطباق Aggregateهای EF با دیتابیس استقرار به اتصال مجاز توسعه/آزمون نیاز دارد.
