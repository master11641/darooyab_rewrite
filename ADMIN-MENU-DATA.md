# راهنمای منوهای پنل مدیریت داروخانه و اثر آن‌ها بر داده

این سند مرجع زنده‌ی رفتار منوهای پنل داروخانه در `darooyab_rewrite` است. برای هر منو، مسیر، کارکرد، Entity/جدول‌های خوانده‌شده و تغییراتی که عملیات ثبت، ویرایش، حذف یا خاتمه روی داده ایجاد می‌کنند ثبت می‌شود.

قواعد مشترک بررسی Legacy، اعتبارسنجی ورودی و مالکیت، تست‌های خودکار/دستی و محدودیت ادعای صحت زنده در [راهنمای توسعه و اعتبارسنجی پنل داروخانه](./ADMIN-PHARMACY-DEVELOPMENT.md) آمده است.

## قواعد نگهداری سند

- هر زمان منوی پنل فعال یا قابلیت آن کامل شد، همین فایل را در همان تغییر نرم‌افزاری به‌روز کنید.
- مسیر و نام منو را از [سایدبار پنل](./Areas/Admin/Views/Shared/_Sidebar.cshtml) تطبیق دهید و منطق واقعی را از Controller، Service و Entity/Mapping دنبال کنید؛ صرف View یا متن دکمه منبع کافی نیست.
- برای هر عملیات صریحاً بگویید چه رکوردهایی فقط خوانده می‌شوند و کدام ستون‌ها درج، به‌روزرسانی یا حذف می‌شوند. اگر در یک تراکنش چند جدول تغییر می‌کنند، همه را ذکر کنید.
- منو/عملیات پیاده‌سازی‌نشده را فعال یا قابل استفاده معرفی نکنید. داده‌ی محاسباتی، فیلد خواندنی، جدول lookup و تغییر واقعی داده را از هم جدا کنید.
- تغییرات Schema/Migration، رفتار Legacy که در نسخه‌ی جدید منتقل نشده و محدودیت‌های اتصال/اعتبارسنجی زنده را جداگانه توضیح دهید.
- این سند وضعیت فعلی منوهای پنل داروخانه است، نه فهرست پنل مدیر سراسری. Area فنی `Admin` در این پروژه مجوز مدیر سراسری ایجاد نمی‌کند.

## مبنای دسترسی و انتساب مالکیت

Controllerهای پنل با Policy `PharmacyAccount` محافظت شده‌اند. شناسه‌ی حساب مالک از Claim هویت احراز‌شده به دست می‌آید؛ شناسه‌ی ارسال‌شده از مرورگر مجوز دسترسی ایجاد نمی‌کند. عملیات تغییردهنده POST و Anti-Forgery دارند و سرویس‌ها رکورد را با `PharmacyAccountIdFk` مالک جاری محدود می‌کنند. هیچ‌یک از قابلیت‌های زیر به Schema جدید نیاز ندارد.

## خلاصه‌ی وضعیت منوها

| عنوان نمایشی | مسیر/عملیات | وضعیت |
|---|---|---|
| داشبورد داروخانه | `/PharmacyAdmin/Dashboard` | فعال؛ آمار موجودی ارجاع، خرید رتبه، نسخه‌ها و مانده کیف پول به‌صورت فقط‌خواندنی |
| ثبت موجودی داروخانه (کمبودهای دارویی) | `/PharmacyRequest/AddOrEdit` | فعال؛ اعلام موجودی برای ارجاع بیمار، خرید رتبه و به‌روزرسانی آخرین قیمت برند |
| ثبت کسری داروخانه | `/DrugShortage/AddOrEdit` | فعال؛ ثبت نیاز دارویی و مشاهده‌ی مازادهای منطبق |
| مازاد بر نیاز جهت تبادل | `/PharmacyRequest/TabadolManage` | فعال؛ ایجاد/ویرایش مازاد پایه، ویرایش انقضا، حذف یا خاتمه |
| کیف پول | `/Wallet` | فعال؛ مانده، جمع شارژ/هزینه، تاریخچه‌ی صفحه‌بندی‌شده و درگاه شارژِ مشروط به پیکربندی |
| رتبه‌های خریداری‌شده | `/Wallet/Rankings` | فعال؛ تاریخچه‌ی فقط‌خواندنی همان حساب، صفحه‌بندی ۵تایی طبق Legacy |
| اقلام تاریخ نزدیک سایرین | `/DrugShortage/otherPhNearDateDrugShortageList` | فعال؛ فهرست فقط‌خواندنی، فیلتر استان/شهر و صفحه‌بندی |
| کمک به تکمیل برندها | `/Brand/SubmitBrandNameByPharmacy` | فعال؛ ثبت پیشنهاد در `TBL_Contactus` برای بررسی مدیر، بدون ایجاد خودکار برند |
| تازه‌های شرکت پخش | `/PharmacyAccount/DrugDistribution` | فعال؛ فهرست فقط‌خواندنی، جستجوی برند، فیلتر شرکت پخش و صفحه‌بندی ۵تایی |
| مشاهده نسخه | `/PharmacyRequest/DarooyabPrescription` | مسیر AJAX و عملیات بررسی/پیش‌نمایش SMS پیاده‌سازی و لینک Sidebar فعال است؛ اعتبارسنجی زنده همچنان نیازمند حساب/کلید آزمون است |
| مدیریت اطلاعات داروخانه | `/DarooyabPharmacy/PharmacySpecialties` | فعال؛ شش بخش منطقه، گروه دارویی، موقعیت، تصاویر، تماس و بیمه |
| مازاد تبادل‌شده، تغییر کلمه عبور | بدون مسیر عملیاتی در سایدبار فعلی | غیرفعال/منتقل‌نشده؛ این برچسب‌ها قابلیت CRUD قابل استفاده ایجاد نمی‌کنند |
| ثبت کسری داروخانه در گروه «سایر امکانات قدیمی» | بدون مسیر (Placeholder) | نسخه‌ی تکراریِ غیرفعال؛ منوی فعال همان لینک بالاتر به `/DrugShortage/AddOrEdit` است |
| وب‌سایت دارویاب، آموزش تصویری | لینک بیرونی/عمومی | Navigation است؛ تغییری در داده‌های پایگاه‌داده ندارد |

---

## ۱. داشبورد

**مسیر و پیاده‌سازی:** `GET /PharmacyAdmin/Dashboard`، [DashboardController](./Areas/Admin/Controllers/DashboardController.cs)، [View داشبورد](./Areas/Admin/Views/Dashboard/Index.cshtml). این URL مختص داشبورد داروخانه است؛ نام فنی Area و محل Viewها در کد همچنان `Admin` مانده و به معنی رزرو `/Admin` برای داروخانه نیست.

**کارکرد:** نمایش نام داروخانه و کاربر، تعداد موجودی‌های ارجاع معتبر/منقضی، تعداد و مبلغ رتبه‌های خریداری‌شده، رتبه‌های فعال، آمار نسخه‌های بررسی‌شده/بررسی‌نشده/کل و مانده‌ی قابل استفاده‌ی کیف پول. همه‌ی آمار به‌صورت فقط‌خواندنی و برای حساب جاری محاسبه می‌شوند؛ نمودارها و جریان فعالیت زمانی هنوز پیاده‌سازی نشده‌اند.

