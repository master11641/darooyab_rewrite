# راهنمای منوهای پنل مدیریت داروخانه و اثر آن‌ها بر داده

این سند مرجع زنده‌ی رفتار منوهای پنل داروخانه در `darooyab_rewrite` است. برای هر منو، مسیر، کارکرد، Entity/جدول‌های خوانده‌شده و تغییراتی که عملیات ثبت، ویرایش، حذف یا خاتمه روی داده ایجاد می‌کنند ثبت می‌شود.

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
| داشبورد | `/Admin/Dashboard` | فعال؛ نمایش اطلاعات داروخانه، بدون ثبت/ویرایش داده |
| ثبت موجودی داروخانه (کمبودهای دارویی) | `/PharmacyRequest/AddOrEdit` | فعال؛ اعلام موجودی برای ارجاع بیمار، خرید رتبه و به‌روزرسانی آخرین قیمت برند |
| ثبت کسری داروخانه | `/DrugShortage/AddOrEdit` | فعال؛ ثبت نیاز دارویی و مشاهده‌ی مازادهای منطبق |
| مازاد بر نیاز جهت تبادل | `/PharmacyRequest/TabadolManage` | فعال؛ ایجاد/ویرایش مازاد پایه، ویرایش انقضا، حذف یا خاتمه |
| کیف پول | `/Wallet` | فعال؛ مانده، جمع شارژ/هزینه، تاریخچه‌ی صفحه‌بندی‌شده و درگاه شارژِ مشروط به پیکربندی |
| رتبه‌های خریداری‌شده، مازاد تبادل‌شده، اقلام تاریخ‌نزدیک سایرین، مدیریت اطلاعات داروخانه، کمک به تکمیل برندها، مشاهده نسخه، تازه‌های شرکت پخش، تغییر کلمه عبور | بدون مسیر عملیاتی در سایدبار فعلی | غیرفعال/منتقل‌نشده؛ این برچسب‌ها قابلیت CRUD قابل استفاده ایجاد نمی‌کنند |
| ثبت کسری داروخانه در گروه «سایر امکانات قدیمی» | بدون مسیر (Placeholder) | نسخه‌ی تکراریِ غیرفعال؛ منوی فعال همان لینک بالاتر به `/DrugShortage/AddOrEdit` است |
| وب‌سایت دارویاب، آموزش تصویری | لینک بیرونی/عمومی | Navigation است؛ تغییری در داده‌های پایگاه‌داده ندارد |

---

## ۱. داشبورد

**مسیر و پیاده‌سازی:** `GET /Admin/Dashboard`، [DashboardController](./Areas/Admin/Controllers/DashboardController.cs)، [View داشبورد](./Areas/Admin/Views/Dashboard/Index.cshtml).

**کارکرد:** خوشامدگویی با نام داروخانه و کاربر. اعداد آماری، نمودارها و فعالیت‌های فاقد منبع واقعی Placeholder هستند و به‌عنوان داده‌ی جاری نمایش داده نمی‌شوند.

**جدول‌ها و فیلدهای خوانده‌شده:**

- `PharmacyAccount`: فیلتر `Id`، `AccountStatus == "2"`، `DarooyabAdminApproved != false` و `AccountExpireDate` معتبر؛ از رابطه‌ی `PharmacyIdFk` فقط نام داروخانه از `Pharmacy.Name` خوانده می‌شود.
- اطلاعات UserName از هویت جاری (Claim) خوانده می‌شود، نه از query حساب جدید.

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

## ۴. مازاد بر نیاز جهت تبادل

**مسیر و پیاده‌سازی:** `GET /PharmacyRequest/TabadolManage`، فهرست جزئی `GET /PharmacyRequest/TabadolManage/List`، ثبت `POST /PharmacyRequest/TabadolManage`، ویرایش تاریخ `POST /PharmacyRequest/TabadolManage/{id}/Expiry`، حذف `POST /PharmacyRequest/TabadolManage/{id}/Delete` و خاتمه `POST /PharmacyRequest/TabadolManage/{id}/Finish`. Controller مشترک: [PharmacyRequestController](./Areas/Admin/Controllers/PharmacyRequestController.cs)، منطق داده: [PharmacyExchangeService](./Services/PharmacyRequests/PharmacyExchangeService.cs).

**کارکرد:** ایجاد یا مدیریت اعلام مازاد دارو برای تبادل. در نسخه‌ی فعلی فرم پایه فقط برند، نوع مازاد و ماه/سال انقضا را می‌گیرد؛ فیلدهای تعداد، قیمت و سایر اطلاعات فرم کامل در UI پنهان/غیرفعال‌اند و ثبت پایه آن‌ها را مقدار موجودی قابل‌تبادل نمی‌کند.