**منابع و تعریف شمارش‌ها:**

- `PharmacyAccount` و `Pharmacy`: حساب جاری بر اساس Claim، وضعیت `AccountStatus == "2"`، `DarooyabAdminApproved != false` و معتبر بودن `AccountExpireDate` تأیید می‌شود؛ `Pharmacy.Name` و شناسه‌ی Pharmacy برای آمار نسخه خوانده می‌شوند.
- `PharmacyRequest` و `Category`: درخواست‌های حساب جاری با دسته‌ی `patient_referral` و `RequestExpireDate` غیرتهی شمرده می‌شوند. پایان از امروز به بعد معتبر است و پیش از امروز منقضی؛ مانند فهرست `/PharmacyRequest/AddOrEdit`، `StatusNum` فیلتر اضافه‌ای نیست.
- `RankingTransaction`: همه‌ی رکوردهای حساب جاری، بدون حذف تاریخچه، برای تعداد خرید و مجموع `Amount` (تبدیل ریال به تومان) استفاده می‌شوند. رتبه‌ی فعال مطابق منطق خرید وقتی `StartDate <= اکنون <= EndDate` است.
- `DarooyabPrescription`: تمام نسخه‌های Pharmacy جاری شمرده می‌شوند؛ `IsReviewed == true` بررسی‌شده و `IsReviewed != true` بررسی‌نشده است، مطابق فیلتر صفحه‌ی نسخه‌ها.
- مانده با `IPharmacyWalletService` محاسبه می‌شود، همان فرمول صفحه‌ی کیف پول شامل شارژهای تأییدشده، هزینه‌ی رتبه و پیامک موفق؛ مبلغ قابل نمایش به تومان تبدیل می‌شود.
- `UserName` فقط از هویت جاری خوانده می‌شود. هیچ داده‌ای در این صفحه درج، ویرایش یا حذف نمی‌شود و Schema/Migration جدیدی وجود ندارد.

## نسخه‌های ارسالی (جدید)

**مسیر و پیاده‌سازی:** `GET /PharmacyRequest/DarooyabPrescription`، AJAX فهرست `GET /PharmacyRequest/SearchPrescriptions`، تغییر وضعیت بررسی `POST /PharmacyRequest/UpdateIsReviewed`، پیش‌نمایش `GET /PharmacyRequest/SendSMS` و ارسال `POST /PharmacyRequest/SendSMSPost`. پیاده‌سازی در [PharmacyRequestController](./Areas/Admin/Controllers/PharmacyRequestController.cs) و [PharmacyPrescriptionService](./Services/PharmacyPrescriptions/PharmacyPrescriptionService.cs) است.

**رفتار و مالکیت:** فهرست نسخه‌های `PharmacyId` داروخانه‌ی جاری را با جستجوی `TrackingCode.Contains` و فیلتر `IsReviewed` نشان می‌دهد؛ مقدار پیش‌فرض «بررسی‌نشده»، اندازه صفحه ۱۰ و ترتیب `Id DESC` است. JSON شامل نام دارو با IRC، تاریخ شمسی، منبع وب/اپلیکیشن، وضعیت پیامک موفق و تصویر نسخه است. عملیات بررسی، پیش‌نمایش و ارسال مجدداً مالکیت همان Pharmacy را کنترل می‌کنند؛ هیچ شناسه‌ی مالک از مرورگر پذیرفته نمی‌شود. View قدیمی برای لیست/تغییر وضعیت Partial نداشت؛ صفحه‌ی پیش‌نمایش SMS یک View جداگانه است.

**اثر داده:** تغییر بررسی فقط `DarooyabPrescription.IsReviewed` را به‌روزرسانی می‌کند. ارسال ساده‌ی Kavenegar فقط با API Key پیکربندی‌شده، Sender، شماره‌ی معتبر و مانده‌ی حداقل ۵۰٬۰۰۰ ریال اجرا می‌شود؛ پاسخ موفق/ناموفق در `DarooyabPrescriptionSMSMessage` ثبت می‌شود و فقط پیامک موفق در فرمول کیف پول هزینه محسوب می‌شود. `ApiBaseUrl`، مسیرهای `sms/send.json` و `verify/lookup.json`، Sender و قالب `AppDarooyabOTP` در `appsettings.json` هستند؛ API Key قدیمی که داخل کد Legacy قرار داشت به فایل جدید منتقل نشده است. ارسال OTP در Legacy از VerifyLookup استفاده می‌کرد؛ در Rewrite متد همان قرارداد در سرویس فراهم است ولی جریان OTP متناظری هنوز به آن متصل نشده. کلیک ارسال در View یک‌بار ثبت می‌شود و هنگام POST دکمه قفل/وضعیت Loading می‌گیرد.

**اعتبارسنجی:** مسیر داده به مدل/جدول موجود وصل شده و لینک Sidebar فعال است. نتیجه‌ی زنده هنوز با حساب مجاز توسعه/آزمون بررسی نشده؛ فیلتر و مالکیت، تغییر بررسی، تصویر، مانده‌ی ناکافی و ارسال آزمایشی پیامک باید پیش از بهره‌برداری با حساب و کلید چرخش‌یافته آزموده شوند.

**تغییر داده:** ندارد. `SELECT`ها read-only و `AsNoTracking` هستند. اگر حساب معتبر نباشد، نشست خارج و درخواست به Login هدایت می‌شود؛ این کار به معنی تغییر رکورد حساب نیست.

## ۲. ثبت موجودی داروخانه (کمبودهای دارویی)

**مسیر و پیاده‌سازی:** `GET/POST /PharmacyRequest/AddOrEdit`، جستجوی برند `GET /PharmacyRequest/SearchBrands`، جزئیات برند `GET /PharmacyRequest/BrandDetails/{irc}` و حذف `POST /PharmacyRequest/Delete/{id}`. Controller: [PharmacyRequestController](./Areas/Admin/Controllers/PharmacyRequestController.cs)، منطق داده: [PharmacyRequestService](./Services/PharmacyRequests/PharmacyRequestService.cs).

**کارکرد:** این فرم یک اعلام موجودی/درخواست «ارجاع بیمار» برای برند است؛ تعداد فیزیکی موجودی، سری ساخت و تاریخ انقضای خود دارو را ثبت نمی‌کند. این فیلدها متعلق به مازاد برای تبادل هستند. امکان خرید رتبه و انتشار/به‌روزرسانی آخرین قیمت برند از همین مسیر وجود دارد.

**جدول‌ها/داده‌های خواندنی:**

- `PharmacyAccount` و `Pharmacy`: حساب جاری و استان/داروخانه‌ی مالک برای قیدهای دسترسی، نمایش نام و تعیین محدوده‌ی رتبه.
- `PharmacyRequest`: درخواست‌های دسته‌ی `patient_referral` متعلق به حساب؛ فهرست با جستجو و صفحه‌بندی نمایش داده می‌شود.
- `DrugsNames`: نام فارسی/نمایشی برند بر اساس `IRC` و جستجوی برند.
- `Category`: گزینه‌های محدودیت تعداد (`Exchange_Seg1_CountLimit`)، گروه مشاهده (`Exchange_Seg1_PationRefferalView`) و نوع درخواست (`patient_referral`).
- `DarooyabBrandRestrictions`: رد برندهای ممنوعِ کامل در جستجو/ذخیره.
- `RankingTransaction`, `Pharmacy`, `TblBrandAttribute`: نمایش رتبه‌های فعال استان/IRC و آخرین قیمت و نام/شهر آخرین ویرایش‌کننده.
- `WalletTransaction`, `RankingTransaction`, `DarooyabPrescriptionSMSMessage`: محاسبه‌ی موجودی قابل استفاده‌ی کیف پول؛ محاسبه فقط خواندنی است.

**ثبت یا ویرایش درخواست در `PharmacyRequest`:**

- برای رکورد جدید، `PharmacyAccountIdFk`، `RequestTypeFromCategoryIdFk`، `Irc`، `RequestExpireDate`، `CountLimit`، `PationRefferalView`، `Description`، `RegistrationDateTime`، `DrugExpireDate = null` و `StatusNum = "1"` تنظیم می‌شوند.
- در upsert درخواست باز موجود، همان فیلدهای درخواست در بالا به‌روزرسانی می‌شوند؛ فیلدهای دیگری که در این فرم نیستند به‌عنوان فرم عمومی Entity بازنویسی نمی‌شوند.
- گروه‌های مشاهده در `PationRefferalView` به شکل رشته‌ی شناسه‌های Category جداشده با کاما ذخیره می‌شوند. `CountLimit` شناسه‌ی Category محدودیت است.
- هنگام ویرایش، اگر تاریخ پایان درخواست از رتبه‌ی فعال همان IRC کوتاه‌تر شود، رکورد `RankingTransaction` متناظر نیز تنظیم می‌شود: `DurationInDays`، `EndDate` و `ModifyDescription`.

**خرید رتبه (در صورت انتخاب کاربر):**

- در همان تراکنش ثبت درخواست، یک ردیف جدید در `RankingTransaction` درج می‌شود. فیلدهای مقداردهی‌شده: `RegisterDate`، `RankNumber`، `StartDate`، `EndDate`، `PharmacyAccountId`، `PharmacyId`، `Amount`، `Irc`، `ProvinceId` و `DurationInDays`.
- قیمت از رتبه‌ی خالی بعدی و تعرفه‌ی سمت سرور محاسبه می‌شود؛ موجودی و شماره‌ی رتبه‌ی ارسالی مرورگر مرجع نهایی نیست. اگر مدت رتبه از مدت درخواست بیشتر باشد، `RequestExpireDate` نیز به اندازه‌ی لازم افزایش می‌یابد.
- در Login یا بارگذاری جزئیات برند هیچ تراکنش رتبه یا برداشت کیف پولی نوشته نمی‌شود. اثر مالی خرید در این پیاده‌سازی از `RankingTransaction.Amount` ثبت‌شده محاسبه می‌شود؛ `WalletTransaction` در این مسیر ویرایش/کسر نمی‌شود.

**به‌روزرسانی آخرین قیمت برند (در صورت انتخاب کاربر):**

- در `TblBrandAttribute` اگر ردیف قیمت موجود باشد، آخرین ردیف بر اساس `LastPriceEditDate` و شناسه به‌روزرسانی می‌شود؛ در غیر این صورت یک ردیف جدید ایجاد می‌شود.
- ستون‌های نوشته‌شده: `TblBrandAttributeIrc` برای درج، `Price`، `PriceUnitNum`، `LastPriceEditorPharmacyFk` و `LastPriceEditDate`.
- درج/ویرایش قیمت و خرید رتبه (اگر هر دو انتخاب شده باشند) همراه درخواست موجودی در تراکنش واحد انجام می‌شوند.

**حذف درخواست:** رکورد متعلق به حساب و از نوع `patient_referral` از `PharmacyRequest` حذف می‌شود. اگر رتبه‌ی فعال همان حساب/IRC وجود داشته باشد، ردیف `RankingTransaction` حذف نمی‌شود؛ `ModifyDescription`، `DurationInDays` و `EndDate` برای خاتمه‌ی رتبه به‌روزرسانی می‌شوند. مبلغ رتبه بازپرداخت نمی‌شود. `DrugsNames`، `Category` و `TblBrandAttribute` در حذف درخواست تغییر نمی‌کنند.

**محاسبه‌ی کیف پول (فقط خواندن):** موجودی ریالی برابر است با مجموع `WalletTransaction.Amount` حساب جاری که `RefId` خالی نیست و `PaymentDate` دارد، منهای مجموع `RankingTransaction.Amount` همان حساب و منهای `50,000` ریال به‌ازای هر پیام موفق در `DarooyabPrescriptionSMSMessage` با `IsSuccess == true`. این منو `WalletTransaction` یا رکورد SMS را ایجاد/ویرایش نمی‌کند.

## ۳. ثبت کسری داروخانه

**مسیر و پیاده‌سازی:** `GET/POST /DrugShortage/AddOrEdit`، جستجو `GET /DrugShortage/Search`، فهرست شهر `GET /DrugShortage/Townships`، نتایج Modal `POST /DrugShortage/ExchangeOffers` و حذف `POST /DrugShortage/Delete/{id}`. Controller: [DrugShortageController](./Areas/Admin/Controllers/DrugShortageController.cs)، منطق داده: [PharmacyDrugShortageService](./Services/PharmacyDrugShortages/PharmacyDrugShortageService.cs).

**کارکرد:** یک درخواست کسری با برند یا شکل دارویی ثبت می‌کند. نتایج عادی/ویژه از Stored Procedure موجود خوانده می‌شوند؛ تب تاریخ‌نزدیک Query مستقیم فقط‌خواندنی دارد. مشاهده‌ی پیشنهادها در این صفحه معامله یا ردیف تبادل جدید ایجاد نمی‌کند.

**جدول‌ها/داده‌های خواندنی:**

- `PharmacyRequest`: درخواست‌های کسری متعلق به حساب، دسته‌ی `drug_shortages` و وضعیت باز؛ مرتب‌سازی فهرست `Id DESC`.
- `Category`: شناسه‌ی دسته‌ی `drug_shortages`، استان‌ها، شهرها و رابطه‌ی Parent برای اعتبارسنجی فیلتر مکانی.
- `PharmacyAccount` و `Pharmacy`: بررسی وضعیت حساب و اخذ استان/شهر داروخانه برای مقدارهای اولیه‌ی فیلتر.
- `DrugsNames` و `DosageForms`: جستجو و نمایش نام انتخاب‌شده؛ `DarooyabBrandRestrictions` برای منع برند ممنوع.
- `PharmacyExchange`: فقط بررسی اینکه درخواست کسری پیش از حذف، سابقه‌ی تبادل دارد یا خیر.
- عادی/ویژه: Stored Procedure `dbo.SP_GetOtherPharmacyExcess_ForThisShortage` و projection keyless `PharmacyDrugShortageExchangeRow`. خواندن واقعی آن به وجود Procedure و سازگاری schema خروجی در DB استقرار نیاز دارد؛ در این محیط integration-test نشده است.
- تاریخ‌نزدیک: `PharmacyRequest` مازاد (`excess_drugs`) و `DrugsNames`، با فیلتر IRC، `ExchangeLimitNum = "3"`، `StatusNum = "1"`، تاریخ انقضای امروز یا بعد، استان/شهر؛ نتایج به‌ازای داروخانه گروه‌بندی می‌شوند.