**جدول‌ها/داده‌های خواندنی:**

- `PharmacyRequest`: مازادهای دسته‌ی `excess_drugs`، مالک جاری، وضعیت باز؛ فهرست و جستجو بر اساس IRC/نام برند.
- `Category`: دسته‌ی `excess_drugs` و اطلاعات داروخانه/شهر برای query.
- `DrugsNames`: بررسی برند، نام، IRC و منع برندهای `DarooyabBrandRestrictions`.
- `PharmacyExchange`: خواندن تعداد تبادل‌شده برای فهرست/محافظت از حذف مازادی که معامله دارد.
- `PharmacyAccount` و `Pharmacy`: اعتبارسنجی حساب، نمایش نام مالک و محدوده‌ی اطلاعات داروخانه.

**ثبت پایه‌ی `SaveBasicAsync` در `PharmacyRequest`:**

- اگر برای IRC درخواست باز موجود باشد، همان ردیف به‌روزرسانی می‌شود و فقط `ExchangeLimitNum`، `DrugExpireDate` و `RegistrationDateTime` تغییر می‌کنند.
- اگر ردیف باز موجود نباشد، ردیف جدید درج می‌شود با `PharmacyAccountIdFk`، `RequestTypeFromCategoryIdFk`، `Irc`، `ExchangeRandomCode`، `ExchangeLimitNum`، `DrugCount = 0`، `DrugUnitNum = null`، `DrugExpireDate`، `RegistrationDateTime` و `StatusNum = "1"`.
- عملیات پایه **تعداد، واحد، قیمت، قیمت همکار، سری ساخت، شماره فاکتور، تخفیف یا تسهیلات را تنظیم نمی‌کند**. در نتیجه از ایجاد ردیف جدید، موجودی مثبت یا قابل‌تبادل نتیجه نمی‌شود. فهرست ممکن است فیلدهای موجودِ این ستون‌ها را نمایش دهد اما این فرم پایه آن‌ها را تغییر نمی‌دهد.
- ورودی چندبرندی به ازای هر IRC جداگانه upsert می‌شود؛ عملیات در تراکنش Serializable انجام می‌شود.

**ویرایش تاریخ انقضا:** فقط `PharmacyRequest.DrugExpireDate` برای رکورد باز متعلق به همان مالک/دسته تغییر می‌کند؛ تاریخ به آخرین روز ماه انتخاب‌شده تنظیم می‌شود. سایر فیلدها دست‌نخورده می‌مانند.

**حذف:** ردیف مازاد از `PharmacyRequest` حذف می‌شود، فقط اگر متعلق به مالک/دسته‌ی صحیح، باز و بدون ردیف وابسته در `PharmacyExchange` باشد. در غیر این صورت حذف رد می‌شود؛ داده‌ی Exchange حذف آبشاری یا بازنویسی نمی‌شود.

**خاتمه:** فقط `PharmacyRequest.StatusNum` به مقدار وضعیت بسته‌شده توسط داروخانه (`"2"` در سرویس فعلی) تغییر می‌کند. رکورد فیزیکی حذف نمی‌شود و فیلدهای تعداد/قیمت/انقضا تغییر نمی‌کنند.

## ۵. کیف پول

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

## منوهای غیرفعال/فاقد قابلیت عملیاتی

مقادیر وضعیت در سایدبار برای جلوگیری از نمایش قابلیت پیاده‌سازی‌نشده عمداً غیرفعال‌اند. تا زمانی که مسیر Controller/Service و سناریوهای ذخیره‌سازی در پروژه‌ی جدید ایجاد و اعتبارسنجی نشده‌اند، برای موارد زیر هیچ تغییر CRUD در نسخه‌ی جدید ادعا نمی‌شود:

- رتبه‌های خریداری‌شده به‌عنوان منوی مستقل (خلاصه‌ی رتبه‌ها فقط در جزئیات برند AddOrEdit نمایش داده می‌شود).
- مازاد تبادل‌شده/معامله‌ی واقعی (مدیریت معامله در این منوها فعال نشده است).
- صفحه‌ی مستقل اقلام تاریخ‌نزدیک سایرین.
- مدیریت اطلاعات داروخانه، تکمیل برند، نسخه، تازه‌های شرکت پخش و تغییر رمز.

با فعال‌سازی هر مورد، پیش از اعلام «فعال»، این سند باید قراردادهای خواندن/نوشتن، ستون‌ها، تراکنش‌های چندجدولی، مالکیت و تفاوت Legacy را ثبت کند.