**ثبت یا ویرایش در `PharmacyRequest`:**

- درج: `PharmacyAccountIdFk`، `RequestTypeFromCategoryIdFk`، `RequestView = 0`، `RegistrationDateTime`، `RequestExpireDate = null`، `DrugExpireDate = null`، `StatusNum = "1"` و یکی از `Irc` یا `DrugShapFromDosageIdFk` نوشته می‌شوند.
- ویرایش فقط رکورد متعلق به همان حساب و دسته‌ی کسری را هدف می‌گیرد و همین فیلدهای مرتبط با فرم را تنظیم می‌کند؛ مقدار انتخاب برند/شکل جایگزین می‌شود. هیچ Entity یا جدول مستقل دیگری ایجاد نمی‌شود.
- فیلدهای فرم شامل شناسه‌ی درخواست، نوع جستجو و شناسه‌ی برند/شکل‌اند؛ خرید رتبه، ثبت تعداد موجودی و ثبت کمبود در مدل این صفحه وجود ندارد.

**حذف:** ردیف `PharmacyRequest` فقط اگر متعلق به حساب جاری، دسته‌ی کسری و بدون ارجاع وابسته در `PharmacyExchange` باشد حذف می‌شود. ردیف‌های `PharmacyExchange`، دارو/دسته‌بندی یا داده‌ی داروخانه‌ی پیشنهاددهنده تغییر نمی‌کنند. در صورت وجود Exchange حذف رد می‌شود.

**مشاهده‌ی نتایج:** endpoint نتایج در این قابلیت فقط SELECT می‌کند. هیچ ستون/ردیفی از `PharmacyRequest` مازاد یا `PharmacyExchange` به‌علت بازکردن Modal تغییر نمی‌کند. دکمه‌ی «تبادل» که در Legacy مخفی بود به جریان ثبت معامله منتقل نشده است.

## ۴. اقلام تاریخ نزدیک سایرین

**مسیر و پیاده‌سازی:** `GET /DrugShortage/otherPhNearDateDrugShortageList?provId=&cityId=&page=` و دریافت شهرها با `GET /DrugShortage/Townships?provinceId=`. Controller: [DrugShortageController](./Areas/Admin/Controllers/DrugShortageController.cs)، Query: [PharmacyDrugShortageService](./Services/PharmacyDrugShortages/PharmacyDrugShortageService.cs)، View: [OtherPhNearDateDrugShortageList.cshtml](./Areas/Admin/Views/DrugShortage/OtherPhNearDateDrugShortageList.cshtml).

**کارکرد:** فهرست مازادهای تاریخ‌نزدیک داروخانه‌های دیگر با فیلتر استان/شهر و صفحه‌بندی ۱۰تایی. صفحه به حساب فعال داروخانه محدود است، اما برای حساب‌های عرضه‌کننده‌ی دیگر فیلتر وضعیت فعالیت اضافه نمی‌کند؛ اکشن Legacy نیز فیلتر فعال‌بودن آن حساب‌ها را در کد غیرفعال/comment کرده بود. فیلتر شهر بر استان اولویت دارد. ترتیب اصلی `RegistrationDateTime DESC` است و برای زمان‌های برابر ترتیب ثانویه‌ای اضافه نشده است.

**جدول‌ها و فیلدهای فقط‌خواندنی:**

- `Category`: نوع `province` برای گزینه‌های استان و نوع `township` با `ParentID` استان برای فهرست شهرها؛ ترتیب ضمنی قدیمی حفظ شده و مرتب‌سازی تازه‌ای افزوده نشده است.
- `PharmacyRequest`: شرط دسته‌ی `excess_drugs`، حسابی غیر از حساب جاری، `ExchangeLimitNum = "3"`، `StatusNum = "1"` و `DrugExpireDate` از ابتدای روز جاری به بعد؛ از فیلتر نقدی (`SellToColleague`) استفاده نمی‌شود چون در Legacy غیرفعال بود.
- `DrugsNames`: فقط ردیف‌های `DrugNameLanguage == true` با IRC یکسان؛ همانند Legacy اتصال از نوع inner join است.
- `PharmacyAccount` و `Pharmacy`: نام داروخانه، استان/شهر، نشانی و تلفن نمایش داده می‌شوند. شماره‌ی همراه مدیر در View اصلی Legacy نمایش داده نمی‌شد و در نسخه‌ی جدید نیز اضافه نشده است.
- `PharmacyExchange`: مجموع `RequireCount` همه‌ی تبادلات هر درخواست مازاد برای محاسبه‌ی تعداد باقیمانده؛ این محاسبه و فهرست هیچ ردیفی را تغییر نمی‌دهند.

تعداد کل صفحه از شمارش درخواست‌های واجد شرایط پیش از اتصال نام انگلیسی دارو محاسبه می‌شود، مطابق ترتیب اجرای اکشن Legacy. تاریخ ثبت و زمان نسبی، نام دارو، تاریخ انقضا، تعداد باقیمانده، واحد، قیمت و اطلاعات تماس خروجی هستند. هیچ `INSERT`، `UPDATE` یا `DELETE`ای در این منو انجام نمی‌شود.

**AJAX و جریان‌های منتقل‌نشده:** تغییر استان با `fetch` به endpoint شهرهای موجود در همین Controller انجام می‌شود و پاسخ JSON را در Select شهر قرار می‌دهد؛ جستجو و صفحه‌بندی GET هستند و Partial جداگانه‌ای برای خود فهرست وجود ندارد. دکمه‌ی «تبادل» و Popup در View قدیمی comment/غیرفعال بودند؛ handler باقی‌مانده‌ی jQuery به `POST /ExchangeDrug/PartialExchange_CallByAjax` مربوط می‌شد، اما از UI فعال قابل فراخوانی نبود. آن دکمه، Partial و جریان ثبت تبادل در این صفحه فعال نشده‌اند و `PharmacyExchange` فقط خوانده می‌شود. اجرای Query زنده نیازمند اتصال مجاز به دیتابیس است و در این محیط integration-test نشده است.

## ۵. مازاد بر نیاز جهت تبادل

**مسیر و پیاده‌سازی:** `GET /PharmacyRequest/TabadolManage`، فهرست جزئی `GET /PharmacyRequest/TabadolManage/List`، ثبت `POST /PharmacyRequest/TabadolManage`، ویرایش تاریخ `POST /PharmacyRequest/TabadolManage/{id}/Expiry`، حذف `POST /PharmacyRequest/TabadolManage/{id}/Delete` و خاتمه `POST /PharmacyRequest/TabadolManage/{id}/Finish`. Controller مشترک: [PharmacyRequestController](./Areas/Admin/Controllers/PharmacyRequestController.cs)، منطق داده: [PharmacyExchangeService](./Services/PharmacyRequests/PharmacyExchangeService.cs).

**کارکرد:** ایجاد یا مدیریت اعلام مازاد دارو برای تبادل. در نسخه‌ی فعلی فرم پایه فقط برند، نوع مازاد و ماه/سال انقضا را می‌گیرد؛ فیلدهای تعداد، قیمت و سایر اطلاعات فرم کامل در UI پنهان/غیرفعال‌اند و ثبت پایه آن‌ها را مقدار موجودی قابل‌تبادل نمی‌کند.

**جدول‌ها/داده‌های خواندنی:**

- `PharmacyRequest`: مازادهای دسته‌ی `excess_drugs`، مالک جاری، وضعیت باز؛ فهرست و جستجو بر اساس IRC/نام برند.
- `Category`: دسته‌ی `excess_drugs` و اطلاعات داروخانه/شهر برای query.
- `DrugsNames`: بررسی برند، نام، IRC و منع برندهای `DarooyabBrandRestrictions`.
- `PharmacyExchange`: خواندن تعداد تبادل‌شده برای فهرست/محافظت از حذف مازادی که معامله دارد.
- `PharmacyAccount` و `Pharmacy`: اعتبارسنجی حساب، نمایش نام مالک و محدوده‌ی اطلاعات داروخانه.

**ثبت پایه‌ی `SaveBasicAsync` در `PharmacyRequest`:**

- اگر برای IRC درخواست باز موجود باشد، همان ردیف به‌روزرسانی می‌شود؛ نوع، تاریخ و زمان ثبت به‌روز می‌شوند و `DrugUnitNum = "2"` و `SellToColleague = false` نیز روی مقدار ثابت خود تنظیم می‌شوند.
- اگر ردیف باز موجود نباشد، ردیف جدید درج می‌شود با `PharmacyAccountIdFk`، `RequestTypeFromCategoryIdFk`، `Irc`، `ExchangeRandomCode`، `ExchangeLimitNum`، `DrugCount = 0`، `DrugUnitNum = "2"`، `SellToColleague = false`، `DrugExpireDate`، `RegistrationDateTime` و `StatusNum = "1"`.
- عملیات پایه **تعداد مثبت، قیمت، سری ساخت، شماره فاکتور، تخفیف یا تسهیلات را تنظیم نمی‌کند**؛ `ColleaguePrice` نیز به‌دلیل ثابت بودن `SellToColleague = false` تهی می‌ماند. در نتیجه از ایجاد ردیف جدید، موجودی مثبت یا قابل‌تبادل نتیجه نمی‌شود. فهرست ممکن است فیلدهای موجودِ این ستون‌ها را نمایش دهد اما این فرم پایه آن‌ها را تغییر نمی‌دهد.
- ورودی چندبرندی به ازای هر IRC جداگانه upsert می‌شود؛ عملیات در تراکنش Serializable انجام می‌شود.

**ویرایش تاریخ انقضا:** `PharmacyRequest.DrugExpireDate` برای رکورد باز متعلق به همان مالک/دسته به آخرین روز ماه انتخاب‌شده تنظیم می‌شود؛ هم‌زمان `DrugUnitNum = "2"` و `SellToColleague = false` نیز تضمین می‌شوند و `ColleaguePrice` تهی می‌شود.

**حذف:** ردیف مازاد از `PharmacyRequest` حذف می‌شود، فقط اگر متعلق به مالک/دسته‌ی صحیح، باز و بدون ردیف وابسته در `PharmacyExchange` باشد. در غیر این صورت حذف رد می‌شود؛ داده‌ی Exchange حذف آبشاری یا بازنویسی نمی‌شود.

**خاتمه:** فقط `PharmacyRequest.StatusNum` به مقدار وضعیت بسته‌شده توسط داروخانه (`"2"` در سرویس فعلی) تغییر می‌کند. رکورد فیزیکی حذف نمی‌شود و فیلدهای تعداد/قیمت/انقضا تغییر نمی‌کنند.

## ۶. کیف پول

**مسیر و پیاده‌سازی:** `GET /Wallet`، آغاز پرداخت `POST /Wallet/Charge`، بررسی دستی وضعیت `POST /Wallet/Verify/{transactionId}` و بازگشت درگاه `GET /Wallet/Callback`. Controller: [WalletController](./Areas/Admin/Controllers/WalletController.cs)، سرویس: [PharmacyWalletService](./Services/PharmacyRequests/PharmacyWalletService.cs)، قرارداد صفحه: [PharmacyWalletViewModels.cs](./Areas/Admin/ViewModels/PharmacyWalletViewModels.cs).

**کارکرد:** نمایش جمع شارژهای تأییدشده، جمع هزینه‌های منظورشده در محاسبه‌ی موجودی، مانده‌ی قابل استفاده و تاریخچه‌ی تراکنش‌های حساب جاری (۲۰ رکورد در هر صفحه). شارژ آنلاین ZarinPal با تنظیمات `Wallet:ZarinPal` در `appsettings.json` فعال است: MerchantId از مقدار موجود در کد پروژه‌ی مرجع گرفته شده و CallbackUrl برابر `https://www.darooyab.ir/Wallet/Callback` است. اگر دامنه‌ی استقرار تغییر کند، CallbackUrl نیز باید با دامنه‌ی عمومی HTTPS همان استقرار تغییر کند. بازگشت از درگاه به‌تنهایی شارژ محسوب نمی‌شود و مبلغ فقط پس از Verify سمت سرور ثبت‌شده تلقی می‌شود.

**جدول‌ها و فیلدهای فقط‌خواندنی:**

- `WalletTransactions`: تاریخچه‌ی همان `AccountId`؛ فیلدهای `Id`, `RegisterDate`, `TransactionType`, `Amount`, `Authority`, `RefId`, `PaymentDate`.
- `WalletTransactions`: جمع شارژهای تأییدشده، با شرط `RefId` غیرخالی و `PaymentDate` ثبت‌شده؛ `Amount` در DB به ریال و در UI تقسیم بر ۱۰ و به تومان نمایش داده می‌شود.
- `RankingTransactions`: جمع مبالغ رتبه‌های خریده‌شده‌ی همان حساب؛ فیلد `Amount` ریالی.
- `DarooyabPrescriptionSMSMessages`: شمارش پیامک‌های موفق همان حساب با `IsSuccess == true`؛ هزینه‌ی محاسباتی هر مورد ۵۰,۰۰۰ ریال است.
- `PharmacyAccount`/`Pharmacy`: فقط برای فعال‌بودن حساب و نام نمایش‌داده‌شده در Layout.

**فرمول مانده:** `(شارژهای دارای RefId و PaymentDate - مجموع RankingTransactions.Amount - تعداد پیامک موفق × ۵۰,۰۰۰) ÷ ۱۰`. مانده‌ی نمایشی منفی به صفر clamp می‌شود. با خرید رتبه یا ارسال پیامک، سرویس کیف پول هیچ ردیفی در این جداول نمی‌نویسد؛ مصرف از داده‌های واقعی همان قابلیت‌ها محاسبه می‌شود.

**درخواست شارژ:** مقدار ورودی به تومان و در بازه‌ی قدیمی `1,100` تا `20,000,000` اعتبارسنجی می‌شود؛ مبلغ به ریال (`× 10`) به درگاه ارسال می‌شود. پس از پاسخ معتبر درگاه، یک ردیف جدید در `WalletTransactions` درج می‌شود: `AccountId` از Claim کاربر جاری، `Amount` ریالی، `RegisterDate` زمان سرور، `Authority` صادرشده‌ی درگاه، `TransactionType = Cach` و `Description` ثابت شارژ. هیچ مقدار AccountId یا Authority از مرورگر برای ساخت ردیف پذیرفته نمی‌شود. سقف درخواست‌های پرداخت نیز per-IP محدود شده است.

**Callback و تأیید پرداخت:** endpoint بازگشت `GET /Wallet/Callback` است و در تنظیمات به `https://www.darooyab.ir/Wallet/Callback` اشاره می‌کند. زرین‌پال پارامترهای `Authority` و `Status` را به آن برمی‌گرداند. این endpoint در نسخه‌ی فعلی زیر Policy احراز هویت داروخانه قرار دارد؛ بنابراین مرورگر باید با همان حساب داروخانه‌ای که پرداخت را آغاز کرده وارد باشد. اگر `Status` موفق نباشد، هیچ Verify یا تغییری در رکورد انجام نمی‌شود. در حالت موفق، سرویس `Authority` را همراه شناسه‌ی حساب جاری روی ردیف pending همان حساب تطبیق می‌دهد؛ مبلغ Verify از `WalletTransactions.Amount` خوانده می‌شود و از query یا فرم پذیرفته نمی‌شود. بعد از پاسخ موفق/قبلاً تأییدشده‌ی Verify و وجود کد پیگیری، فقط `WalletTransactions.RefId` و `PaymentDate` با UPDATE شرطی روی همان `Id`, `AccountId`, `Authority` و وضعیت pending نوشته می‌شوند؛ تکرار callback/Verify مبلغ را دوباره اعمال نمی‌کند. برای تراکنش ناموفق/لغوشده، مقدار `RefId` و `PaymentDate` تغییر نمی‌کنند. دکمه‌ی بررسی دستی `POST /Wallet/Verify/{transactionId}` و Anti-Forgery دارد. callback درگاه GET است و فقط پس از احراز هویت حساب و کنترل مالکیت تراکنش پردازش می‌شود.

**درگاه و محدودیت انتقال:** ثبت `ZarinPalOptions` در `Program.cs`، سرویس `PharmacyWalletService` و `WalletController` از بخش `Wallet:ZarinPal` در پیکربندی ASP.NET Core استفاده می‌کنند؛ مقدارهای پیش‌فرض در `appsettings.json` قرار دارند و override استاندارد با User Secrets یا متغیرهای محیطی `Wallet__ZarinPal__MerchantId` و `Wallet__ZarinPal__CallbackUrl` همچنان ممکن است. MerchantId همان شناسه‌ی موجود در کد Legacy است و مقدار ساختگی اضافه نشده است. callback ثابت و HTTPS است و از Host درخواست ساخته نمی‌شود. برای کارکرد بازگشت، دامنه باید از اینترنت با HTTPS به برنامه برسد و URL callback باید با دامنه/مسیر ثبت‌شده در پنل پذیرنده سازگار باشد؛ اگر دامنه یا مسیر واقعی استقرار متفاوت است، تنظیم `CallbackUrl` و ثبت پذیرنده باید با هم اصلاح شوند. پرداخت زنده تا تأیید فعال‌بودن MerchantId، ثبت callback در پنل پذیرنده‌ی ZarinPal و دسترسی شبکه‌ای دامنه به این برنامه وابسته است و در این محیط integration-test زنده انجام نشده است.

## ۷. رتبه‌های خریداری‌شده

**مسیر و پیاده‌سازی:** `GET /Wallet/Rankings?page={page}` در [WalletController](./Areas/Admin/Controllers/WalletController.cs)، سرویس [PharmacyWalletService](./Services/PharmacyRequests/PharmacyWalletService.cs)، قرارداد [PharmacyWalletViewModels.cs](./Areas/Admin/ViewModels/PharmacyWalletViewModels.cs) و View [Rankings.cshtml](./Areas/Admin/Views/Wallet/Rankings.cshtml).

**کارکرد مطابق Legacy:** ردیف‌های `RankingTransactions` حساب جاری با `Id` نزولی و ۵ مورد در هر صفحه نمایش داده می‌شوند. نام برند از `DrugsNames` با `IRC` متناظر و `DrugNameLanguage == false` خوانده می‌شود و در نبود نام، «نامشخص» نمایش می‌یابد. شروع و انقضا با تقویم شمسی و قالب تاریخی صفحه‌ی قدیمی، مدت خرید، رتبه/تصویر رتبه، مبلغ `Amount / 10` به تومان و `ModifyDescription` غیرخالی نمایش داده می‌شوند. صفحه‌های خارج از بازه به نزدیک‌ترین صفحه‌ی معتبر محدود می‌شوند.

**AJAX، Partial و عملیات تغییردهنده:** View قدیمی `Views/Wallet/Rankings.cshtml` هیچ فراخوانی jQuery/AJAX، Partial یا عملیات نوشتنی نداشت؛ تنها صفحه‌بندی با لینک GET انجام می‌شد. دکمه‌ی «خرید مجدد» در View قدیمی کامنت شده بود؛ بنابراین در نسخه‌ی جدید نیز خرید، ویرایش یا حذف از این صفحه فعال نیست. Controller قدیمی برای مقادیر `EditUrl` و `ShowButton` یک درخواست داروخانه را هم می‌خواند، اما چون markup مصرف‌کننده‌ی آن کامنت بود، این خواندنِ بدون خروجی در انتقال حذف شده است. نسخه‌ی جدید جدول واکنش‌گرا را در موبایل به ردیف‌های کارت‌مانند تبدیل می‌کند؛ برای این صفحه JavaScript اختصاصی لازم نیست.

**جدول‌ها و فیلدهای فقط‌خواندنی:** `RankingTransactions` فیلدهای `Id`, `PharmacyAccountId`, `Irc`, `StartDate`, `EndDate`, `DurationInDays`, `RankNumber`, `Amount`, `ModifyDescription` و `DrugsNames` فیلدهای `Irc`, `DrugNameLanguage`, `DrugName`. داده فقط برای حساب Claim جاری خوانده می‌شود؛ هیچ ردیفی درج/ویرایش/حذف نمی‌شود و Schema/Migration جدید لازم نیست. در این محیط اتصال زنده برای تأیید محتوای حساب و صفحات نتیجه اجرا نشده است.

## ۸. کمک به تکمیل برندها

**مسیر و پیاده‌سازی:** `GET /Brand/SubmitBrandNameByPharmacy` و `POST /Brand/SubmitBrandNameInTblContactus`. Controller: [BrandController](./Areas/Admin/Controllers/BrandController.cs)، سرویس: [PharmacyBrandSuggestionService](./Services/PharmacyBrandSuggestions/PharmacyBrandSuggestionService.cs)، View: [SubmitBrandNameByPharmacy.cshtml](./Areas/Admin/Views/Brand/SubmitBrandNameByPharmacy.cshtml).

**کارکرد و دسترسی:** مدیر داروخانه‌ی فعال نام فارسی برندی را که در سامانه پیدا نمی‌کند برای بررسی ارسال می‌کند. POST با Anti-Forgery و Policy `PharmacyAccount` محافظت شده است؛ شناسه و نام داروخانه از Claim و رابطه‌ی دیتابیس به دست می‌آیند، نه از ورودی مرورگر. موفقیت فقط به معنی ارجاع پیشنهاد برای بررسی است؛ هیچ برند یا رکوردی در `DrugsNames` خودکار ساخته یا تأیید نمی‌شود.

**جدول‌ها و فیلدهای تغییرکرده:**

- `PharmacyAccount` و `Pharmacy`: فقط خواندن حساب جاری، وضعیت فعال و نام داروخانه.
- `Category`: مقدار ثابت `53` برای `TBL_Contactus_CategoryID_fk` مطابق اکشن Legacy (نظر کد قدیمی: «سایر») استفاده می‌شود؛ ردیف Category تغییر نمی‌کند.
- `TBL_Contactus`: با هر ارسال معتبر یک ردیف درج می‌شود. فیلدها: `TBL_Contactus_NameAndFamily = "تکمیل برند"`، `TBL_Contactus_CategoryID_fk = 53`، `TBL_Contactus_OrganizationOrCompanyName = "داروخانه " + Pharmacy.Name`، `TBL_Contactus_TextMessage = نام فارسی پیشنهادی`، `TBL_Contactus_ViewByManagerStatusNum = 2` (دیده‌نشده)، `TBL_Contactus_RegisterDate = DateTime.Now.Date`، `TBL_Contactus_ViewByManagerDate = null` و `TBL_Contactus_IPSystem = IP درخواست` در صورتی که حداکثر ۲۵ نویسه باشد. سایر فیلدهای متنی که اکشن قدیمی مقداردهی نمی‌کرد (`Role`, `Tel`, `Email`) در Entity جدید رشته‌ی خالی می‌گیرند تا با نگاشت non-null آن سازگار باشند.
- هیچ فیلدی از `DrugsNames`، `TblBrandAttributes` یا اطلاعات حساب داروخانه نوشته/ویرایش نمی‌شود و Schema/Migration جدیدی وجود ندارد.

**AJAX و خطاها:** View قدیمی یک `Ajax.BeginForm` با Anti-Forgery داشت و محتوای بازگشتی سرور را با jQuery در `html()` قرار می‌داد. نسخه‌ی جدید همان مسیرهای GET/POST و تجربه‌ی ارسال درون‌صفحه‌ای را با `fetch` حفظ می‌کند؛ پیام نتیجه نام برند پیشنهادی را شامل می‌شود اما با `textContent` نمایش داده می‌شود تا ورودی به‌عنوان HTML تفسیر نشود. دکمه هنگام ارسال غیرفعال می‌شود و حالت موفقیت/خطا اعلام می‌گردد. Partial جداگانه‌ای در این جریان وجود ندارد. اتصال به دیتابیس استقرار و وجود Category با شناسه‌ی ۵۳ در این محیط integration-test نشده است.

## ۹. تازه‌های شرکت پخش

**مسیر و پیاده‌سازی:** `GET /PharmacyAccount/DrugDistribution` با پارامترهای `drugName`, `factoryId` و `page`. [PharmacyAccountController](./Areas/Admin/Controllers/PharmacyAccountController.cs) فقط حساب فعال Claim جاری را می‌پذیرد؛ query و نگاشت در [PharmacyDrugDistributionService](./Services/PharmacyDrugDistributions/PharmacyDrugDistributionService.cs)، مدل صفحه در [PharmacyDrugDistributionViewModels.cs](./Areas/Admin/ViewModels/PharmacyDrugDistributionViewModels.cs) و View در [DrugDistribution.cshtml](./Areas/Admin/Views/PharmacyAccount/DrugDistribution.cshtml) هستند.

**کارکرد مطابق Legacy:** تمام ردیف‌های توزیع با فیلتر نام برند از `DrugsNames.DrugName.Contains(drugName)` و فیلتر اختیاری `FactoryId` خوانده می‌شوند؛ گزینه‌های فهرست شرکت از `Factory`هایی با `FactoryTypeId == 4` می‌آیند. نتایج پس از فیلتر با `Id` نزولی و ۵ ردیف در صفحه نمایش داده می‌شوند. نام برند، نام شرکت، تاریخ شمسی اعلام و توضیحات قابل نمایش‌اند. صفحه‌بندی نسخه‌ی جدید فیلترهای فعال را حفظ می‌کند؛ در Legacy لینک‌های helper فیلتر جاری را همراه نداشتند.

**AJAX، Partial و نمایش توضیحات:** View قدیمی فقط `Html.BeginForm` از نوع GET و لینک صفحه‌بندی داشت؛ در خود صفحه یا اکشن `DrugDistribution` هیچ jQuery/AJAX یا Partial وجود ندارد. نسخه‌ی جدید نیز فرم GET عادی را نگه می‌دارد و جاوااسکریپت اختصاصی اضافه نمی‌کند. توضیحات قدیمی با `Html.Raw` بدون پاک‌سازی رندر می‌شد؛ در نسخه‌ی جدید HTML پیش از نمایش با `HtmlSanitizer` پاک‌سازی می‌شود تا قالب‌بندی مجاز بماند و محتوای خطرناک اجرا نشود. جدول دسکتاپ در موبایل به کارت‌های ردیفی تبدیل می‌شود.

**جدول‌ها و فیلدهای فقط‌خواندنی:** `DarooyabDrugDistributions`: `Id`, `DrugsNameId`, `FactoryId`, `AnnouncementDate`, `Description`; `DrugsNames`: `Id`, `DrugName`; `Factory`: `FactoryId`, `FactoryName`, `FactoryTypeId`. هیچ رکوردی درج، ویرایش یا حذف نمی‌شود و Schema/Migration جدیدی وجود ندارد. جستجو و نتیجه‌ی زنده به اتصال مجاز پایگاه‌داده نیاز دارد و در این محیط integration-test نشده است.

## ۱۰. مدیریت اطلاعات داروخانه

**مسیر و دسترسی:** `GET/POST /DarooyabPharmacy/PharmacySpecialties` در [DarooyabPharmacyController](./Areas/Admin/Controllers/DarooyabPharmacyController.cs) و [PharmacySpecialtiesService](./Services/PharmacySpecialties/PharmacySpecialtiesService.cs)، زیر Policy حساب داروخانه و محدود به Pharmacy متصل به Claim جاری. لینک «مدیریت اطلاعات داروخانه» در Sidebar به همین مسیر اشاره می‌کند. View رندر اولیه‌ی شش بخش را انجام می‌دهد؛ بخش «انتخاب منطقه» مطابق مرجع فقط وقتی وجود منطقه‌ی فعال برای شهر داروخانه باشد نمایش داده می‌شود. تعویض تب، مقدار `active` نشانی را بدون بارگذاری مجدد با `history.replaceState` تغییر می‌دهد.

**داده‌های فقط‌خواندنی:** گزینه‌های تخصص فعال با ترتیب `OrderIndex`، مناطق فعال همان شهر با ترتیب `OrderIndex` و سپس نام، بیمه‌گرهای فعال با ترتیب `OrderIndex` و سپس نام و نگاشت‌های فعال داروخانه خوانده می‌شوند. نام استان/شهر، نشانی و تلفن ثابت صرفاً برای نمایش از `Category` و `Pharmacy` می‌آیند. مالکیت همه‌ی خواندن‌ها و تغییرها با حساب احراز‌شده تعیین می‌شود.

**بخش گروه دارویی:** POST همان مسیر صفحه، حداکثر دو شناسه‌ی فعال `PharmacySpecialty` را می‌پذیرد. در تراکنش، نگاشت‌های قبلی و حذف‌نشده در `PharmacySpecialtyMap` با `IsDeleted = true` و `IsActive = false` soft-delete و برای انتخاب‌های تازه ردیف با `PharmacyId_fk`، `PharmacySpecialtyId_fk`، `CreateDate`، `IsDeleted = false` و `IsActive = true` درج می‌شود. انتخاب صفر مورد مجاز است و نگاشت‌های قبلی را غیرفعال می‌کند.

**بخش منطقه:** POST `/PharmacyCityDistricts/Index` مقدار `districtId` را فقط در صورت فعال بودن و تعلق به شهر همان داروخانه می‌پذیرد؛ فقط `Pharmacy.DistrictId` به‌روزرسانی می‌شود.

**بخش لوکیشن:** با انتخاب نقشه، کشیدن نشانگر یا درخواست موقعیت مرورگر، جاوااسکریپت POST `/GeoLocation/SetPharmacyGeoLocarion` را با Anti-Forgery token اجرا می‌کند. محدوده‌ی مختصات سمت سرور اعتبارسنجی و فقط `Pharmacy.Latitude` و `Pharmacy.Longitude` همان داروخانه ذخیره می‌شوند. نقشه از Leaflet و tileهای OpenStreetMap استفاده می‌کند؛ مقدار شروع در صورت نبود مختصات، تهران است. ذخیره با AJAX انجام می‌شود و وضعیت موفقیت/خطا در صفحه اعلام می‌گردد.

**بخش تصاویر:** فرم POST `/DarooyabPharmacy/ManageImages` حداکثر سه JPG/JPEG یا WebP با سقف ۵۰ کیلوبایت و ابعاد حداکثر ۶۰۰×۴۰۰ را می‌پذیرد؛ ترتیب تصویر دوم و سوم وابسته به وجود تصویر قبلی است. URL فایل‌های جدید در `Pharmacy.Image1Url`, `Image2Url`, `Image3Url` ذخیره و فایل‌های قبلیِ مدیریت‌شده پاک‌سازی می‌شوند. حذف هر تصویر با AJAX POST `/DarooyabPharmacy/DeleteImage` و Anti-Forgery انجام می‌شود؛ URLهای خارج از مسیر فایل مدیریت‌شده پاک نمی‌شوند. خطاها و نتیجه‌ی ذخیره در رابط اعلام می‌شوند.

**بخش اطلاعات تماس:** POST `/PharmacyContactInfo/Save` سه شماره‌ی موبایل مدیر داروخانه، مسئول موجودی و تکنسین را پس از نرمال‌سازی رقم فارسی/عربی و اعتبارسنجی قالب موبایل در `Pharmacy.PharmacyManagerMobile`, `InventoryManagerMobile` و `PharmacyTechnicianMobile` ذخیره می‌کند. نشانی، تلفن ثابت، استان و شهر فقط‌خواندنی‌اند.

**بخش بیمه‌ها:** POST `/DarooyabPharmacy/SavePharmacyInsurances` دست‌کم یک بیمه‌ی پایه‌ی فعال را الزام می‌کند؛ بیمه‌ی تکمیلی اختیاری است و نوع بیمه از رکوردهای `InsuranceProviders` سمت سرور تعیین می‌شود. در تراکنش، قراردادهای حذف‌نشده‌ی قبلی در `PharmacyInsuranceContracts` با `IsDeleted = true` و `IsActive = false` soft-delete و قراردادهای منتخب با `PharmacyIdFk`, `InsuranceProviderIdFk`, `CreateDate`, `IsDeleted = false`, `IsActive = true` درج می‌شوند.

**AJAX، خطا و محدودیت استقرار:** Viewهای Partial برای هر یک از شش بخش استفاده می‌شوند؛ در Legacy عملیات منطقه، تخصص، تماس و بیمه فرم‌های POST عادی بودند و نسخه‌ی جدید نیز همین چرخه‌ی ارسال/بازگشت به تب را دارد. عملیات موقعیت و حذف تصویر AJAX هستند؛ هر POST دارای Anti-Forgery است. Schema یا Migration ایجاد نشده؛ مدل و نگاشت جدول‌های بیمه بر اساس قرارداد Legacy افزوده شده‌اند و ستون‌های تماس نیز باید با schema محیط استقرار تطبیق داده شوند. Build و آزمون‌های قرارداد/اعتبارسنجی تصویر موفق‌اند، اما داده‌ی زنده و نوشتن در دیتابیس استقرار در این محیط آزموده نشده است.

## منوهای غیرفعال/فاقد قابلیت عملیاتی

مقادیر وضعیت در سایدبار برای جلوگیری از نمایش قابلیت پیاده‌سازی‌نشده عمداً غیرفعال‌اند. تا زمانی که مسیر Controller/Service و سناریوهای ذخیره‌سازی در پروژه‌ی جدید ایجاد و اعتبارسنجی نشده‌اند، برای موارد زیر هیچ تغییر CRUD در نسخه‌ی جدید ادعا نمی‌شود:

- مازاد تبادل‌شده/معامله‌ی واقعی (مدیریت معامله در این منوها فعال نشده است).
- نسخه و تغییر رمز.

با فعال‌سازی هر مورد، پیش از اعلام «فعال»، این سند باید قراردادهای خواندن/نوشتن، ستون‌ها، تراکنش‌های چندجدولی، مالکیت و تفاوت Legacy را ثبت کند.
