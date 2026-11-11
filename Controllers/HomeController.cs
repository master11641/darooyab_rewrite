#nullable enable

using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Threading.RateLimiting;
using darooyab.Models;
using Ganss.Xss;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using darooyab_rewrite.Extensions;
using darooyab_rewrite.Routing;
using darooyab_rewrite.Security;
using darooyab_rewrite.Services.Captcha;
using darooyab_rewrite.Services.Comments;
using darooyab_rewrite.Utilities;
using darooyab_rewrite.ViewModels;
using darooyab.Models.DTOs;

namespace darooyab_rewrite.Controllers;

public class HomeController : Controller
{
    private readonly darooConContext _context;
    private readonly ILogger<HomeController> _logger;
    private readonly ICaptchaService _captchaService;
    private readonly IPublicCommentService _publicCommentService;

    private static readonly IReadOnlyDictionary<string, SiteSectionViewModel> Sections =
        new Dictionary<string, SiteSectionViewModel>(StringComparer.OrdinalIgnoreCase)
        {
            ["drugs"] = new(
                "اطلاعات دارویی",
                "در این بخش، جست‌وجو و مشاهده‌ی اطلاعات داروها در دسترس خواهد بود.",
                "پایگاه اطلاعات دارویی",
                "✚"),
            ["shortages"] = new(
                "داروهای کمیاب",
                "امکانات مربوط به مشاهده‌ی داروهای کمیاب در این بخش آماده خواهد شد.",
                "دسترسی آسان‌تر",
                "＋"),
            ["news"] = new(
                "تازه‌های دارویی",
                "مطالب و خبرهای دارویی پس از آماده‌شدن سرویس محتوا در این بخش نمایش داده می‌شوند.",
                "مجله دارویاب",
                "▤")
        };

    public HomeController(
        darooConContext context,
        ILogger<HomeController> logger,
        ICaptchaService captchaService,
        IPublicCommentService publicCommentService)
    {
        _context = context;
        _logger = logger;
        _captchaService = captchaService;
        _publicCommentService = publicCommentService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "مرجع اطلاعات دارویی";
        var newsUnavailable = false;
        IReadOnlyList<LatestNewsItem> latestNews = [];

        try
        {
            var newsItems = await _context.DarooyabAppNews
                .AsNoTracking()
                .Where(item => item.IsActive)
                .OrderByDescending(item => item.OrderIndex)
                .Take(5)
                .Select(item => new
                {
                    item.Id,
                    item.Title,
                    item.SubTitle,
                    item.ImageUrl,
                    item.ShowDate
                })
                .ToListAsync(cancellationToken);

            latestNews = newsItems
                .Select(item => new LatestNewsItem(
                    item.Id,
                    item.Title,
                    item.SubTitle,
                    item.ImageUrl,
                    null,
                    item.ShowDate.HasValue
                        ? PersianDateFormatter.Format(item.ShowDate.Value)
                        : null))
                .ToArray();
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load homepage news from SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);
            newsUnavailable = true;
        }

        return View(new HomePageViewModel(latestNews, newsUnavailable));
    }

    [HttpGet("/App", Name = "App")]
    public async Task<IActionResult> App(CancellationToken cancellationToken)
    {
        try
        {
            var page = await _context.TblNews
                .AsNoTracking()
                .Where(item =>
                    item.TblNewsId == 14 &&
                    item.TblNewsTypeIsPage &&
                    item.TblNewsIsActive &&
                    item.TblNewsDoConfirmByManager != false)
                .Select(item => new
                {
                    item.TblNewsTitle,
                    item.TblNewsSubTitle,
                    item.TblNewsShortDescription,
                    item.TblNewsLongDescription
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (page is null)
            {
                return NotFound();
            }

            var sanitizer = new HtmlSanitizer();
            var legacyContent = string.IsNullOrWhiteSpace(page.TblNewsLongDescription)
                ? page.TblNewsShortDescription ?? string.Empty
                : page.TblNewsLongDescription;
            var contentHtml = sanitizer.Sanitize(PrepareLegacyAppContent(legacyContent));
            var description = Regex.Replace(
                WebUtility.HtmlDecode(page.TblNewsSubTitle ?? string.Empty),
                @"\s+",
                " ").Trim();
            if (string.IsNullOrWhiteSpace(description))
            {
                description = "معرفی اپلیکیشن دارویاب.";
            }

            SetAppMetadata(page.TblNewsTitle, page.TblNewsSubTitle, description);
            return View(new AppPageViewModel(
                page.TblNewsTitle,
                page.TblNewsSubTitle,
                contentHtml));
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load the legacy app page from SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            SetAppMetadata(
                "اپلیکیشن دارویاب",
                null,
                "محتوای معرفی اپلیکیشن دارویاب در حال حاضر در دسترس نیست.");
            return View(new AppPageViewModel(
                "اپلیکیشن دارویاب",
                null,
                string.Empty,
                DataUnavailable: true));
        }
    }

    [HttpGet("/Contactus", Name = "ContactUs")]
    public async Task<IActionResult> Contactus(CancellationToken cancellationToken)
    {
        SetContactMetadata();

        try
        {
            var activities = await LoadContactActivitiesAsync(cancellationToken);
            return View(new ContactPageViewModel(
                activities,
                new ContactMessageInput(),
                MessageSent: TempData["ContactMessageSent"] is bool sent && sent));
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load contact activity categories from SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return View(new ContactPageViewModel([], new ContactMessageInput(), DataUnavailable: true));
        }
    }

    [HttpGet("/About", Name = "About")]
    public IActionResult About()
    {
        const string title = "درباره ما";
        const string description =
            "با دارویاب آشنا شوید؛ بستری برای جست‌وجو و مشاهده‌ی اطلاعات دارویی و دسترسی به بخش‌های مختلف خدمات دارویی.";
        ViewData["Title"] = title;
        ViewData["MetaDescription"] = description;
        ViewData["CanonicalUrl"] = "https://www.darooyab.ir/About";
        ViewData["OpenGraphUrl"] = ViewData["CanonicalUrl"];
        ViewData["OpenGraphTitle"] = title;
        ViewData["OpenGraphDescription"] = description;
        ViewData["OpenGraphType"] = "website";

        return View();
    }

    [HttpPost("/Contactus")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("contact-submission")]
    public async Task<IActionResult> SubmitContactus(
        ContactMessageInput input,
        CancellationToken cancellationToken)
    {
        SetContactMetadata();

        CaptchaValidationResult captchaResult;
        try
        {
            captchaResult = await _captchaService.ValidateAsync(
                input.CaptchaChallengeId,
                input.CaptchaAnswer,
                CaptchaClientCookie.GetOrCreate(HttpContext),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                cancellationToken);
        }
        catch (CaptchaStoreUnavailableException exception)
        {
            _logger.LogError(exception, "The CAPTCHA challenge store is unavailable.");
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            IReadOnlyList<ContactActivityOption> unavailableActivities = [];
            try
            {
                unavailableActivities = await LoadContactActivitiesAsync(cancellationToken);
            }
            catch (SqlException sqlException)
            {
                _logger.LogError(
                    "Could not load contact activity categories after CAPTCHA store failure. SQL error number: {SqlErrorNumber}.",
                    sqlException.Number);
            }

            return View("Contactus", new ContactPageViewModel(
                unavailableActivities,
                input,
                DataUnavailable: unavailableActivities.Count == 0,
                SubmissionUnavailable: true));
        }

        IReadOnlyList<ContactActivityOption> activities;
        try
        {
            activities = await LoadContactActivitiesAsync(cancellationToken);
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load contact activity categories for submission. SQL error number: {SqlErrorNumber}.",
                exception.Number);
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return View("Contactus", new ContactPageViewModel(
                [],
                input,
                DataUnavailable: true,
                SubmissionUnavailable: true));
        }

        if (!captchaResult.IsValid)
        {
            ModelState.AddModelError(
                "Input.CaptchaAnswer",
                "کد امنیتی معتبر نیست یا منقضی شده است. کد تازه را دریافت کنید.");
            return View("Contactus", new ContactPageViewModel(activities, input));
        }

        if (!string.IsNullOrWhiteSpace(input.Website))
        {
            ModelState.AddModelError(nameof(input.Website), "درخواست قابل ثبت نیست.");
        }

        if (!activities.Any(activity => activity.Id == input.ActivityId))
        {
            ModelState.AddModelError(nameof(input.ActivityId), "نوع فعالیت معتبر نیست.");
        }

        if (!ModelState.IsValid)
        {
            return View("Contactus", new ContactPageViewModel(activities, input));
        }

        var remoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (remoteIpAddress?.Length > 25)
        {
            remoteIpAddress = null;
        }

        var contactMessage = new TblContactu
        {
            TblContactusNameAndFamily = input.Name?.Trim() ?? string.Empty,
            TblContactusCategoryIdFk = input.ActivityId,
            TblContactusOrganizationOrCompanyName = input.Organization?.Trim() ?? string.Empty,
            TblContactusRole = input.Role?.Trim() ?? string.Empty,
            TblContactusTel = input.Phone?.Trim() ?? string.Empty,
            TblContactusEmail = input.Email?.Trim() ?? string.Empty,
            TblContactusTextMessage = input.Message.Trim(),
            TblContactusViewByManagerStatusNum = 2,
            TblContactusIpsystem = remoteIpAddress ?? string.Empty,
            TblContactusRegisterDate = DateTime.Now.Date,
            TblContactusViewByManagerDate = null
        };

        try
        {
            _context.TblContactus.Add(contactMessage);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not save contact message to SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return View("Contactus", new ContactPageViewModel(
                activities,
                input,
                SubmissionUnavailable: true));
        }

        TempData["ContactMessageSent"] = true;
        return RedirectToRoute("ContactUs");
    }

    private void SetAppMetadata(string title, string? subtitle, string description)
    {
        var pageTitle = string.IsNullOrWhiteSpace(subtitle)
            ? title
            : $"{title} | {subtitle}";
        const string canonicalUrl = "https://www.darooyab.ir/App";
        ViewData["Title"] = pageTitle;
        ViewData["MetaDescription"] = description;
        ViewData["CanonicalUrl"] = canonicalUrl;
        ViewData["OpenGraphTitle"] = pageTitle;
        ViewData["OpenGraphDescription"] = description;
        ViewData["OpenGraphUrl"] = canonicalUrl;
        ViewData["OpenGraphType"] = "website";
    }

    private static string PrepareLegacyAppContent(string content)
    {
        const string imageTagPattern = @"<img\b[^>]*>";
        const string imageAltPattern = @"\balt\s*=\s*(['""])(.*?)\1";

        var contentWithAccessibleLinks = Regex.Replace(
            content,
            @"<a\b(?<attributes>[^>]*)>(?<content>.*?)</a>",
            match =>
            {
                var linkContent = match.Groups["content"].Value;
                if (!Regex.IsMatch(linkContent, @"<img\b", RegexOptions.IgnoreCase))
                {
                    return match.Value;
                }

                var altMatch = Regex.Match(linkContent, imageAltPattern, RegexOptions.IgnoreCase);
                var label = altMatch.Success
                    ? WebUtility.HtmlDecode(altMatch.Groups[2].Value).Trim()
                    : string.Empty;
                if (string.IsNullOrWhiteSpace(label))
                {
                    var hrefMatch = Regex.Match(
                        match.Groups["attributes"].Value,
                        @"\bhref\s*=\s*(['""])(.*?)\1",
                        RegexOptions.IgnoreCase);
                    var href = hrefMatch.Success
                        ? WebUtility.HtmlDecode(hrefMatch.Groups[2].Value)
                        : string.Empty;
                    label = href.Contains("Darooyab32.apk", StringComparison.OrdinalIgnoreCase)
                        ? "دریافت نسخه‌ی ۳۲ بیتی اندروید"
                        : href.Contains("cafebazaar.ir", StringComparison.OrdinalIgnoreCase)
                            ? "دریافت از کافه‌بازار"
                            : href.Contains("myket.ir", StringComparison.OrdinalIgnoreCase)
                                ? "دریافت از مایکت"
                                : href.Contains("app.darooyab.ir", StringComparison.OrdinalIgnoreCase)
                                    ? "استفاده از نسخه‌ی وب اپلیکیشن"
                                    : "دریافت اپلیکیشن دارویاب";
                }

                var remainingContent = Regex.Replace(
                    linkContent,
                    imageTagPattern,
                    string.Empty,
                    RegexOptions.IgnoreCase).Trim();
                var imageLabel = $"<span class=\"app-page-image-label\">{WebUtility.HtmlEncode(label)}</span>";
                return $"<a{match.Groups["attributes"].Value}>{remainingContent}{imageLabel}</a>";
            },
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        return Regex.Replace(
            contentWithAccessibleLinks,
            imageTagPattern,
            match =>
            {
                var altMatch = Regex.Match(match.Value, imageAltPattern, RegexOptions.IgnoreCase);
                var label = altMatch.Success
                    ? WebUtility.HtmlDecode(altMatch.Groups[2].Value).Trim()
                    : string.Empty;
                return string.IsNullOrWhiteSpace(label)
                    ? string.Empty
                    : $"<span class=\"app-page-image-label\">{WebUtility.HtmlEncode(label)}</span>";
            },
            RegexOptions.IgnoreCase);
    }

    private async Task<IReadOnlyList<ContactActivityOption>> LoadContactActivitiesAsync(
        CancellationToken cancellationToken) =>
        await _context.Categories
            .AsNoTracking()
            .Where(category => category.Type == "Activity")
            .Select(category => new ContactActivityOption(
                category.CatId,
                category.Name ?? string.Empty))
            .ToListAsync(cancellationToken);

    private void SetContactMetadata()
    {
        ViewData["CaptchaEnabled"] = _captchaService.IsEnabled;
        const string title =
            "اطلاعات تماس دارویاب|تلفن دارویاب|ارتباط با دارویاب|تلفن اطلاعات دارویی|شرکت دانش بنیان اکسیر دانش آسیا";
        const string description =
            "راه‌های ارتباط با دارویاب: تماس تلفنی، رایانامه و پیام‌رسان‌ها؛ همچنین ارسال پیام به تیم دارویاب.";
        ViewData["Title"] = title;
        ViewData["MetaDescription"] = description;
        ViewData["CanonicalUrl"] = "https://www.darooyab.ir/Contactus";
        ViewData["OpenGraphUrl"] = ViewData["CanonicalUrl"];
        ViewData["OpenGraphTitle"] = title;
        ViewData["OpenGraphDescription"] = description;
        ViewData["OpenGraphType"] = "website";
    }

    public async Task<IActionResult> Section(
        string? id,
        string? q,
        int? articleId,
        CancellationToken cancellationToken,
        int page = 1,
        int pageSize = 10)
    {
        if (string.Equals(id, "pharmacies", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToRoute("PharmacyDirectory");
        }

        if (string.IsNullOrWhiteSpace(id) || !Sections.TryGetValue(id, out var section))
        {
            return NotFound();
        }

        if (string.Equals(id, "drugs", StringComparison.OrdinalIgnoreCase) && !articleId.HasValue)
        {
            return await DrugDirectory(cancellationToken);
        }

        if (string.Equals(id, "news", StringComparison.OrdinalIgnoreCase) && !articleId.HasValue)
        {
            ViewData["Title"] = "اخبار";
            ViewData["UseLegacyNewsMetadata"] = true;
            ViewData["CanonicalUrl"] =
                $"{Request.Scheme}://{Request.Host}{Request.PathBase}{Request.Path}";

            try
            {
                var pagedNews = await _context.DarooyabAppNews
                    .AsNoTracking()
                    .Where(item => item.IsActive)
                    .OrderByDescending(item => item.OrderIndex)
                    .Select(item => new LatestNewsItem(
                        item.Id,
                        item.Title,
                        item.SubTitle,
                        item.ImageUrl,
                        null,
                        null))
                    .ToPagedResultAsync(page, pageSize, cancellationToken);

                return View("News", new NewsPageViewModel(pagedNews, DataUnavailable: false));
            }
            catch (SqlException exception)
            {
                _logger.LogError(
                    "Could not load news list from SQL Server. SQL error number: {SqlErrorNumber}.",
                    exception.Number);
                return View("News", new NewsPageViewModel(
                    PagedResult<LatestNewsItem>.Empty(Math.Clamp(pageSize, 1, 100)),
                    DataUnavailable: true));
            }
        }

        if (articleId.HasValue)
        {
            if (!string.Equals(id, "news", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }

            try
            {
                var article = await _context.DarooyabAppNews
                    .AsNoTracking()
                    .Where(item => item.IsActive && item.Id == articleId.Value)
                    .Select(item => new
                    {
                        item.Id,
                        item.Title,
                        item.SubTitle,
                        item.ImageUrl,
                        item.Description,
                        item.ShowDate,
                        item.ChangeDate,
                        item.InsertDate,
                        item.ClipUrl
                    })
                    .SingleOrDefaultAsync(cancellationToken);

                if (article is null)
                {
                    return NotFound();
                }

                var sanitizedDescription = article.Description is null
                    ? null
                    : new HtmlSanitizer().Sanitize(article.Description);
                var canonicalUrl = $"https://www.darooyab.ir/NewsApp/{article.Id}";
                var schemaDescription = article.SubTitle ?? string.Empty;
                var schemaImageUrl = ResolveSchemaUrl(article.ImageUrl);
                var schemaClipUrl = ResolveSchemaUrl(article.ClipUrl);
                var articleBody = Regex.Replace(
                    sanitizedDescription ?? string.Empty,
                    "<[^>]*>",
                    " ");
                var newsSchema = new Dictionary<string, object?>
                {
                    ["@context"] = "https://schema.org",
                    ["@type"] = "NewsArticle",
                    ["@id"] = $"{canonicalUrl}#newsarticle",
                    ["mainEntityOfPage"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "WebPage",
                        ["@id"] = canonicalUrl
                    },
                    ["headline"] = article.Title,
                    ["description"] = schemaDescription,
                    ["image"] = schemaImageUrl is null
                        ? Array.Empty<string>()
                        : new[] { schemaImageUrl },
                    ["articleBody"] = articleBody,
                    ["datePublished"] = article.ShowDate?.ToString(
                        "yyyy-MM-dd",
                        System.Globalization.CultureInfo.InvariantCulture),
                    ["dateModified"] = (article.ChangeDate ?? article.InsertDate)?.ToString(
                        "yyyy-MM-dd",
                        System.Globalization.CultureInfo.InvariantCulture),
                    ["inLanguage"] = "fa-IR",
                    ["publisher"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "Organization",
                        ["name"] = "دارویاب",
                        ["url"] = "https://www.darooyab.ir/"
                    }
                };
                if (schemaClipUrl is not null)
                {
                    newsSchema["video"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "VideoObject",
                        ["name"] = article.Title,
                        ["description"] = schemaDescription,
                        ["thumbnailUrl"] = schemaImageUrl,
                        ["uploadDate"] = article.ShowDate?.ToString(
                            "yyyy-MM-dd",
                            System.Globalization.CultureInfo.InvariantCulture),
                        ["contentUrl"] = schemaClipUrl
                    };
                }

                ViewData["CanonicalUrl"] = canonicalUrl;
                ViewData["MetaDescription"] = schemaDescription;
                ViewData["OpenGraphUrl"] = canonicalUrl;
                ViewData["OpenGraphTitle"] = article.Title;
                ViewData["OpenGraphDescription"] = schemaDescription;
                ViewData["OpenGraphType"] = "article";
                ViewData["StructuredData"] = JsonSerializer.Serialize(newsSchema);

                section = section with
                {
                    Article = new LatestNewsItem(
                        article.Id,
                        article.Title,
                        article.SubTitle,
                        article.ImageUrl,
                        sanitizedDescription,
                        article.ShowDate.HasValue
                            ? PersianDateFormatter.Format(article.ShowDate.Value)
                            : null)
                };
            }
            catch (SqlException exception)
            {
                _logger.LogError(
                    "Could not load article {ArticleId} from SQL Server. SQL error number: {SqlErrorNumber}.",
                    articleId.Value,
                    exception.Number);
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
        }

        ViewData["Title"] = section.Article?.Title ?? section.Title;
        ViewData["ActiveSection"] = id;
        ViewData["IsNewsArticle"] = section.Article is not null;
        return View(section with { Query = q });
    }

    private async Task<IActionResult> DrugDirectory(CancellationToken cancellationToken)
    {
        const string title = "اطلاعات دارویی";
        const string description =
            "جست‌وجوی اطلاعات داروها در دارویاب؛ دسترسی به اطلاعات ژنریک و برند، موارد مصرف، هشدارها، تداخلات و شکل‌های دارویی بر پایه‌ی اطلاعات موجود هر دارو.";
        var canonicalUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}{Request.Path}";

        ViewData["Title"] = title;
        ViewData["MetaDescription"] = description;
        ViewData["CanonicalUrl"] = canonicalUrl;
        ViewData["OpenGraphTitle"] = title;
        ViewData["OpenGraphDescription"] = description;
        ViewData["OpenGraphUrl"] = canonicalUrl;
        ViewData["OpenGraphType"] = "website";
        ViewData["TwitterCard"] = "summary";
        ViewData["ActiveSection"] = "drugs";
        ViewData["StructuredData"] = JsonSerializer.Serialize(new Dictionary<string, string>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "CollectionPage",
            ["name"] = title,
            ["description"] = description,
            ["url"] = canonicalUrl
        });

        try
        {
            var popularDrugRows = await _context.DrugsNames
                .AsNoTracking()
                .Where(item =>
                    item.AddDate.HasValue &&
                    item.AddDate < 1000 &&
                    item.DrugNameLanguage == false)
                .Join(
                    _context.DrugsNames.AsNoTracking().Where(item => item.DrugNameType == "A"),
                    drug => drug.DrugId,
                    generic => generic.DrugId,
                    (drug, generic) => new
                    {
                        drug.DrugId,
                        PersianName = drug.DrugName,
                        EnglishName = generic.DrugName
                    })
                .OrderByDescending(item => item.DrugId)
                .Take(6)
                .ToListAsync(cancellationToken);

            var newestBrandRows = await (
                from persianBrand in _context.DrugsNames.AsNoTracking()
                where persianBrand.Irc != null &&
                      persianBrand.DrugNameLanguage == false &&
                      persianBrand.IsNewestDrug == true
                join englishBrand in _context.DrugsNames.AsNoTracking()
                    .Where(item => item.DrugNameLanguage == true)
                    on persianBrand.Irc equals englishBrand.Irc
                orderby persianBrand.Id descending
                select new
                {
                    Irc = persianBrand.Irc!.Value,
                    PersianName = persianBrand.DrugName,
                    EnglishName = englishBrand.DrugName
                })
                .Take(6)
                .ToListAsync(cancellationToken);

            var popularDrugs = popularDrugRows
                .Where(item =>
                    !string.IsNullOrWhiteSpace(item.PersianName) &&
                    !string.IsNullOrWhiteSpace(item.EnglishName))
                .Select(item => new DrugDirectoryItem(
                    item.DrugId,
                    item.PersianName!,
                    item.EnglishName!,
                    null))
                .ToArray();
            var newestBrands = newestBrandRows
                .Where(item =>
                    !string.IsNullOrWhiteSpace(item.PersianName) &&
                    !string.IsNullOrWhiteSpace(item.EnglishName))
                .Select(item => new DrugDirectoryItem(
                    null,
                    item.PersianName!,
                    item.EnglishName!,
                    item.Irc))
                .ToArray();

            return View("DrugDirectory", new DrugDirectoryViewModel(
                popularDrugs,
                newestBrands,
                DataUnavailable: false));
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load drug directory highlights from SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);
            return View("DrugDirectory", new DrugDirectoryViewModel([], [], DataUnavailable: true));
        }
    }

    [HttpGet("/DrugGroups", Name = "DrugGroups")]
    public async Task<IActionResult> DrugGroups(CancellationToken cancellationToken)
    {
        const string title =
            "طبقه بندی دارویی|گروه دارویی|دسته بندی داروها|دسته های دارویی|دارویاب";
        const string description = "فهرست گروه‌های دارویی مارتیندل و داروهای مرتبط با هر گروه در دارویاب.";
        ViewData["Title"] = title;
        ViewData["MetaDescription"] = description;
        ViewData["CanonicalUrl"] = "https://www.darooyab.ir/DrugGroups";
        ViewData["OpenGraphUrl"] = ViewData["CanonicalUrl"];
        ViewData["OpenGraphTitle"] = title;
        ViewData["OpenGraphDescription"] = description;
        ViewData["OpenGraphType"] = "website";

        try
        {
            var groupRows = await _context.Groups1
                .AsNoTracking()
                .Select(group => new
                {
                    group.GroupId,
                    group.GroupName,
                    group.GroupNameFa
                })
                .ToListAsync(cancellationToken);
            var groups = groupRows
                .Select(group => CreateDrugGroupDirectoryItem(
                    group.GroupId,
                    group.GroupName,
                    group.GroupNameFa))
                .ToArray();

            return View(new DrugGroupDirectoryViewModel(groups));
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load drug groups from SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);

            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return View(new DrugGroupDirectoryViewModel([], DataUnavailable: true));
        }
    }

    [HttpGet("/DrugGroups/{groupId:int}", Name = "DrugGroupDetails")]
    public async Task<IActionResult> DrugGroupDetails(int groupId, CancellationToken cancellationToken)
    {
        if (groupId is < byte.MinValue or > byte.MaxValue)
        {
            return NotFound();
        }

        try
        {
            var groupRow = await _context.Groups1
                .AsNoTracking()
                .Where(item => item.GroupId == (byte)groupId)
                .Select(item => new
                {
                    item.GroupId,
                    item.GroupName,
                    item.GroupNameFa
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (groupRow is null)
            {
                return NotFound();
            }

            var group = CreateDrugGroupDirectoryItem(
                groupRow.GroupId,
                groupRow.GroupName,
                groupRow.GroupNameFa);

            var drugs = await (
                from genericInfo in _context.DrugsGenericInfos.AsNoTracking()
                join generic in _context.DrugsNames.AsNoTracking()
                    on genericInfo.DrugId equals (int?)generic.DrugId
                join ahfsGroup in _context.Ahfsgroups.AsNoTracking()
                    on genericInfo.AhfsId equals (int?)ahfsGroup.Id
                join persianName in _context.DrugsNames.AsNoTracking()
                    .Where(item => item.DrugNameType == "F" && item.DrugNameLanguage == false)
                    on generic.DrugId equals persianName.DrugId into persianNames
                from persianName in persianNames.DefaultIfEmpty()
                join brandAttribute in _context.TblBrandAttributes.AsNoTracking()
                    on generic.Irc equals (long?)brandAttribute.TblBrandAttributeIrc into brandAttributes
                where genericInfo.GroupId == groupId &&
                      generic.DrugNameType == "A" &&
                      (groupId != 52 ||
                       !(generic.DrugName ?? string.Empty).Contains("+"))
                select new DrugGroupDrugItem(
                    generic.DrugId,
                    generic.DrugName ?? string.Empty,
                    persianName == null ? null : persianName.DrugName,
                    brandAttributes
                        .Select(attribute => attribute.Image1Url)
                        .FirstOrDefault()))
                .ToListAsync(cancellationToken);

            var title = $"{group.EnglishName} | {group.PersianName}";
            var description = $"فهرست داروهای گروه {group.PersianName} ({group.EnglishName}) در دارویاب.";
            ViewData["Title"] = title;
            ViewData["MetaDescription"] = description;
            ViewData["CanonicalUrl"] = $"https://www.darooyab.ir/DrugGroups/{group.Id}";
            ViewData["OpenGraphUrl"] = ViewData["CanonicalUrl"];
            ViewData["OpenGraphTitle"] = title;
            ViewData["OpenGraphDescription"] = description;
            ViewData["OpenGraphType"] = "website";

            return View("DrugsInGroup", new DrugGroupDetailsViewModel(group, drugs));
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load drug group {DrugGroupId} from SQL Server. SQL error number: {SqlErrorNumber}.",
                groupId,
                exception.Number);

            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return View("DrugsInGroup", new DrugGroupDetailsViewModel(
                new DrugGroupDirectoryItem((byte)groupId, string.Empty, "گروه دارویی"),
                [],
                DataUnavailable: true));
        }
    }

    [HttpGet("/HerbalMedicines", Name = "HerbalMedicines")]
    public async Task<IActionResult> HerbalMedicines(CancellationToken cancellationToken)
    {
        const string title = "داروهای گیاهی|گیاهان دارویی|طب سنتی|طب اسلامی|خواص گیاهان دارویی";
        const string description = "فهرست داروهای گیاهی دارای IRC در دارویاب، همراه با نام فارسی و انگلیسی، تصویر و تولیدکننده در صورت ثبت.";
        ViewData["Title"] = title;
        ViewData["MetaDescription"] = description;
        ViewData["CanonicalUrl"] = "https://www.darooyab.ir/HerbalMedicines";
        ViewData["OpenGraphUrl"] = ViewData["CanonicalUrl"];
        ViewData["OpenGraphTitle"] = title;
        ViewData["OpenGraphDescription"] = description;
        ViewData["OpenGraphType"] = "website";

        try
        {
            var groupBrands = _context.DrugsNames
                .AsNoTracking()
                .Where(brand =>
                    brand.Irc != null &&
                    brand.DrugNameType == "T")
                .Join(
                    _context.DrugsGenericInfos
                        .AsNoTracking()
                        .Where(genericInfo => genericInfo.GroupId == 52),
                    brand => brand.DrugId,
                    genericInfo => genericInfo.DrugId,
                    (brand, _) => brand);
            var englishBrands = groupBrands
                .Where(brand => brand.DrugNameLanguage == true);
            var persianBrands = groupBrands
                .Where(brand => brand.DrugNameLanguage == false);

            var herbalMedicines = await (
                from englishBrand in englishBrands
                join persianBrand in persianBrands
                    on englishBrand.Irc equals persianBrand.Irc into persianBrandsByIrc
                from persianBrand in persianBrandsByIrc.DefaultIfEmpty()
                join genericAttribute in _context.GenericInfoAttributes.AsNoTracking()
                    on (int?)englishBrand.DrugId equals genericAttribute.DrugId into genericAttributes
                from genericAttribute in genericAttributes.DefaultIfEmpty()
                join ahfsGroup in _context.Ahfsgroups.AsNoTracking()
                    on (genericAttribute == null ? null : genericAttribute.Ahfscode) equals ahfsGroup.Ahfscode
                    into ahfsGroups
                from ahfsGroup in ahfsGroups.DefaultIfEmpty()
                select new DrugGroupDrugItem(
                    englishBrand.DrugId,
                    englishBrand.DrugName ?? string.Empty,
                    persianBrand == null ? null : persianBrand.DrugName,
                    _context.TblBrandAttributes
                        .Where(attribute => attribute.TblBrandAttributeIrc == englishBrand.Irc)
                        .Select(attribute => attribute.Image1Url)
                        .FirstOrDefault(),
                    englishBrand.Irc,
                    englishBrand.Factory == null ? null : englishBrand.Factory!.FactoryName))
                .ToListAsync(cancellationToken);

            return View("DrugsInGroup", new DrugGroupDetailsViewModel(
                new DrugGroupDirectoryItem(52, string.Empty, "دارو های گیاهی"),
                herbalMedicines,
                IsHerbalMedicines: true));
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load herbal medicines from SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return View("DrugsInGroup", new DrugGroupDetailsViewModel(
                new DrugGroupDirectoryItem(52, string.Empty, "دارو های گیاهی"),
                [],
                DataUnavailable: true,
                IsHerbalMedicines: true));
        }
    }

    public async Task<IActionResult> DrugHerbal(CancellationToken cancellationToken)
    {
        const string title = "گیاهان دارویی";
        const string description = "فهرست گیاهان دارویی همراه با نام فارسی و نام علمی ثبت‌شده در اطلاعات دارویی دارویاب.";
        ViewData["Title"] = title;
        ViewData["MetaDescription"] = description;
        ViewData["CanonicalUrl"] = "https://www.darooyab.ir/DrugHerbal";
        ViewData["OpenGraphUrl"] = ViewData["CanonicalUrl"];
        ViewData["OpenGraphTitle"] = title;
        ViewData["OpenGraphDescription"] = description;
        ViewData["OpenGraphType"] = "website";

        try
        {
            var groupPlants =
                from genericInfo in _context.DrugsGenericInfos.AsNoTracking()
                join generic in _context.DrugsNames.AsNoTracking()
                    on genericInfo.DrugId equals (int?)generic.DrugId
                where generic.DrugNameType == "A"
                join persianName in _context.DrugsNames.AsNoTracking()
                    .Where(item => item.DrugNameType == "F" && item.DrugNameLanguage == false)
                    on generic.DrugId equals persianName.DrugId into persianNames
                from persianName in persianNames.DefaultIfEmpty()
                join ahfsGroup in _context.Ahfsgroups.AsNoTracking()
                    on genericInfo.AhfsId equals (int?)ahfsGroup.Id
                join brandAttribute in _context.TblBrandAttributes.AsNoTracking()
                    on generic.Irc equals (long?)brandAttribute.TblBrandAttributeIrc into brandAttributes
                where genericInfo.GroupId == 52 &&
                      !(generic.DrugName ?? string.Empty).Contains("+")
                select new DrugGroupDrugItem(
                    generic.DrugId,
                    generic.DrugName ?? string.Empty,
                    persianName == null ? null : persianName.DrugName,
                    brandAttributes
                        .Select(attribute => attribute.Image1Url)
                        .FirstOrDefault());

            var plants = await groupPlants.ToListAsync(cancellationToken);

            return View("DrugsInGroup", new DrugGroupDetailsViewModel(
                new DrugGroupDirectoryItem(52, string.Empty, "گیاهان دارویی"),
                plants,
                IsHerbalPlants: true));
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load herbal plants from SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return View("DrugsInGroup", new DrugGroupDetailsViewModel(
                new DrugGroupDirectoryItem(52, string.Empty, "گیاهان دارویی"),
                [],
                DataUnavailable: true,
                IsHerbalPlants: true));
        }
    }

    private static DrugGroupDirectoryItem CreateDrugGroupDirectoryItem(
        byte id,
        string? groupName,
        string? persianName)
    {
        var name = groupName ?? string.Empty;
        var separatorIndex = name.IndexOf('>');
        var persianNameFromLegacyFormat = separatorIndex >= 0
            ? name[(separatorIndex + 1)..].Trim()
            : string.Empty;

        return new DrugGroupDirectoryItem(
            id,
            name.Split('<')[0].Trim(),
            !string.IsNullOrWhiteSpace(persianNameFromLegacyFormat)
                ? persianNameFromLegacyFormat
                : persianName?.Trim() ?? string.Empty);
    }

    [HttpGet("/داروخانه", Name = "PharmacyDirectory")]
    [HttpGet("/Home/Pharmacies")]
    public async Task<IActionResult> Pharmacies(
        [FromQuery] int provId = 0,
        [FromQuery] int cityId = 0,
        [FromQuery] int districtId = 0,
        [FromQuery] int pharmacySpecialtyId = 0,
        [FromQuery] string? pharmacyName = null,
        [FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
    {
        var cleanPharmacyName = pharmacyName?.Trim() ?? string.Empty;
        if (cleanPharmacyName.Length > 100)
        {
            return BadRequest(new { message = "نام داروخانه نباید بیشتر از ۱۰۰ نویسه باشد." });
        }

        provId = Math.Max(0, provId);
        cityId = Math.Max(0, cityId);
        districtId = Math.Max(0, districtId);
        pharmacySpecialtyId = Math.Max(0, pharmacySpecialtyId);

        const int pageSize = 20;
        ViewData["Title"] = "فهرست داروخانه‌ها";
        ViewData["MetaDescription"] = "جست‌وجو و مشاهده‌ی فهرست داروخانه‌های تأییدشده بر اساس استان، شهر، منطقه، حوزه‌ی دارویی و نام.";

        try
        {
            var provinces = await _context.Categories
                .AsNoTracking()
                .Where(category => category.Type != null && category.Type.ToLower() == "province")
                .OrderBy(category => category.Name)
                .Select(category => new PharmacyFilterOption(category.CatId, category.Name ?? string.Empty))
                .ToListAsync(cancellationToken);

            var specialties = await _context.PharmacySpecialties
                .AsNoTracking()
                .Where(specialty => specialty.IsActive && !specialty.IsDeleted)
                .OrderBy(specialty => specialty.OrderIndex)
                .Select(specialty => new PharmacyFilterOption(specialty.ID, specialty.Title))
                .ToListAsync(cancellationToken);

            var cities = provId > 0
                ? await _context.Categories
                    .AsNoTracking()
                    .Where(city => city.Type == "Township" && city.ParentId == provId)
                    .OrderBy(city => city.Name)
                    .Select(city => new PharmacyFilterOption(city.CatId, city.Name ?? string.Empty))
                    .ToListAsync(cancellationToken)
                : [];

            if (cityId > 0 && !cities.Any(city => city.Id == cityId))
            {
                cityId = 0;
                districtId = 0;
            }

            var districts = cityId > 0
                ? await _context.DarooyabCityDistricts
                    .AsNoTracking()
                    .Where(district => district.CityId == cityId && district.IsActive)
                    .OrderBy(district => district.OrderIndex)
                    .ThenBy(district => district.Name)
                    .Select(district => new PharmacyFilterOption(district.Id, district.Name))
                    .ToListAsync(cancellationToken)
                : [];

            if (districtId > 0 && !districts.Any(district => district.Id == districtId))
            {
                districtId = 0;
            }

            var pharmacyQuery = _context.Pharmacies
                .AsNoTracking()
                .Where(pharmacy => pharmacy.DarooyabAdminApproved == true);

            if (cityId > 0)
            {
                pharmacyQuery = pharmacyQuery.Where(pharmacy => pharmacy.CityIdFk == cityId);
            }
            else if (provId > 0)
            {
                pharmacyQuery = pharmacyQuery.Where(pharmacy => pharmacy.ProvinceIdFk == provId);
            }

            if (districtId > 0)
            {
                pharmacyQuery = pharmacyQuery.Where(pharmacy => pharmacy.DistrictId == districtId);
            }

            if (pharmacySpecialtyId > 0)
            {
                pharmacyQuery = pharmacyQuery.Where(pharmacy => _context.PharmacySpecialtyMaps.Any(map =>
                    map.PharmacyId_fk == pharmacy.Id &&
                    map.PharmacySpecialtyId_fk == pharmacySpecialtyId &&
                    map.IsActive &&
                    !map.IsDeleted));
            }

            if (cleanPharmacyName.Length > 0)
            {
                pharmacyQuery = pharmacyQuery.Where(pharmacy => pharmacy.Name.Contains(cleanPharmacyName));
            }

            var now = DateTime.Now;
            var rankedPharmacies = pharmacyQuery
                .Select(pharmacy => new
                {
                    Pharmacy = pharmacy,
                    ValidRankCount = _context.RankingTransactions.Count(transaction =>
                        transaction.PharmacyId == pharmacy.Id &&
                        transaction.EndDate > now),
                    SumWalletAmount = _context.WalletTransactions
                        .Where(transaction =>
                            transaction.RefId != null &&
                            transaction.PaymentDate != null &&
                            _context.PharmacyAccounts.Any(account =>
                                account.PharmacyIdFk == pharmacy.Id &&
                                account.Id == transaction.AccountId))
                        .Sum(transaction => (int?)transaction.Amount) ?? 0
                })
                .OrderByDescending(item => item.ValidRankCount)
                .ThenByDescending(item => item.SumWalletAmount)
                .ThenBy(item => item.Pharmacy.Name)
                .Select(item => new PharmacyDirectoryItem(
                    item.Pharmacy.Id,
                    item.Pharmacy.Name,
                    item.Pharmacy.CityIdFkNavigation.Name ?? string.Empty,
                    item.Pharmacy.ProvinceIdFkNavigation.Name ?? string.Empty,
                    item.Pharmacy.Address,
                    item.Pharmacy.Phone));

            var pagedPharmacies = await rankedPharmacies.ToPagedResultAsync(page, pageSize, cancellationToken);
            var model = new PharmacyDirectoryViewModel(
                pagedPharmacies,
                provId,
                cityId,
                districtId,
                pharmacySpecialtyId,
                cleanPharmacyName,
                provinces,
                cities,
                districts,
                specialties);

            return View(model);
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load the pharmacy directory from SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);

            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return View(new PharmacyDirectoryViewModel(
                PagedResult<PharmacyDirectoryItem>.Empty(pageSize),
                provId,
                cityId,
                districtId,
                pharmacySpecialtyId,
                cleanPharmacyName,
                [],
                [],
                [],
                [],
                DataUnavailable: true));
        }
    }

    [HttpGet]
    public async Task<IActionResult> AvailabilityGeneric(
        int DrugId,
        [FromRoute(Name = "ProvinceId")] int? routeProvinceId = null,
        [FromRoute(Name = "ProvinceSlug")] string? provinceSlug = null,
        [FromQuery] int provId = 0,
        [FromQuery] int cityId = 0,
        [FromQuery] int districtId = 0,
        [FromQuery] int pharmacySpecialtyId = 0,
        [FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
    {
        if (DrugId <= 0)
        {
            return NotFound();
        }

        try
        {
            var names = await _context.DrugsNames
                .AsNoTracking()
                .Where(item =>
                    item.DrugId == DrugId &&
                    (item.DrugNameType == "A" ||
                     (item.DrugNameType == "F" && item.DrugNameLanguage == false)))
                .Select(item => new { item.DrugNameType, item.DrugName })
                .ToListAsync(cancellationToken);

            var englishName = names.FirstOrDefault(item => item.DrugNameType == "A")?.DrugName;
            var persianName = names.FirstOrDefault(item => item.DrugNameType == "F")?.DrugName;
            if (string.IsNullOrWhiteSpace(englishName) && string.IsNullOrWhiteSpace(persianName))
            {
                return NotFound();
            }

            var drugName = string.IsNullOrWhiteSpace(persianName) ? englishName! : persianName;
            var genericUrl = string.IsNullOrWhiteSpace(englishName)
                ? null
                : $"/G-{DrugId}/{DrugUrl.CreateSlug(englishName)}";

            return await ShowAvailabilityPageAsync(
                isBrand: false,
                drugId: DrugId,
                irc: 0,
                drugName,
                genericName: null,
                genericUrl,
                brandUrl: null,
                routeProvinceId,
                provinceSlug,
                provId,
                cityId,
                districtId,
                pharmacySpecialtyId,
                byDosageForm: false,
                page,
                cancellationToken);
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load generic drug availability for drug {DrugId}. SQL error number: {SqlErrorNumber}.",
                DrugId,
                exception.Number);
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            var fallbackName = $"داروی {DrugId}";
            SetAvailabilityMetadata(false, DrugId, 0, fallbackName, null, null, null);
            return View("Availability", new DrugAvailabilityPageViewModel(
                false, DrugId, 0, fallbackName, null, null, null,
                routeProvinceId ?? Math.Max(0, provId), Math.Max(0, cityId),
                Math.Max(0, districtId), Math.Max(0, pharmacySpecialtyId), false,
                [], [], [], [], PagedResult<DrugAvailabilityItem>.Empty(20), DataUnavailable: true));
        }
    }

    [HttpGet]
    public async Task<IActionResult> AvailabilityBrand(
        long IRC,
        [FromRoute(Name = "ProvinceId")] int? routeProvinceId = null,
        [FromRoute(Name = "ProvinceSlug")] string? provinceSlug = null,
        [FromQuery] int provId = 0,
        [FromQuery] int cityId = 0,
        [FromQuery] int districtId = 0,
        [FromQuery] int pharmacySpecialtyId = 0,
        [FromQuery] bool byDosageForm = false,
        [FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
    {
        if (IRC <= 0)
        {
            return NotFound();
        }

        try
        {
            var brandNames = await _context.DrugsNames
                .AsNoTracking()
                .Where(item => item.Irc == IRC && item.DrugNameType == "T")
                .Select(item => new { item.DrugId, item.DrugNameLanguage, item.DrugName })
                .ToListAsync(cancellationToken);

            var englishName = brandNames.FirstOrDefault(item => item.DrugNameLanguage == true)?.DrugName;
            var persianName = brandNames.FirstOrDefault(item => item.DrugNameLanguage == false)?.DrugName;
            var genericDrugId = brandNames.FirstOrDefault()?.DrugId ?? 0;
            if (string.IsNullOrWhiteSpace(englishName) && string.IsNullOrWhiteSpace(persianName))
            {
                return NotFound();
            }

            var drugName = string.IsNullOrWhiteSpace(persianName) ? englishName! : persianName;
            var brandUrl = $"/B-{IRC}/{DrugUrl.CreateSlug(englishName ?? drugName)}";
            string? genericName = null;
            string? genericUrl = null;
            if (genericDrugId > 0)
            {
                var genericNames = await _context.DrugsNames
                    .AsNoTracking()
                    .Where(item =>
                        item.DrugId == genericDrugId &&
                        (item.DrugNameType == "A" ||
                         (item.DrugNameType == "F" && item.DrugNameLanguage == false)))
                    .Select(item => new { item.DrugNameType, item.DrugName })
                    .ToListAsync(cancellationToken);

                var genericEnglishName = genericNames
                    .FirstOrDefault(item => item.DrugNameType == "A")?.DrugName;
                genericName = genericNames
                    .FirstOrDefault(item => item.DrugNameType == "F")?.DrugName;
                if (!string.IsNullOrWhiteSpace(genericEnglishName))
                {
                    genericUrl = $"/G-{genericDrugId}/{DrugUrl.CreateSlug(genericEnglishName)}";
                }
            }

            return await ShowAvailabilityPageAsync(
                isBrand: true,
                drugId: genericDrugId,
                irc: IRC,
                drugName,
                genericName,
                genericUrl,
                brandUrl,
                routeProvinceId,
                provinceSlug,
                provId,
                cityId,
                districtId,
                pharmacySpecialtyId,
                byDosageForm,
                page,
                cancellationToken);
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load brand availability for IRC {Irc}. SQL error number: {SqlErrorNumber}.",
                IRC,
                exception.Number);
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            var fallbackName = $"برند با IRC {IRC}";
            SetAvailabilityMetadata(true, 0, IRC, fallbackName, null, null, null);
            return View("Availability", new DrugAvailabilityPageViewModel(
                true, 0, IRC, fallbackName, null, null, null,
                routeProvinceId ?? Math.Max(0, provId), Math.Max(0, cityId),
                Math.Max(0, districtId), Math.Max(0, pharmacySpecialtyId), byDosageForm,
                [], [], [], [], PagedResult<DrugAvailabilityItem>.Empty(20), DataUnavailable: true));
        }
    }

    private async Task<IActionResult> ShowAvailabilityPageAsync(
        bool isBrand,
        int drugId,
        long irc,
        string drugName,
        string? genericName,
        string? genericUrl,
        string? brandUrl,
        int? routeProvinceId,
        string? provinceSlug,
        int requestedProvinceId,
        int requestedCityId,
        int requestedDistrictId,
        int requestedSpecialtyId,
        bool byDosageForm,
        int page,
        CancellationToken cancellationToken)
    {
        const int pageSize = 20;
        page = Math.Max(1, page);
        if (routeProvinceId is <= 0)
        {
            return NotFound();
        }

        var provinceId = Math.Max(0, routeProvinceId ?? requestedProvinceId);
        var cityId = Math.Max(0, requestedCityId);
        var districtId = Math.Max(0, requestedDistrictId);
        var pharmacySpecialtyId = Math.Max(0, requestedSpecialtyId);

        var provinces = await _context.Categories
            .AsNoTracking()
            .Where(category => category.Type != null && category.Type.ToLower() == "province")
            .OrderBy(category => category.Name)
            .Select(category => new PharmacyFilterOption(category.CatId, category.Name ?? string.Empty))
            .ToListAsync(cancellationToken);

        var specialties = await _context.PharmacySpecialties
            .AsNoTracking()
            .Where(specialty => specialty.IsActive && !specialty.IsDeleted)
            .OrderBy(specialty => specialty.OrderIndex)
            .Select(specialty => new PharmacyFilterOption(specialty.ID, specialty.Title))
            .ToListAsync(cancellationToken);

        var province = provinceId > 0
            ? provinces.FirstOrDefault(item => item.Id == provinceId)
            : null;
        if (provinceId > 0 && province is null)
        {
            return NotFound();
        }

        if (provinceId == 0 && !string.IsNullOrEmpty(provinceSlug))
        {
            return NotFound();
        }

        if (!routeProvinceId.HasValue && requestedProvinceId > 0)
        {
            if (province is null)
            {
                return NotFound();
            }

            return RedirectToRoute(
                GetAvailabilityProvinceRoute(isBrand),
                BuildAvailabilityRouteValues(
                    isBrand, drugId, irc, province.Id, CreateProvinceSlug(province.Name),
                    cityId, districtId, pharmacySpecialtyId, byDosageForm, page));
        }

        if (province is not null)
        {
            var canonicalSlug = CreateProvinceSlug(province.Name);
            if (!string.Equals(provinceSlug, canonicalSlug, StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToRoutePermanent(
                    GetAvailabilityProvinceRoute(isBrand),
                    BuildAvailabilityRouteValues(
                        isBrand, drugId, irc, province.Id, canonicalSlug, cityId,
                        districtId, pharmacySpecialtyId, byDosageForm, page));
            }
        }
        var cities = provinceId > 0
            ? await _context.Categories
                .AsNoTracking()
                .Where(city => city.Type == "Township" && city.ParentId == provinceId)
                .OrderBy(city => city.Name)
                .Select(city => new PharmacyFilterOption(city.CatId, city.Name ?? string.Empty))
                .ToListAsync(cancellationToken)
            : [];
        if (cityId > 0 && !cities.Any(city => city.Id == cityId))
        {
            cityId = 0;
            districtId = 0;
        }

        var districts = cityId > 0
            ? await _context.DarooyabCityDistricts
                .AsNoTracking()
                .Where(item => item.CityId == cityId && item.IsActive)
                .OrderBy(item => item.OrderIndex)
                .ThenBy(item => item.Name)
                .Select(item => new PharmacyFilterOption(item.Id, item.Name))
                .ToListAsync(cancellationToken)
            : [];
        if (districtId > 0 && !districts.Any(item => item.Id == districtId))
        {
            districtId = 0;
        }
        if (pharmacySpecialtyId > 0 && !specialties.Any(item => item.Id == pharmacySpecialtyId))
        {
            pharmacySpecialtyId = 0;
        }

        var (items, totalCount) = await GetAvailabilityResultsAsync(
            isBrand, drugId, irc, provinceId, cityId, districtId, pharmacySpecialtyId,
            byDosageForm, page, pageSize, cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        if (page > totalPages && page > 1)
        {
            var validPage = Math.Max(1, totalPages);
            if (items.Count == 0)
            {
                (items, totalCount) = await GetAvailabilityResultsAsync(
                    isBrand, drugId, irc, provinceId, cityId, districtId, pharmacySpecialtyId,
                    byDosageForm, 1, pageSize, cancellationToken);
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                validPage = Math.Max(1, Math.Min(page, totalPages));
            }

            return RedirectToRoute(
                GetAvailabilityRoute(isBrand, provinceId > 0),
                BuildAvailabilityRouteValues(
                    isBrand, drugId, irc,
                    provinceId > 0 ? provinceId : null,
                    province is null ? null : CreateProvinceSlug(province.Name),
                    cityId, districtId, pharmacySpecialtyId, byDosageForm, validPage));
        }

        var description = province is null
            ? $"به دنبال {drugName} هستید؟ فهرست داروخانه‌های دارای موجودی ثبت‌شده‌ی این دارو را در سراسر کشور مشاهده کنید."
            : $"به دنبال {drugName} هستید؟ فهرست داروخانه‌های دارای موجودی ثبت‌شده‌ی این دارو را در استان {province.Name} مشاهده کنید.";
        SetAvailabilityMetadata(isBrand, drugId, irc, drugName, description, province, null);

        var model = new DrugAvailabilityPageViewModel(
            isBrand,
            drugId,
            irc,
            drugName,
            genericName,
            genericUrl,
            brandUrl,
            provinceId,
            cityId,
            districtId,
            pharmacySpecialtyId,
            byDosageForm,
            provinces,
            cities,
            districts,
            specialties,
            new PagedResult<DrugAvailabilityItem>(items, page, pageSize, totalCount));
        return View("Availability", model);
    }

    private async Task<(IReadOnlyList<DrugAvailabilityItem> Items, int TotalCount)> GetAvailabilityResultsAsync(
        bool isBrand,
        int drugId,
        long irc,
        int provinceId,
        int cityId,
        int districtId,
        int pharmacySpecialtyId,
        bool byDosageForm,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        if (!isBrand)
        {
            var rows = await _context.GetPatientReferralsAsync(
                drugId, provinceId, cityId, districtId, pharmacySpecialtyId,
                userIsFoodAndDrug: false, pharmacyAccountId: null,
                pageNumber: page, pageSize: pageSize, cancellationToken);
            var firstRow = rows.FirstOrDefault();
            return (
                rows.Select(row => new DrugAvailabilityItem(
                    row.PharmacyId, row.PharmacyName ?? string.Empty,
                    row.ProvinceName ?? string.Empty, row.TownShipName ?? string.Empty,
                    row.Address, row.Phone, row.RequestExpireDate, row.IRC,
                    row.PersBrandName, row.EngBrandName, ToShortRank(row.RankNumber)))
                    .ToArray(),
                firstRow?.TotalCount ?? 0);
        }

        if (byDosageForm)
        {
            var rows = await _context.GetSimilarBrandPatientReferralsAsync(
                new SimilarBrandFilterDto
                {
                    BrandIrc = irc,
                    ProvId = provinceId,
                    CityId = cityId,
                    DistrictId = districtId,
                    PharmacySpecialtyId = pharmacySpecialtyId,
                    UserIsFoodAndDrug = false,
                    PageNumber = page,
                    PageSize = pageSize
                },
                cancellationToken);
            var firstRow = rows.FirstOrDefault();
            return (
                rows.Select(row => new DrugAvailabilityItem(
                    row.PharmacyId, row.PharmacyName,
                    row.ProvinceName ?? string.Empty, row.TownShipName ?? string.Empty,
                    row.Address, row.Phone, row.RequestExpireDate, row.IRC,
                    row.PersBrandName, row.EnglishBrandName, row.RankNumber))
                    .ToArray(),
                firstRow?.TotalRows ?? 0);
        }

        var brandRows = await _context.GetSimilarBrandPharmacyRequestsAsync(
            new SimilarBrandFilterDto
            {
                BrandIrc = irc,
                ProvId = provinceId,
                CityId = cityId,
                DistrictId = districtId,
                PharmacySpecialtyId = pharmacySpecialtyId,
                UserIsFoodAndDrug = false,
                PageNumber = page,
                PageSize = pageSize
            },
            cancellationToken);
        var brandFirstRow = brandRows.FirstOrDefault();
        return (
            brandRows.Select(row => new DrugAvailabilityItem(
                row.PharmacyId, row.PharmacyName ?? string.Empty,
                row.ProvinceName ?? string.Empty, row.TownShipName ?? string.Empty,
                row.Address, row.Phone, row.RequestExpireDate, row.IRC ?? irc,
                row.PersBrandName, row.EnglishBrandName, row.RankNumber))
                .ToArray(),
            brandFirstRow?.TotalCount ?? 0);
    }

    private void SetAvailabilityMetadata(
        bool isBrand,
        int drugId,
        long irc,
        string drugName,
        string? description,
        PharmacyFilterOption? province,
        int? routeProvinceId)
    {
        var title = $"{drugName} در کدام داروخانه موجود است؟ | یافتن دارو در سراسر ایران";
        description ??= $"به دنبال {drugName} هستید؟ فهرست داروخانه‌های دارای موجودی ثبت‌شده‌ی این دارو را در سراسر کشور مشاهده کنید.";
        var provinceId = routeProvinceId ?? province?.Id ?? 0;
        var routeName = GetAvailabilityRoute(isBrand, provinceId > 0);
        var routeValues = BuildAvailabilityRouteValues(
            isBrand, drugId, irc, provinceId > 0 ? provinceId : null,
            provinceId > 0 && province is not null ? CreateProvinceSlug(province.Name) : null,
            0, 0, 0, false, 1);
        var canonicalUrl = Url.RouteUrl(routeName, routeValues, Request.Scheme);

        ViewData["Title"] = title;
        ViewData["MetaDescription"] = description;
        ViewData["CanonicalUrl"] = canonicalUrl;
        ViewData["OpenGraphUrl"] = canonicalUrl;
        ViewData["OpenGraphTitle"] = title;
        ViewData["OpenGraphDescription"] = description;
        ViewData["OpenGraphType"] = "website";
        ViewData["DrugId"] = drugId > 0 ? drugId : null;
    }

    private static short? ToShortRank(int? rank) =>
        rank is > 0 and <= short.MaxValue ? (short)rank.Value : null;

    private static string CreateProvinceSlug(string provinceName)
    {
        var slug = Regex.Replace(provinceName.Trim(), @"\s+", "-");
        slug = Regex.Replace(slug, @"[^آ-یا-زA-Za-z0-9\-]", string.Empty);
        return slug.ToLowerInvariant();
    }

    private static string GetAvailabilityRoute(bool isBrand, bool byProvince) =>
        (isBrand, byProvince) switch
        {
            (true, true) => "AvailabilityByProvinceIRC",
            (true, false) => "AvailabilityIRC",
            (false, true) => "AvailabilityByProvince",
            _ => "Availability"
        };

    private static string GetAvailabilityProvinceRoute(bool isBrand) =>
        GetAvailabilityRoute(isBrand, byProvince: true);

    private static RouteValueDictionary BuildAvailabilityRouteValues(
        bool isBrand,
        int drugId,
        long irc,
        int? provinceId,
        string? provinceSlug,
        int cityId,
        int districtId,
        int pharmacySpecialtyId,
        bool byDosageForm,
        int page)
    {
        var values = new RouteValueDictionary
        {
            [isBrand ? "IRC" : "DrugId"] = isBrand ? irc : drugId
        };
        if (provinceId.HasValue)
        {
            values["ProvinceId"] = provinceId.Value;
            values["ProvinceSlug"] = provinceSlug ?? string.Empty;
        }
        if (cityId > 0)
        {
            values["cityId"] = cityId;
        }
        if (districtId > 0)
        {
            values["districtId"] = districtId;
        }
        if (pharmacySpecialtyId > 0)
        {
            values["pharmacySpecialtyId"] = pharmacySpecialtyId;
        }
        if (isBrand && byDosageForm)
        {
            values["byDosageForm"] = true;
        }
        if (page > 1)
        {
            values["page"] = page;
        }
        return values;
    }

    [HttpGet("/Pharmaceuticalcompanies", Name = "PharmaceuticalCompanies")]
    public async Task<IActionResult> PharmaceuticalCompanies(
        [FromQuery] int producerType = 1,
        CancellationToken cancellationToken = default)
    {
        if (producerType is not (1 or 2))
        {
            return BadRequest();
        }

        const string title =
            "لیست شرکت های دارویی ایران|شرکت های تولیدکننده دارو در ایران|کارخانه های دارویی ایران|iran pharmaceutical companies list";
        const string description =
            "فهرست شرکت‌های تولیدکننده دارو، همراه با کشور و تعداد محصولات ثبت‌شده‌ی مرتبط در دارویاب.";
        ViewData["Title"] = title;
        ViewData["MetaDescription"] = description;
        ViewData["CanonicalUrl"] = "https://www.darooyab.ir/PharmaceuticalCompanies";
        ViewData["OpenGraphUrl"] = ViewData["CanonicalUrl"];
        ViewData["OpenGraphTitle"] = title;
        ViewData["OpenGraphDescription"] = description;
        ViewData["OpenGraphType"] = "website";

        try
        {
            var countryId = (byte)1;
            var isIranianProducer = producerType == 1;
            var companies = await (
                from factory in _context.Factorys.AsNoTracking()
                join factoryType in _context.FactoryTypes.AsNoTracking()
                    on factory.FactoryTypeId equals (byte?)factoryType.FactoryTypeId
                join organizationDetail in _context.TblOrganizationDetails.AsNoTracking()
                    on (int?)factory.FactoryId equals organizationDetail.TblOrganizationDetailFactoryIdFk
                    into organizationDetails
                from organizationDetail in organizationDetails.DefaultIfEmpty()
                where isIranianProducer
                    ? factory.CountryId == countryId
                    : factory.CountryId != countryId
                let productCount = _context.DrugsNames.Count(drug =>
                    drug.FactoryId == factory.FactoryId &&
                    drug.DrugNameType == "T" &&
                    drug.Irc != null &&
                    drug.DrugNameLanguage != false)
                where productCount > 0
                orderby
                    (organizationDetail == null
                        ? 0
                        : organizationDetail.TblOrganizationDetailRankInDarooyab ?? 0) descending,
                    productCount descending
                select new PharmaceuticalCompanyDirectoryItem(
                    factory.FactoryId,
                    factory.FactoryName ?? string.Empty,
                    factory.Country == null ? string.Empty : factory.Country!.CountryNameFa,
                    productCount,
                    organizationDetail == null
                        ? null
                        : organizationDetail.TblOrganizationDetailLogoAddress))
                .ToListAsync(cancellationToken);

            return View(new CompanyDirectoryViewModel(
                companies,
                producerType,
                DataUnavailable: false));
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load pharmaceutical companies from SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);

            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return View(new CompanyDirectoryViewModel(
                [],
                producerType,
                DataUnavailable: true));
        }
    }

    [HttpGet("/ImporterCompanies", Name = "ImporterCompanies")]
    public async Task<IActionResult> ImporterCompanies(CancellationToken cancellationToken)
    {
        const string title =
            "لیست شرکت های وارد کننده دارو ایران|شرکت های دارویی وارداتی ایران|iran pharmaceutical companies list";
        const string description =
            "فهرست شرکت‌های واردکننده‌ی دارو در ایران، همراه با کشور و تعداد محصولات ثبت‌شده‌ی مرتبط در دارویاب.";
        ViewData["Title"] = title;
        ViewData["MetaDescription"] = description;
        ViewData["CanonicalUrl"] = "https://www.darooyab.ir/ImporterCompanies";
        ViewData["OpenGraphUrl"] = ViewData["CanonicalUrl"];
        ViewData["OpenGraphTitle"] = title;
        ViewData["OpenGraphDescription"] = description;
        ViewData["OpenGraphType"] = "website";

        try
        {
            var countryId = (byte)1;
            var companies = await (
                from factory in _context.Factorys.AsNoTracking()
                join factoryType in _context.FactoryTypes.AsNoTracking()
                    on factory.FactoryTypeId equals (byte?)factoryType.FactoryTypeId
                join organizationDetail in _context.TblOrganizationDetails.AsNoTracking()
                    on (int?)factory.FactoryId equals organizationDetail.TblOrganizationDetailFactoryIdFk
                    into organizationDetails
                from organizationDetail in organizationDetails.DefaultIfEmpty()
                where factory.CountryId == countryId
                let productCount = _context.DrugsNames.Count(drug =>
                    drug.ImporterCompanyId == factory.FactoryId &&
                    drug.DrugNameType == "T" &&
                    drug.Irc != null &&
                    drug.DrugNameLanguage != false)
                where productCount > 0
                orderby
                    (organizationDetail == null
                        ? 0
                        : organizationDetail.TblOrganizationDetailRankInDarooyab ?? 0) descending,
                    productCount descending
                select new PharmaceuticalCompanyDirectoryItem(
                    factory.FactoryId,
                    factory.FactoryName ?? string.Empty,
                    factory.Country == null ? string.Empty : factory.Country!.CountryNameFa,
                    productCount,
                    organizationDetail == null
                        ? null
                        : organizationDetail.TblOrganizationDetailLogoAddress))
                .ToListAsync(cancellationToken);

            return View("PharmaceuticalCompanies", new CompanyDirectoryViewModel(
                companies,
                ProducerType: 3,
                DataUnavailable: false,
                IsImporterDirectory: true));
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load importer companies from SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);

            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return View("PharmaceuticalCompanies", new CompanyDirectoryViewModel(
                [],
                ProducerType: 3,
                DataUnavailable: true,
                IsImporterDirectory: true));
        }
    }

    [HttpGet("/DistributerCompanies", Name = "DistributerCompanies")]
    public async Task<IActionResult> DistributerCompanies(CancellationToken cancellationToken)
    {
        const string title = "لیست شرکت های توزیع کننده دارو";
        const string description =
            "فهرست شرکت‌های توزیع‌کننده‌ی دارو در ایران، همراه با کشور و تعداد محصولات ثبت‌شده‌ی مرتبط در دارویاب.";
        ViewData["Title"] = title;
        ViewData["MetaDescription"] = description;
        ViewData["CanonicalUrl"] = "https://www.darooyab.ir/DistributerCompanies";
        ViewData["OpenGraphUrl"] = ViewData["CanonicalUrl"];
        ViewData["OpenGraphTitle"] = title;
        ViewData["OpenGraphDescription"] = description;
        ViewData["OpenGraphType"] = "website";

        try
        {
            const byte iranCountryId = 1;
            const byte distributorFactoryTypeId = 4;
            var companies = await (
                from factory in _context.Factorys.AsNoTracking()
                join factoryType in _context.FactoryTypes.AsNoTracking()
                    on factory.FactoryTypeId equals (byte?)factoryType.FactoryTypeId
                join organizationDetail in _context.TblOrganizationDetails.AsNoTracking()
                    on (int?)factory.FactoryId equals organizationDetail.TblOrganizationDetailFactoryIdFk
                    into organizationDetails
                from organizationDetail in organizationDetails.DefaultIfEmpty()
                where factory.FactoryTypeId == distributorFactoryTypeId &&
                    factory.CountryId == iranCountryId
                let productCount = _context.DrugsNames.Count(drug =>
                    drug.FactoryId == factory.FactoryId &&
                    drug.DrugNameType == "T" &&
                    drug.Irc != null &&
                    drug.DrugNameLanguage != false)
                where productCount > 0
                orderby
                    (organizationDetail == null
                        ? 0
                        : organizationDetail.TblOrganizationDetailRankInDarooyab ?? 0) descending,
                    productCount descending
                select new PharmaceuticalCompanyDirectoryItem(
                    factory.FactoryId,
                    factory.FactoryName ?? string.Empty,
                    factory.Country == null ? string.Empty : factory.Country!.CountryNameFa,
                    productCount,
                    organizationDetail == null
                        ? null
                        : organizationDetail.TblOrganizationDetailLogoAddress))
                .ToListAsync(cancellationToken);

            return View("PharmaceuticalCompanies", new CompanyDirectoryViewModel(
                companies,
                ProducerType: 4,
                DataUnavailable: false,
                IsDistributorDirectory: true));
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load distributor companies from SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);

            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return View("PharmaceuticalCompanies", new CompanyDirectoryViewModel(
                [],
                ProducerType: 4,
                DataUnavailable: true,
                IsDistributorDirectory: true));
        }
    }

    [HttpGet("/Pharmaceuticalcompanies/{companyId:int}", Name = "PharmaceuticalCompanyProducts")]
    public Task<IActionResult> PharmaceuticalCompanyProducts(
        int companyId,
        CancellationToken cancellationToken) =>
        LoadCompanyProductsAsync(companyId, "Pharmaceuticalcompanies", isImporterDirectory: false, cancellationToken);

    [HttpGet("/ImporterCompanies/{companyId:int}", Name = "ImporterCompanyProducts")]
    public Task<IActionResult> ImporterCompanyProducts(
        int companyId,
        CancellationToken cancellationToken) =>
        LoadCompanyProductsAsync(companyId, "ImporterCompanies", isImporterDirectory: true, cancellationToken);

    [HttpGet("/DistributerCompanies/{companyId:int}", Name = "DistributorCompanyProducts")]
    public Task<IActionResult> DistributorCompanyProducts(
        int companyId,
        CancellationToken cancellationToken) =>
        LoadCompanyProductsAsync(companyId, "DistributerCompanies", isImporterDirectory: false, cancellationToken);

    private async Task<IActionResult> LoadCompanyProductsAsync(
        int companyId,
        string directoryPath,
        bool isImporterDirectory,
        CancellationToken cancellationToken)
    {
        if (companyId is <= 0 or > short.MaxValue)
        {
            return NotFound();
        }

        const string siteUrl = "https://www.darooyab.ir";
        var canonicalUrl = $"{siteUrl}/{directoryPath}/{companyId}";
        ViewData["CanonicalUrl"] = canonicalUrl;
        ViewData["OpenGraphUrl"] = canonicalUrl;
        ViewData["OpenGraphType"] = "website";
        ViewData["CaptchaEnabled"] = _captchaService.IsEnabled;

        try
        {
            var company = await _context.Factorys
                .AsNoTracking()
                .Where(factory => factory.FactoryId == companyId)
                .Select(factory => new
                {
                    factory.FactoryId,
                    Name = factory.FactoryName ?? string.Empty,
                    factory.CountryId,
                    CountryName = factory.Country == null ? null : factory.Country.CountryNameFa
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (company is null)
            {
                return NotFound();
            }

            var organization = await _context.TblOrganizationDetails
                .AsNoTracking()
                .Where(detail => detail.TblOrganizationDetailFactoryIdFk == companyId)
                .Select(detail => new CompanyOrganizationDetails(
                    detail.TblOrganizationDetailLogoAddress,
                    detail.TblOrganizationDetailSiteAddress,
                    detail.TblOrganizationDetailEmail,
                    detail.TblOrganizationDetailTel,
                    detail.TblOrganizationDetailFax,
                    detail.TblOrganizationDetailAddress))
                .FirstOrDefaultAsync(cancellationToken);

            var productQuery = _context.DrugsNames
                .AsNoTracking()
                .Where(drug =>
                    drug.DrugNameType == "T" &&
                    drug.Irc.HasValue &&
                    drug.DrugNameLanguage != false);
            productQuery = isImporterDirectory
                ? productQuery.Where(drug => drug.ImporterCompanyId == companyId)
                : productQuery.Where(drug => drug.FactoryId == companyId);

            var productRows = await productQuery
                .Select(drug => new
                {
                    drug.Id,
                    drug.DrugId,
                    Irc = drug.Irc!.Value,
                    Name = drug.DrugName ?? string.Empty,
                    ManufacturerName = drug.FactoryId2Navigation == null
                        ? null
                        : drug.FactoryId2Navigation.FactoryName
                })
                .ToListAsync(cancellationToken);

            var ircs = productRows.Select(product => product.Irc).Distinct().ToArray();
            var drugIds = productRows.Select(product => product.DrugId).Distinct().ToArray();
            var relatedNames = ircs.Length == 0
                ? []
                : await _context.DrugsNames
                    .AsNoTracking()
                    .Where(drug =>
                        (drug.Irc.HasValue &&
                         ircs.Contains(drug.Irc.Value) &&
                         drug.DrugNameLanguage == false) ||
                        (drugIds.Contains(drug.DrugId) && drug.DrugNameType == "A"))
                    .Select(drug => new
                    {
                        drug.Irc,
                        drug.DrugId,
                        drug.DrugNameType,
                        drug.DrugNameLanguage,
                        drug.DrugName
                    })
                    .ToListAsync(cancellationToken);

            var persianNames = relatedNames
                .Where(name => name.Irc.HasValue && name.DrugNameLanguage == false)
                .GroupBy(name => name.Irc!.Value)
                .ToDictionary(group => group.Key, group => group.First().DrugName);
            var genericNames = relatedNames
                .Where(name => name.DrugNameType == "A")
                .GroupBy(name => name.DrugId)
                .ToDictionary(group => group.Key, group => group.First().DrugName);

            var images = ircs.Length == 0
                ? []
                : await _context.TblBrandAttributes
                    .AsNoTracking()
                    .Where(attribute => ircs.Contains(attribute.TblBrandAttributeIrc))
                    .Select(attribute => new
                    {
                        attribute.TblBrandAttributeIrc,
                        attribute.Image1Url
                    })
                    .ToListAsync(cancellationToken);
            var imageByIrc = images
                .GroupBy(image => image.TblBrandAttributeIrc)
                .ToDictionary(group => group.Key, group => group.First().Image1Url);

            var products = productRows
                .Select(product => new CompanyProductListItem(
                    product.Id,
                    product.DrugId,
                    product.Irc,
                    product.Name,
                    persianNames.TryGetValue(product.Irc, out var persianName) ? persianName : null,
                    product.ManufacturerName,
                    genericNames.TryGetValue(product.DrugId, out var genericName) ? genericName : null,
                    imageByIrc.TryGetValue(product.Irc, out var imageUrl) ? imageUrl : null))
                .ToArray();

            var title = string.Join(
                " | ",
                new[]
                {
                    company.Name,
                    $"محصولات شرکت {company.Name}",
                    $"آدرس و تلفن {company.Name}",
                    $"وبسایت {company.Name}"
                });
            var description = $"محصولات ثبت‌شده‌ی مرتبط با شرکت {company.Name} در دارویاب.";
            ViewData["Title"] = title;
            ViewData["MetaDescription"] = description;
            ViewData["OpenGraphTitle"] = title;
            ViewData["OpenGraphDescription"] = description;

            return View("CompanyProducts", new CompanyProductsPageViewModel(
                company.FactoryId,
                company.Name,
                company.CountryName,
                directoryPath,
                organization,
                products,
                CanReceiveMessages:
                    directoryPath == "Pharmaceuticalcompanies" && company.CountryId == 1));
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load products for pharmaceutical company {CompanyId}. SQL error number: {SqlErrorNumber}.",
                companyId,
                exception.Number);
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            ViewData["Title"] = "محصولات شرکت";
            ViewData["MetaDescription"] = "دریافت اطلاعات محصولات شرکت در حال حاضر ممکن نیست.";
            ViewData["OpenGraphTitle"] = ViewData["Title"];
            ViewData["OpenGraphDescription"] = ViewData["MetaDescription"];

            return View("CompanyProducts", new CompanyProductsPageViewModel(
                (short)companyId,
                null,
                null,
                directoryPath,
                null,
                [],
                DataUnavailable: true));
        }
    }

    [HttpGet("/profile/{id:int}", Name = "DoctorProfile")]
    public async Task<IActionResult> DoctorProfile(int id, CancellationToken cancellationToken)
    {
        ViewData["CaptchaEnabled"] = _captchaService.IsEnabled;
        try
        {
            var doctor = await _context.DarooyabUsers
                .AsNoTracking()
                .Where(user =>
                    user.Id == id &&
                    (user.ActivityId == 2 ||
                     user.ActivityId == 4 ||
                     user.ActivityId == 6 ||
                     user.ActivityId == 9 ||
                     user.ActivityId == 11) &&
                    user.IsAuthenticationConfirmByIRIMC == true &&
                    (user.Name + user.Family).Length > 4 &&
                    user.IsDeleted != true)
                .Select(user => new
                {
                    user.Id,
                    Name = user.Name ?? string.Empty,
                    Family = user.Family ?? string.Empty,
                    user.ProfileImageUrl,
                    user.MedicalSystemNumber,
                    user.ActivityId,
                    ActivityTitle = _context.DarooyabActivities
                        .Where(activity => activity.Id == user.ActivityId)
                        .Select(activity => activity.Title)
                        .FirstOrDefault(),
                    Specialty = user.DarooyabSpecialty == null
                        ? null
                        : user.DarooyabSpecialty.Title,
                    SubSpecialty = user.DarooyabSuperSpecialty == null
                        ? null
                        : (!string.IsNullOrEmpty(user.DarooyabSuperSpecialty.FullTitle)
                            ? user.DarooyabSuperSpecialty.FullTitle
                            : user.DarooyabSuperSpecialty.Title),
                    Fellowship = user.DarooyabFellowship == null
                        ? null
                        : (!string.IsNullOrEmpty(user.DarooyabFellowship.FullTitle)
                            ? user.DarooyabFellowship.FullTitle
                            : user.DarooyabFellowship.Title),
                    City = user.DarooyabCity == null ? null : user.DarooyabCity.Name,
                    Province = user.DarooyabCity == null || user.DarooyabCity.DarooyabProvince == null
                        ? null
                        : user.DarooyabCity.DarooyabProvince.Name
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (doctor is null)
            {
                return NotFound();
            }

            var addresses = await _context.DarooyabUserAddresses
                .AsNoTracking()
                .Where(address =>
                    address.UserId == id &&
                    address.IsVisible &&
                    !address.IsDeleted &&
                    address.AddressTypeId != 9 &&
                    address.Address != null &&
                    address.Address != string.Empty)
                .OrderBy(address => address.AddressTypeId)
                .ThenBy(address => address.Id)
                .Select(address => new DoctorDirectoryAddress(
                    address.DarooyabAddressType == null
                        ? "نشانی"
                        : address.DarooyabAddressType.Title,
                    address.DarooyabCity == null ||
                    address.DarooyabCity.DarooyabProvince == null
                        ? null
                        : $"استان {address.DarooyabCity.DarooyabProvince.Name}",
                    address.DarooyabCity == null
                        ? null
                        : $"شهر {address.DarooyabCity.Name}",
                    $"آدرس: {address.Address}",
                    address.PostalCode,
                    address.Phone1,
                    address.Phone2,
                    Website: string.Empty,
                    address.Latitude,
                    address.Longitude))
                .ToListAsync(cancellationToken);

            int? legacyAccountId = null;
            IReadOnlyList<DrugComment> doctorComments = [];
            if (!string.IsNullOrWhiteSpace(doctor.MedicalSystemNumber))
            {
                var matchingLegacyAccountIds = await _context.WebSiteUserAccounts
                    .AsNoTracking()
                    .Where(account => account.NezamCode == doctor.MedicalSystemNumber)
                    .OrderBy(account => account.Id)
                    .Select(account => account.Id)
                    .Take(2)
                    .ToListAsync(cancellationToken);
                if (matchingLegacyAccountIds.Count == 1)
                {
                    legacyAccountId = matchingLegacyAccountIds[0];
                    doctorComments = await _publicCommentService.GetApprovedCommentsAsync(
                        "doctor",
                        legacyAccountId.Value,
                        cancellationToken);
                }
            }

            var profile = new DoctorProfileViewModel(
                doctor.Id,
                doctor.Name,
                doctor.Family,
                string.IsNullOrWhiteSpace(doctor.ProfileImageUrl)
                    ? null
                    : $"https://api.darooyab.ir/upload/{doctor.ProfileImageUrl.TrimStart('/')}",
                doctor.ActivityId,
                doctor.ActivityTitle,
                doctor.Specialty,
                doctor.SubSpecialty,
                doctor.Fellowship,
                doctor.MedicalSystemNumber,
                doctor.City,
                doctor.Province,
                addresses,
                doctorComments,
                legacyAccountId);

            var isMidwife = profile.ActivityId is 9 or 11;
            var pageTitle = isMidwife
                ? $"{profile.Name} {profile.Family}"
                : $"دکتر {profile.Name} {profile.Family}";
            if (!string.IsNullOrWhiteSpace(profile.Specialty))
            {
                pageTitle += $". {profile.Specialty}";
                if (profile.ActivityId == 9)
                {
                    pageTitle += " مامایی";
                }
            }

            var location = new[] { profile.Province, profile.City }
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .ToArray();
            if (location.Length > 0)
            {
                pageTitle += $". {string.Join("، ", location)}";
            }

            var displayName = string.IsNullOrWhiteSpace(profile.ActivityTitle)
                ? $"کاربر {profile.Name} {profile.Family}"
                : isMidwife
                    ? $"{profile.Name} {profile.Family}"
                    : $"دکتر {profile.Name} {profile.Family}";
            var description = $"پروفایل دارویاب {displayName}";
            var canonicalUrl = $"https://www.darooyab.ir/profile/{profile.Id}";
            ViewData["Title"] = pageTitle;
            ViewData["MetaDescription"] = description;
            ViewData["CanonicalUrl"] = canonicalUrl;
            ViewData["OpenGraphUrl"] = canonicalUrl;
            ViewData["OpenGraphTitle"] = pageTitle;
            ViewData["OpenGraphDescription"] = description;
            ViewData["OpenGraphType"] = "profile";
            ViewData["OpenGraphImage"] = profile.ImageUrl;

            return View(profile);
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load doctor profile {DoctorId} from SQL Server. SQL error number: {SqlErrorNumber}.",
                id,
                exception.Number);
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
    }

    [HttpGet("/doctors", Name = "HealthcareProfessionalsIndex")]
    [HttpGet("/doctors/{page:int}", Name = "HealthcareProfessionals")]
    public async Task<IActionResult> Doctors(
        [FromRoute] int page = 1,
        CancellationToken cancellationToken = default)
    {
        const int pageSize = 20;
        const string title = "فهرست متخصصان سلامت دارویاب";
        const string description = "فهرست دکترها، دندانپزشکان، داروسازان و ماماهای تأییدشده در دارویاب";
        static string? GetSafeWebsiteUrl(string? website)
        {
            if (string.IsNullOrWhiteSpace(website))
            {
                return null;
            }

            var value = website.Trim();
            if (!value.Contains("://", StringComparison.Ordinal))
            {
                value = $"https://{value}";
            }

            return Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
                   (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                ? uri.AbsoluteUri
                : null;
        }

        ViewData["Title"] = title;
        ViewData["MetaDescription"] = description;
        ViewData["CanonicalUrl"] = "https://www.darooyab.ir/doctors";
        ViewData["OpenGraphUrl"] = ViewData["CanonicalUrl"];
        ViewData["OpenGraphTitle"] = title;
        ViewData["OpenGraphDescription"] = description;
        ViewData["OpenGraphType"] = "website";

        page = Math.Max(1, page);

        try
        {
            var orderedUsers = _context.DarooyabUsers
                .AsNoTracking()
                .Where(user =>
                    (user.ActivityId == 2 ||
                     user.ActivityId == 4 ||
                     user.ActivityId == 6 ||
                     user.ActivityId == 9 ||
                     user.ActivityId == 11) &&
                    user.IsAuthenticationConfirmByIRIMC == true &&
                    (user.Name + user.Family).Length > 4 &&
                    user.IsDeleted != true)
                .OrderBy(user => user.ActivityId == 2
                    ? (user.SpecialtyId.HasValue && user.SpecialtyId.Value != 171 ? 0 : 1)
                    : user.ActivityId == 6 ? 2 : 3)
                .ThenBy(user => user.Id)
                .Select(user => new
                {
                    user.Id,
                    Name = user.Name ?? string.Empty,
                    Family = user.Family ?? string.Empty,
                    user.ProfileImageUrl,
                    user.MedicalSystemNumber,
                    user.ActivityId,
                    ActivityTitle = _context.DarooyabActivities
                        .Where(activity => activity.Id == user.ActivityId)
                        .Select(activity => activity.Title)
                        .FirstOrDefault(),
                    Specialty = user.DarooyabSpecialty == null
                        ? null
                        : user.DarooyabSpecialty.Title,
                    SubSpecialty = user.DarooyabSuperSpecialty == null
                        ? null
                        : (!string.IsNullOrEmpty(user.DarooyabSuperSpecialty.FullTitle)
                            ? user.DarooyabSuperSpecialty.FullTitle
                            : user.DarooyabSuperSpecialty.Title),
                    Fellowship = user.DarooyabFellowship == null
                        ? null
                        : (!string.IsNullOrEmpty(user.DarooyabFellowship.FullTitle)
                            ? user.DarooyabFellowship.FullTitle
                            : user.DarooyabFellowship.Title)
                });

            var pagedUsers = await orderedUsers.ToPagedResultAsync(page, pageSize, cancellationToken);
            var userIds = pagedUsers.Items.Select(user => user.Id).ToArray();
            var addresses = await _context.DarooyabUserAddresses
                .AsNoTracking()
                .Where(address =>
                    userIds.Contains(address.UserId) &&
                    address.IsVisible &&
                    !address.IsDeleted &&
                    address.AddressTypeId != 9 &&
                    address.Address != null &&
                    address.Address != string.Empty)
                .OrderBy(address => address.AddressTypeId)
                .ThenBy(address => address.Id)
                .Select(address => new
                {
                    address.UserId,
                    Type = address.DarooyabAddressType == null
                        ? "نشانی"
                        : address.DarooyabAddressType.Title,
                    Province = address.DarooyabCity == null ||
                               address.DarooyabCity.DarooyabProvince == null
                        ? null
                        : address.DarooyabCity.DarooyabProvince.Name,
                    City = address.DarooyabCity == null
                        ? null
                        : address.DarooyabCity.Name,
                    Address = address.Address!,
                    address.PostalCode,
                    address.Phone1,
                    address.Phone2,
                    address.Website
                })
                .ToListAsync(cancellationToken);

            var addressesByUser = addresses
                .GroupBy(address => address.UserId)
                .ToDictionary(
                    group => group.Key,
                    group => (IReadOnlyList<DoctorDirectoryAddress>)group
                        .Select(address => new DoctorDirectoryAddress(
                            address.Type,
                            address.Province is null ? null : $"استان {address.Province}",
                            address.City is null ? null : $"شهر {address.City}",
                            $"آدرس: {address.Address}",
                            address.PostalCode,
                            address.Phone1,
                            address.Phone2,
                            GetSafeWebsiteUrl(address.Website)))
                        .ToArray());

            var doctors = pagedUsers.Items
                .Select(user => new DoctorDirectoryItem(
                    user.Id,
                    user.Name,
                    user.Family,
                    string.IsNullOrWhiteSpace(user.ProfileImageUrl)
                        ? null
                        : $"https://api.darooyab.ir/upload/{user.ProfileImageUrl.TrimStart('/')}",
                    user.MedicalSystemNumber,
                    user.ActivityId,
                    user.ActivityTitle,
                    user.Specialty,
                    user.SubSpecialty,
                    user.Fellowship,
                    addressesByUser.TryGetValue(user.Id, out var userAddresses)
                        ? userAddresses
                        : []))
                .ToArray();

            return View(new DoctorDirectoryViewModel(
                new PagedResult<DoctorDirectoryItem>(
                    doctors,
                    pagedUsers.PageNumber,
                    pagedUsers.PageSize,
                    pagedUsers.TotalCount)));
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load the public doctor directory from SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);

            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return View(new DoctorDirectoryViewModel(
                PagedResult<DoctorDirectoryItem>.Empty(pageSize),
                DataUnavailable: true));
        }
    }

    [HttpGet("/ph-{pharmacyId:int}/{pharmacyName?}", Name = "PharmacyDetails")]
    public async Task<IActionResult> PharmacyDetails(
        int pharmacyId,
        string? pharmacyName,
        [FromQuery(Name = "IRC")] long? irc,
        [FromQuery] int page = 1,
        [FromQuery] int? provId = null,
        [FromQuery] int cityId = 0,
        CancellationToken cancellationToken = default)
    {
        if (pharmacyId < 1)
        {
            return NotFound();
        }

        page = Math.Max(1, page);
        irc = irc is > 0 ? irc : null;

        try
        {
            var pharmacy = await _context.Pharmacies
                .AsNoTracking()
                .Include(item => item.CityIdFkNavigation)
                .Include(item => item.ProvinceIdFkNavigation)
                .SingleOrDefaultAsync(item => item.Id == pharmacyId, cancellationToken);

            if (pharmacy is null)
            {
                return NotFound();
            }

            var cityName = pharmacy.CityIdFkNavigation?.Name ?? string.Empty;
            var provinceName = pharmacy.ProvinceIdFkNavigation?.Name ?? string.Empty;
            var canonicalSlug = PharmacyUrl.CreateSlug(pharmacy.Name, cityName);
            var routeNeedsRedirect = !string.Equals(
                pharmacyName,
                canonicalSlug,
                StringComparison.Ordinal);

            if (routeNeedsRedirect ||
                (Request.Query.ContainsKey("page") && (irc.HasValue || page == 1)) ||
                (irc.HasValue && page != 1))
            {
                var redirectUrl = Url.RouteUrl(
                    "PharmacyDetails",
                    new
                    {
                        pharmacyId,
                        pharmacyName = canonicalSlug,
                        IRC = irc,
                        page = irc.HasValue || page == 1 ? (int?)null : page
                    }) ?? throw new InvalidOperationException("The pharmacy detail route is not registered.");
                return RedirectPermanent(redirectUrl);
            }

            const int pageSize = 20;
            var today = DateTime.Today;
            var pharmacyAccountId = await _context.PharmacyAccounts
                .AsNoTracking()
                .Where(account => account.PharmacyIdFk == pharmacyId)
                .OrderBy(account => account.Id)
                .Select(account => (int?)account.Id)
                .FirstOrDefaultAsync(cancellationToken);

            var referralCategoryId = await _context.Categories
                .AsNoTracking()
                .Where(category => category.Type == "patient_referral")
                .Select(category => (int?)category.CatId)
                .FirstOrDefaultAsync(cancellationToken);

            var reportingProvinceId = provId ?? (irc.HasValue ? pharmacy.ProvinceIdFk : 0);
            cityId = Math.Max(0, cityId);
            var reportingProvinces = irc.HasValue
                ? await _context.Categories
                    .AsNoTracking()
                    .Where(category => category.Type != null &&
                        category.Type.ToLower() == "province")
                    .OrderBy(category => category.CatId)
                    .Select(category => new PharmacyFilterOption(
                        category.CatId,
                        category.Name ?? string.Empty))
                    .ToListAsync(cancellationToken)
                : [];
            var reportingCities = irc.HasValue && reportingProvinceId > 0
                ? await _context.Categories
                    .AsNoTracking()
                    .Where(category =>
                        category.Type == "Township" &&
                        category.ParentId == reportingProvinceId)
                    .OrderBy(category => category.CatId)
                    .Select(category => new PharmacyFilterOption(
                        category.CatId,
                        category.Name ?? string.Empty))
                    .ToListAsync(cancellationToken)
                : [];
            if (cityId > 0 && !reportingCities.Any(city => city.Id == cityId))
            {
                cityId = 0;
            }

            var brandRecord = irc.HasValue
                ? await _context.DrugsNames
                    .AsNoTracking()
                    .Where(item => item.Irc == irc && item.DrugNameLanguage == false)
                    .OrderByDescending(item => item.Id)
                    .Select(item => new { item.Id, item.DrugId, item.DrugName })
                    .FirstOrDefaultAsync(cancellationToken)
                : null;

            if (irc.HasValue && brandRecord is null)
            {
                return NotFound();
            }

            var englishBrandName = irc.HasValue
                ? await _context.DrugsNames
                    .AsNoTracking()
                    .Where(item => item.Irc == irc && item.DrugNameLanguage == true)
                    .OrderByDescending(item => item.Id)
                    .Select(item => item.DrugName)
                    .FirstOrDefaultAsync(cancellationToken)
                : null;

            string? genericName = null;
            string? genericUrl = null;
            TblBrandAttribute? brandAttributes = null;
            if (brandRecord is not null)
            {
                genericName = await _context.DrugsNames
                    .AsNoTracking()
                    .Where(item =>
                        item.DrugId == brandRecord.DrugId &&
                        item.Irc == null &&
                        item.DrugNameType == "F" &&
                        item.DrugNameLanguage == false)
                    .OrderByDescending(item => item.Id)
                    .Select(item => item.DrugName)
                    .FirstOrDefaultAsync(cancellationToken);

                var genericEnglishName = await _context.DrugsNames
                    .AsNoTracking()
                    .Where(item =>
                        item.DrugId == brandRecord.DrugId &&
                        item.Irc == null &&
                        item.DrugNameType == "A")
                    .OrderByDescending(item => item.Id)
                    .Select(item => item.DrugName)
                    .FirstOrDefaultAsync(cancellationToken);

                if (!string.IsNullOrWhiteSpace(genericEnglishName))
                {
                    genericUrl = Url.RouteUrl(
                        "G-{DrugId}/{EngGenericName}",
                        new
                        {
                            DrugId = brandRecord.DrugId,
                            EngGenericName = DrugUrl.CreateSlug(genericEnglishName)
                        });
                }

                brandAttributes = await _context.TblBrandAttributes
                    .AsNoTracking()
                    .Where(item => item.TblBrandAttributeIrc == irc)
                    .OrderBy(item => item.TblBrandAttributeId)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            var pharmacyImages = new[]
            {
                PharmacyUrl.NormalizeImageUrl(pharmacy.Image1Url),
                PharmacyUrl.NormalizeImageUrl(pharmacy.Image2Url),
                PharmacyUrl.NormalizeImageUrl(pharmacy.Image3Url)
            };
            var brandImages = brandAttributes is null
                ? []
                : new[]
                {
                    PharmacyUrl.NormalizeImageUrl(brandAttributes.Image1Url),
                    PharmacyUrl.NormalizeImageUrl(brandAttributes.Image2Url),
                    PharmacyUrl.NormalizeImageUrl(brandAttributes.Image3Url)
                };
            pharmacyImages = pharmacyImages
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            brandImages = brandImages
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var specialties = await (
                from map in _context.PharmacySpecialtyMaps.AsNoTracking()
                join specialty in _context.PharmacySpecialties.AsNoTracking()
                    on map.PharmacySpecialtyId_fk equals specialty.ID
                where map.PharmacyId_fk == pharmacyId &&
                      map.IsActive && !map.IsDeleted &&
                      specialty.IsActive && !specialty.IsDeleted
                orderby specialty.OrderIndex
                select specialty.Title)
                .Distinct()
                .ToListAsync(cancellationToken);

            IReadOnlyList<OtherPharmacyReport> otherPharmacies = [];
            if (irc.HasValue && referralCategoryId.HasValue)
            {
                var shortagesCategoryId = await _context.Categories
                    .AsNoTracking()
                    .Where(category => category.Type == "drug_shortagesByFoodAndDrugOrg")
                    .Select(category => (int?)category.CatId)
                    .FirstOrDefaultAsync(cancellationToken);

                var hiddenUniversityIds = new List<int>();
                if (shortagesCategoryId.HasValue)
                {
                    var hiddenReports =
                        from request in _context.PharmacyRequests.AsNoTracking()
                        join account in _context.PharmacyAccounts.AsNoTracking()
                            on request.PharmacyAccountIdFk equals account.Id
                        join hiddenPharmacy in _context.Pharmacies.AsNoTracking()
                            on account.PharmacyIdFk equals hiddenPharmacy.Id
                        where request.RequestTypeFromCategoryIdFk == shortagesCategoryId.Value &&
                              request.Irc == irc.Value &&
                              request.IsHiddenByFdo == true &&
                              (cityId != 0
                                  ? hiddenPharmacy.CityIdFk == cityId
                                  : reportingProvinceId != 0
                                      ? hiddenPharmacy.ProvinceIdFk == reportingProvinceId
                                      : true)
                        select hiddenPharmacy.UniversityIdFk;

                    hiddenUniversityIds = await hiddenReports
                        .Distinct()
                        .ToListAsync(cancellationToken);
                }

                var now = DateTime.Now;
                var otherReportsQuery =
                    from request in _context.PharmacyRequests.AsNoTracking()
                    join account in _context.PharmacyAccounts.AsNoTracking()
                        on request.PharmacyAccountIdFk equals account.Id
                    join reportedPharmacy in _context.Pharmacies.AsNoTracking()
                        on account.PharmacyIdFk equals reportedPharmacy.Id
                    join city in _context.Categories.AsNoTracking()
                        on reportedPharmacy.CityIdFk equals city.CatId
                    join province in _context.Categories.AsNoTracking()
                        on reportedPharmacy.ProvinceIdFk equals province.CatId
                    where request.RequestTypeFromCategoryIdFk == referralCategoryId.Value &&
                          request.Irc == irc.Value &&
                          request.RequestExpireDate >= today &&
                          (cityId != 0
                              ? reportedPharmacy.CityIdFk == cityId
                              : reportingProvinceId != 0
                                  ? reportedPharmacy.ProvinceIdFk == reportingProvinceId
                                  : true) &&
                          !hiddenUniversityIds.Contains(reportedPharmacy.UniversityIdFk)
                    select new
                    {
                        RequestId = request.Id,
                        PharmacyId = reportedPharmacy.Id,
                        reportedPharmacy.Name,
                        City = city.Name,
                        Province = province.Name,
                        reportedPharmacy.Phone,
                        ExpireDate = request.RequestExpireDate!.Value,
                        RankNumber = _context.RankingTransactions
                            .AsNoTracking()
                            .Where(rank =>
                                rank.PharmacyId == reportedPharmacy.Id &&
                                rank.Irc == irc.Value &&
                                rank.StartDate <= now &&
                                rank.EndDate >= now)
                            .OrderBy(rank => rank.RankNumber)
                            .Select(rank => (short?)rank.RankNumber)
                            .FirstOrDefault() ?? 0
                    };

                var otherReportRows = await otherReportsQuery
                    .OrderBy(item => item.RankNumber == 0 ? short.MaxValue : item.RankNumber)
                    .ThenBy(item => item.RequestId)
                    .ToListAsync(cancellationToken);
                otherPharmacies = otherReportRows
                    .Select(item => new OtherPharmacyReport(
                        item.PharmacyId,
                        item.Name,
                        item.City ?? string.Empty,
                        item.Province ?? string.Empty,
                        item.Phone,
                        item.ExpireDate,
                        item.RankNumber,
                        Url.RouteUrl(
                            "PharmacyDetails",
                            new
                            {
                                pharmacyId = item.PharmacyId,
                                pharmacyName = PharmacyUrl.CreateSlug(item.Name, item.City),
                                IRC = irc.Value
                            }) ?? throw new InvalidOperationException(
                                "The pharmacy detail route is not registered.")))
                    .ToArray();
            }

            var availabilityQuery = _context.PharmacyRequests
                .AsNoTracking()
                .Where(request =>
                    referralCategoryId.HasValue &&
                    request.RequestTypeFromCategoryIdFk == referralCategoryId.Value &&
                    request.RequestExpireDate >= today &&
                    request.Irc.HasValue);

            if (irc.HasValue)
            {
                availabilityQuery = availabilityQuery.Where(request =>
                    request.Irc == irc.Value &&
                    _context.PharmacyAccounts.Any(account =>
                        account.Id == request.PharmacyAccountIdFk &&
                        account.PharmacyIdFk == pharmacyId));
            }
            else
            {
                availabilityQuery = availabilityQuery.Where(request =>
                    pharmacyAccountId.HasValue &&
                    request.PharmacyAccountIdFk == pharmacyAccountId.Value);
            }

            var persianBrands = _context.DrugsNames
                .AsNoTracking()
                .Where(item => item.DrugNameLanguage == false && item.Irc.HasValue);
            var genericNames = _context.DrugsNames
                .AsNoTracking()
                .Where(item => item.DrugNameType == "A");
            var activeRanks = _context.RankingTransactions
                .AsNoTracking()
                .Where(item => item.PharmacyId == pharmacyId && item.EndDate >= DateTime.Now)
                .GroupBy(item => item.Irc)
                .Select(group => new
                {
                    Irc = group.Key,
                    RankNumber = group.Min(item => item.RankNumber)
                });

            var availabilityRows =
                from request in availabilityQuery
                join brand in persianBrands on request.Irc equals brand.Irc
                let genericDisplayName = genericNames
                    .Where(item => item.DrugId == brand.DrugId)
                    .OrderBy(item => item.Id)
                    .Select(item => item.DrugName)
                    .FirstOrDefault()
                select new
                {
                    RequestId = request.Id,
                    Irc = request.Irc!.Value,
                    BrandName = brand.DrugName ?? string.Empty,
                    GenericName = genericDisplayName == null
                        ? "نامشخص"
                        : genericDisplayName.Replace("+", " + "),
                    ExpireDate = request.RequestExpireDate!.Value,
                    RankNumber = activeRanks
                        .Where(rank => rank.Irc == request.Irc)
                        .Select(rank => rank.RankNumber)
                        .FirstOrDefault()
                };

            var orderedAvailability = availabilityRows
                .OrderBy(item => item.RankNumber == 0)
                .ThenBy(item => item.RankNumber)
                .ThenByDescending(item => item.GenericName)
                .ThenByDescending(item => item.RequestId);

            var pagedRows = await orderedAvailability.ToPagedResultAsync(page, pageSize, cancellationToken);
            var currentUrl = Url.RouteUrl(
                "PharmacyDetails",
                new { pharmacyId, pharmacyName = canonicalSlug, IRC = irc },
                Request.Scheme)
                ?? throw new InvalidOperationException("The pharmacy detail route is not registered.");

            var availability = new PagedResult<PharmacyAvailabilityItem>(
                pagedRows.Items.Select(item =>
                {
                    var detailUrl = Url.RouteUrl(
                        "PharmacyDetails",
                        new
                        {
                            pharmacyId,
                            pharmacyName = canonicalSlug,
                            IRC = item.Irc
                        }) ?? throw new InvalidOperationException("The pharmacy detail route is not registered.");

                    return new PharmacyAvailabilityItem(
                        item.RequestId,
                        item.Irc,
                        item.BrandName,
                        item.GenericName,
                        item.ExpireDate,
                        item.RankNumber,
                        detailUrl);
                }).ToArray(),
                pagedRows.PageNumber,
                pagedRows.PageSize,
                pagedRows.TotalCount);

            var activeProduct = irc.HasValue
                ? await availabilityQuery
                    .MaxAsync(request => request.RequestExpireDate, cancellationToken)
                : null;
            var model = new PharmacyDetailViewModel(
                pharmacy.Id,
                pharmacy.Name,
                cityName,
                provinceName,
                pharmacy.Address ?? string.Empty,
                pharmacy.Phone,
                pharmacy.SecondaryPhone,
                pharmacy.Introduction,
                pharmacy.Latitude,
                pharmacy.Longitude,
                pharmacyImages,
                brandImages,
                specialties,
                reportingProvinces,
                reportingCities,
                reportingProvinceId,
                cityId,
                otherPharmacies,
                availability,
                irc,
                brandRecord?.DrugName,
                englishBrandName,
                genericName,
                genericUrl,
                brandAttributes?.TherapeuticApplication,
                activeProduct);

            var canonicalUrl = currentUrl;
            var pharmacyPath = Url.RouteUrl(
                "PharmacyDetails",
                new { pharmacyId, pharmacyName = canonicalSlug })
                ?? throw new InvalidOperationException("The pharmacy detail route is not registered.");
            var pharmacyUrl = $"https://www.darooyab.ir{pharmacyPath}";
            var description = irc.HasValue
                ? BuildProductMetaDescription(
                    model.BrandName!,
                    pharmacy.Name,
                    provinceName,
                    cityName,
                    activeProduct,
                    today)
                : $"داروخانه {pharmacy.Name}، واقع در {provinceName}-{cityName}-{pharmacy.Address}، تلفن: {pharmacy.Phone}";
            var title = irc.HasValue
                ? $"خرید {model.BrandName} | داروخانه {pharmacy.Name} در {provinceName}-{cityName}"
                : $"داروخانه {pharmacy.Name} - {cityName}";

            ViewData["Title"] = title;
            ViewData["MetaDescription"] = description;
            ViewData["CanonicalUrl"] = canonicalUrl;
            ViewData["OpenGraphTitle"] = title;
            ViewData["OpenGraphDescription"] = description;
            ViewData["OpenGraphUrl"] = canonicalUrl;
            ViewData["OpenGraphType"] = irc.HasValue ? "product" : "website";
            var socialImage = irc.HasValue
                ? model.ProductImages.FirstOrDefault() ?? model.Images.FirstOrDefault()
                : model.Images.FirstOrDefault();
            ViewData["OpenGraphImage"] = socialImage is null ? null : ToAbsoluteUrl(socialImage);
            ViewData["TwitterCard"] = socialImage is not null ? "summary_large_image" : "summary";
            ViewData["StructuredData"] = JsonSerializer.Serialize(
                irc.HasValue
                    ? CreateProductStructuredData(model, pharmacyUrl, activeProduct, today)
                    : CreatePharmacyStructuredData(model, pharmacyUrl));

            return irc.HasValue
                ? View("PharmacyProductDetails", model)
                : View("PharmacyDetails", model);
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load pharmacy details for pharmacy {PharmacyId}. SQL error number: {SqlErrorNumber}.",
                pharmacyId,
                exception.Number);
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            ViewData["Title"] = "اطلاعات داروخانه در دسترس نیست";
            ViewData["MetaDescription"] = "دریافت اطلاعات این داروخانه در حال حاضر ممکن نیست.";
            return View("PharmacyDetailsUnavailable");
        }
    }

    [HttpGet("/Home/GetTownshipList")]
    [HttpGet("/Pharmacy/GetTownshipList", Name = "PharmacyCities")]
    public async Task<IActionResult> GetPharmacyCities(
        [FromQuery] int provinceCatId,
        CancellationToken cancellationToken)
    {
        try
        {
            var cities = await _context.Categories
                .AsNoTracking()
                .Where(city => city.Type == "Township" && city.ParentId == provinceCatId)
                .OrderBy(city => city.Name)
                .Select(city => new PharmacyFilterOption(city.CatId, city.Name ?? string.Empty))
                .ToListAsync(cancellationToken);

            return Json(new { items = cities });
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load pharmacy cities from SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                message = "دریافت فهرست شهرها در حال حاضر ممکن نیست."
            });
        }
    }

    [HttpGet("/Home/GetCityDistrictList", Name = "PharmacyDistricts")]
    public async Task<IActionResult> GetPharmacyDistricts(
        [FromQuery] int selectedCityId,
        CancellationToken cancellationToken)
    {
        if (selectedCityId <= 0)
        {
            return Json(new { items = Array.Empty<PharmacyFilterOption>() });
        }

        try
        {
            var districts = await _context.DarooyabCityDistricts
                .AsNoTracking()
                .Where(district => district.CityId == selectedCityId && district.IsActive)
                .OrderBy(district => district.OrderIndex)
                .ThenBy(district => district.Name)
                .Select(district => new PharmacyFilterOption(district.Id, district.Name))
                .ToListAsync(cancellationToken);

            return Json(new { items = districts });
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load pharmacy districts from SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                message = "دریافت فهرست مناطق در حال حاضر ممکن نیست."
            });
        }
    }

    [HttpGet("/Search", Name = "DrugSearch")]
    public async Task<IActionResult> Search(
        [FromQuery] string? SearchText,
        [FromQuery] int PageNumber = 1,
        CancellationToken cancellationToken = default)
    {
        var searchText = SearchText?.Trim() ?? string.Empty;
        if (searchText.Length > 100)
        {
            return BadRequest(new { message = "عبارت جست‌وجو نباید بیشتر از ۱۰۰ نویسه باشد." });
        }

        ViewData["Title"] = "جستجوی دارو";
        if (searchText.Length < 2)
        {
            return View(new DrugSearchPageViewModel(
                searchText,
                PagedResult<DrugSearchResult>.Empty(DrugSearchPageViewModel.PageSize),
                false));
        }

        try
        {
            var matches = SearchDrugNames(searchText);
            var pagedMatches = await matches.ToPagedResultAsync(
                PageNumber,
                DrugSearchPageViewModel.PageSize,
                cancellationToken);
            var brandNames = await GetBrandEnglishNamesAsync(pagedMatches.Items, cancellationToken);
            var results = pagedMatches.Items
                .Select(match => CreateDrugSearchResult(match, brandNames))
                .ToArray();
            var pagedResults = new PagedResult<DrugSearchResult>(
                results,
                pagedMatches.PageNumber,
                pagedMatches.PageSize,
                pagedMatches.TotalCount);

            return View(new DrugSearchPageViewModel(searchText, pagedResults, false));
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not search drug names in SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return View(new DrugSearchPageViewModel(
                searchText,
                PagedResult<DrugSearchResult>.Empty(DrugSearchPageViewModel.PageSize),
                true));
        }
    }

    [HttpGet("/Search/Suggestions", Name = "DrugSearchSuggestions")]
    public async Task<IActionResult> DrugSearchSuggestions(
        [FromQuery] string? SearchText,
        CancellationToken cancellationToken)
    {
        var searchText = SearchText?.Trim() ?? string.Empty;
        if (searchText.Length < 2)
        {
            return Json(new { items = Array.Empty<DrugSearchResult>(), hasMore = false });
        }

        if (searchText.Length > 100)
        {
            return BadRequest(new { message = "عبارت جست‌وجو نباید بیشتر از ۱۰۰ نویسه باشد." });
        }

        try
        {
            var rows = await SearchDrugNames(searchText)
                .Take(DrugSearchPageViewModel.SuggestionLimit + 1)
                .ToListAsync(cancellationToken);
            var hasMore = rows.Count > DrugSearchPageViewModel.SuggestionLimit;
            var selectedRows = rows
                .Take(DrugSearchPageViewModel.SuggestionLimit)
                .ToArray();
            var brandNames = await GetBrandEnglishNamesAsync(selectedRows, cancellationToken);
            var items = selectedRows
                .Select(match => CreateDrugSearchResult(match, brandNames))
                .ToArray();

            return Json(new { items, hasMore });
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load drug search suggestions from SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
    }

    [HttpGet("/NewDrugs", Name = "NewDrugs")]
    public async Task<IActionResult> NewDrugs(
        [FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
    {
        ViewData["Title"] = "تازه‌های دارویی";
        ViewData["MetaDescription"] = "فهرست تازه‌های دارویی دارویاب، همراه با اطلاعات برند، نام ژنریک و کاربرد درمانی.";
        ViewData["CanonicalUrl"] = Url.RouteUrl("NewDrugs", null, HttpContext.Request.Scheme);

        try
        {
            var newestBrands = _context.DrugsNames
                .AsNoTracking()
                .Where(item =>
                    item.Irc != null &&
                    item.DrugNameLanguage == false &&
                    item.IsNewestDrug == true);
            var englishBrands = _context.DrugsNames
                .AsNoTracking()
                .Where(item => item.DrugNameLanguage == true);
            var englishGenerics = _context.DrugsNames
                .AsNoTracking()
                .Where(item => item.Irc == null && item.DrugNameType == "A");
            var persianGenerics = _context.DrugsNames
                .AsNoTracking()
                .Where(item =>
                    item.Irc == null &&
                    item.DrugNameType == "F" &&
                    item.DrugNameLanguage == false);

            var query =
                from persianBrand in newestBrands
                join englishBrand in englishBrands
                    on persianBrand.Irc equals englishBrand.Irc
                join englishGeneric in englishGenerics
                    on persianBrand.DrugId equals englishGeneric.DrugId
                join persianGenericRow in persianGenerics
                    on persianBrand.DrugId equals persianGenericRow.DrugId into persianGenericRows
                from persianGeneric in persianGenericRows.DefaultIfEmpty()
                join brandAttributeRow in _context.TblBrandAttributes.AsNoTracking()
                    on persianBrand.Irc!.Value equals brandAttributeRow.TblBrandAttributeIrc into brandAttributeRows
                from brandAttribute in brandAttributeRows.DefaultIfEmpty()
                orderby persianBrand.Id descending
                select new NewDrugListItem(
                    persianBrand.Id,
                    persianBrand.Irc!.Value,
                    persianBrand.DrugId,
                    persianBrand.DrugName ?? string.Empty,
                    englishBrand.DrugName ?? string.Empty,
                    englishGeneric.DrugName ?? string.Empty,
                    persianGeneric == null ? null : persianGeneric.DrugName,
                    brandAttribute == null ? null : brandAttribute.TherapeuticApplication,
                    brandAttribute == null ? null : brandAttribute.Image1Url,
                    persianBrand.Factory == null ? null : persianBrand.Factory!.FactoryName);

            var pagedDrugs = await query.ToPagedResultAsync(
                page,
                NewDrugPageViewModel.PageSize,
                cancellationToken);

            return View(new NewDrugPageViewModel(pagedDrugs, false));
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load new drugs from SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return View(new NewDrugPageViewModel(
                PagedResult<NewDrugListItem>.Empty(NewDrugPageViewModel.PageSize),
                true));
        }
    }

    [HttpGet]
    public async Task<IActionResult> BrandInfo(
        long? IRC,
        string? BrandEngName,
        CancellationToken cancellationToken)
    {
        ViewData["CaptchaEnabled"] = _captchaService.IsEnabled;
        if (!IRC.HasValue || IRC.Value <= 0)
        {
            return NotFound();
        }

        try
        {
            var brand = await _context.DrugsNames
                .AsNoTracking()
                .Where(item => item.Irc == IRC.Value && item.DrugNameLanguage == true)
                .OrderByDescending(item => item.Id)
                .Select(item => new
                {
                    item.Id,
                    item.DrugId,
                    item.Irc,
                    item.DrugName,
                    item.FactoryId,
                    item.ImporterCompanyId,
                    item.ProducingCountryId,
                    ManufacturerName = item.Factory == null ? null : item.Factory.FactoryName,
                    ImporterName = item.ImporterCompany == null ? null : item.ImporterCompany.FactoryName,
                    CountryName = item.ProducingCountry == null ? null : item.ProducingCountry.CountryNameFa
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (brand is null || string.IsNullOrWhiteSpace(brand.DrugName))
            {
                return NotFound();
            }

            var canonicalSlug = DrugUrl.CreateSlug(brand.DrugName);
            if (!string.Equals(BrandEngName, canonicalSlug, StringComparison.Ordinal))
            {
                return RedirectPermanent($"/B-{IRC.Value}/{canonicalSlug}");
            }

            var relatedNameRows = await _context.DrugsNames
                .AsNoTracking()
                .Where(item =>
                    (item.Irc == IRC.Value && item.DrugNameLanguage == false) ||
                    (item.DrugId == brand.DrugId &&
                     (item.DrugNameType == "A" ||
                      (item.DrugNameType == "F" && item.DrugNameLanguage == false))))
                .OrderByDescending(item => item.Id)
                .Select(item => new
                {
                    item.Irc,
                    item.DrugId,
                    item.DrugName,
                    item.DrugNameType,
                    item.DrugNameLanguage
                })
                .ToListAsync(cancellationToken);
            var persianName = relatedNameRows
                .FirstOrDefault(item =>
                    item.Irc == IRC.Value &&
                    item.DrugNameLanguage == false)?.DrugName ?? string.Empty;
            var genericName = relatedNameRows
                .FirstOrDefault(item => item.DrugNameType == "A")?.DrugName;
            var genericPersianName = relatedNameRows
                .FirstOrDefault(item =>
                    item.DrugNameType == "F" &&
                    item.DrugNameLanguage == false)?.DrugName;
            var genericInfo = await _context.DrugsGenericInfos
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.DrugId == brand.DrugId, cancellationToken);

            var attributes = await _context.TblBrandAttributes
                .AsNoTracking()
                .Where(item => item.TblBrandAttributeIrc == IRC.Value)
                .OrderByDescending(item => item.TblBrandAttributeId)
                .Select(item => new
                {
                    item.TblBrandAttributePageTitle,
                    item.TblBrandAttributePageDescription,
                    item.AdvertisingUrl,
                    item.TblBrandAttributeShopUrl,
                    item.TherapeuticApplication,
                    item.TblBrandAttributeContentFa,
                    item.TblBrandAttributeContentEn,
                    StorageTemperature = item.StorageTemperature == null
                        ? null
                        : item.StorageTemperature.Name,
                    item.IsOtc,
                    item.IsChemotherapy,
                    item.IsUnderSupervision,
                    item.IsHospital,
                    item.Price,
                    item.PriceUnitNum,
                    item.LastPriceEditorPharmacyFk,
                    item.Image1Url,
                    item.Image2Url,
                    item.Image3Url
                })
                .FirstOrDefaultAsync(cancellationToken);

            string? pricePharmacyName = null;
            string? priceCityName = null;
            if (attributes is not null && attributes.Price is > 0 && attributes.PriceUnitNum.HasValue &&
                attributes.LastPriceEditorPharmacyFk.HasValue)
            {
                var priceSource = await _context.Pharmacies
                    .AsNoTracking()
                    .Where(item => item.Id == attributes.LastPriceEditorPharmacyFk.Value)
                    .Select(item => new
                    {
                        item.Name,
                        CityName = item.CityIdFkNavigation == null
                            ? null
                            : item.CityIdFkNavigation.Name
                    })
                    .FirstOrDefaultAsync(cancellationToken);
                pricePharmacyName = priceSource?.Name;
                priceCityName = priceSource?.CityName;
            }

            var organization = brand.FactoryId.HasValue
                ? await _context.TblOrganizationDetails
                    .AsNoTracking()
                    .Where(item => item.TblOrganizationDetailFactoryIdFk == brand.FactoryId.Value)
                    .OrderByDescending(item => item.TblOrganizationDetailRankInDarooyab)
                    .Select(item => new
                    {
                        item.TblOrganizationDetailLogoAddress,
                        item.TblOrganizationDetailSiteAddress
                    })
                    .FirstOrDefaultAsync(cancellationToken)
                : null;

            darooyab.Models.Group1? martindaleGroup = null;
            if (genericInfo?.GroupId is short groupId)
            {
                martindaleGroup = await _context.Groups1
                    .AsNoTracking()
                    .FirstOrDefaultAsync(item => item.GroupId == groupId, cancellationToken);
            }

            var ahfsPath = await GetAhfsPathAsync(genericInfo?.AhfsId, cancellationToken);
            var companyId = brand.ImporterCompanyId ?? brand.FactoryId;
            var relatedBrandRows = await _context.DrugsNames
                .AsNoTracking()
                .Where(item =>
                    item.Irc.HasValue &&
                    item.Irc != IRC.Value &&
                    item.DrugNameLanguage.HasValue &&
                    companyId.HasValue &&
                    (item.FactoryId == companyId.Value || item.ImporterCompanyId == companyId.Value))
                .Select(item => new
                {
                    item.Irc,
                    item.DrugId,
                    item.DrugName,
                    item.DrugNameLanguage,
                    item.Id
                })
                .OrderByDescending(item => item.Id)
                .Take(500)
                .ToListAsync(cancellationToken);

            var relatedBrands = relatedBrandRows
                .Where(item => item.Irc.HasValue)
                .GroupBy(item => item.Irc!.Value)
                .Select(group => new
                {
                    Irc = group.Key,
                    DrugId = group.First().DrugId,
                    EnglishName = group
                        .Where(item => item.DrugNameLanguage == true)
                        .OrderByDescending(item => item.Id)
                        .Select(item => item.DrugName)
                        .FirstOrDefault(),
                    PersianName = group
                        .Where(item => item.DrugNameLanguage == false)
                        .OrderByDescending(item => item.Id)
                        .Select(item => item.DrugName)
                        .FirstOrDefault(),
                    LatestId = group.Max(item => item.Id)
                })
                .Where(item => !string.IsNullOrWhiteSpace(item.EnglishName))
                .OrderByDescending(item => item.LatestId)
                .Take(40)
                .ToArray();

            var relatedGenericBrands = relatedBrands
                .Where(item => item.DrugId == brand.DrugId)
                .Select(item => CreateRelatedBrand(item.Irc, item.EnglishName!, item.PersianName))
                .ToArray();
            var relatedCompanyBrands = relatedBrands
                .Where(item => item.DrugId != brand.DrugId)
                .Select(item => CreateRelatedBrand(item.Irc, item.EnglishName!, item.PersianName))
                .ToArray();

            var imageUrls = new[]
            {
                attributes?.Image1Url,
                attributes?.Image2Url,
                attributes?.Image3Url
            };
            var pageAttributes = attributes is null
                ? null
                : new BrandPageAttributes(
                    attributes.AdvertisingUrl,
                    attributes.TblBrandAttributeShopUrl,
                    attributes.TherapeuticApplication,
                    attributes.TblBrandAttributeContentFa,
                    attributes.TblBrandAttributeContentEn,
                    attributes.StorageTemperature,
                    attributes.IsOtc,
                    attributes.IsChemotherapy,
                    attributes.IsUnderSupervision,
                    attributes.IsHospital,
                    attributes.Price,
                    attributes.PriceUnitNum,
                    pricePharmacyName,
                    priceCityName,
                    imageUrls
                        .Where(url => !string.IsNullOrWhiteSpace(url))
                        .Select(url => new BrandGalleryImage(url!, persianName))
                        .ToArray());

            var comments = await _publicCommentService.GetApprovedCommentsAsync(
                "brand",
                IRC.Value,
                cancellationToken);

            var model = new BrandInfoPageViewModel
            {
                Irc = IRC.Value,
                DrugId = brand.DrugId,
                EnglishName = brand.DrugName,
                PersianName = persianName,
                GenericName = genericName,
                GenericPersianName = genericPersianName,
                GenericUrl = string.IsNullOrWhiteSpace(genericName)
                    ? null
                    : Url.RouteUrl(
                        "G-{DrugId}/{EngGenericName}",
                        new
                        {
                            DrugId = brand.DrugId,
                            EngGenericName = DrugUrl.CreateSlug(genericName)
                        }),
                ManufacturerName = brand.ManufacturerName,
                ImporterName = brand.ImporterName,
                CountryName = brand.CountryName,
                MartindaleName = GetMartindalePersianName(martindaleGroup),
                MartindaleGroupId = martindaleGroup?.GroupId,
                AhfsPath = ahfsPath,
                GenericUsageInformation = genericInfo is null
                    ? null
                    : LegacyDrugContentDecryptor.DecryptData(genericInfo.MavaredMasraf),
                ManufacturerLogoUrl = organization?.TblOrganizationDetailLogoAddress,
                ManufacturerWebsiteUrl = organization?.TblOrganizationDetailSiteAddress,
                Attributes = pageAttributes,
                RelatedGenericBrands = relatedGenericBrands,
                RelatedCompanyBrands = relatedCompanyBrands,
                Comments = comments
            };

            var pageTitle = !string.IsNullOrWhiteSpace(attributes?.TblBrandAttributePageTitle)
                ? attributes.TblBrandAttributePageTitle
                : string.Join(
                    " | ",
                    new[] { persianName, brand.DrugName, brand.ManufacturerName }
                        .Where(value => !string.IsNullOrWhiteSpace(value)));
            var metaDescription = !string.IsNullOrWhiteSpace(attributes?.TblBrandAttributePageDescription)
                ? attributes.TblBrandAttributePageDescription
                : !string.IsNullOrWhiteSpace(attributes?.TherapeuticApplication)
                    ? attributes.TherapeuticApplication
                    : string.Join(
                        " | ",
                        new[] { brand.DrugName, persianName, brand.ManufacturerName }
                            .Where(value => !string.IsNullOrWhiteSpace(value)));
            const string siteUrl = "https://www.darooyab.ir";
            var canonicalUrl = $"{siteUrl}/B-{IRC.Value}/{canonicalSlug}";
            ViewData["Title"] = pageTitle;
            ViewData["MetaDescription"] = metaDescription;
            ViewData["CanonicalUrl"] = canonicalUrl;
            ViewData["OpenGraphTitle"] = pageTitle;
            ViewData["OpenGraphDescription"] = metaDescription;
            ViewData["OpenGraphUrl"] = canonicalUrl;
            ViewData["StructuredData"] = JsonSerializer.Serialize(new Dictionary<string, object?>
            {
                ["@context"] = "https://schema.org",
                ["@type"] = "MedicalWebPage",
                ["@id"] = $"{canonicalUrl}#webpage",
                ["url"] = canonicalUrl,
                ["name"] = $"{persianName} | {brand.DrugName}",
                ["description"] = metaDescription,
                ["inLanguage"] = "fa-IR",
                ["mainEntity"] = new Dictionary<string, object?>
                {
                    ["@id"] = $"{canonicalUrl}#drug"
                },
                ["publisher"] = new Dictionary<string, object?>
                {
                    ["@type"] = "Organization",
                    ["name"] = "دارویاب",
                    ["url"] = $"{siteUrl}/"
                }
            });
            ViewData["DrugId"] = brand.DrugId;
            return View(model);
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load brand information for IRC {Irc}. SQL error number: {SqlErrorNumber}.",
                IRC.Value,
                exception.Number);
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
    }

    [HttpGet]
    public async Task<IActionResult> DrugInfo(
        int? DrugId,
        string? EngGenericName,
        CancellationToken cancellationToken = default)
    {
        ViewData["CaptchaEnabled"] = _captchaService.IsEnabled;
        if (!DrugId.HasValue || DrugId.Value <= 0)
        {
            return NotFound();
        }

        try
        {
            var genericName = await _context.DrugsNames
                .AsNoTracking()
                .Where(item => item.DrugId == DrugId.Value && item.DrugNameType == "A")
                .Select(item => item.DrugName)
                .FirstOrDefaultAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(genericName))
            {
                return NotFound();
            }

            var canonicalSlug = DrugUrl.CreateSlug(genericName);
            if (!string.Equals(EngGenericName, canonicalSlug, StringComparison.Ordinal))
            {
                return RedirectPermanent($"/G-{DrugId.Value}/{canonicalSlug}");
            }

            var genericInfo = await _context.DrugsGenericInfos
                .AsNoTracking()
                .Include(item => item.PregnancyCategory)
                .FirstOrDefaultAsync(item => item.DrugId == DrugId.Value, cancellationToken);

            if (genericInfo is null)
            {
                return NotFound();
            }

            var drugContent = await _context.DrugInfos
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.DrugId == DrugId.Value, cancellationToken);

            var persianName = await _context.DrugsNames
                .AsNoTracking()
                .Where(item =>
                    item.DrugId == DrugId.Value &&
                    item.DrugNameType == "F" &&
                    item.DrugNameLanguage == false)
                .Select(item => item.DrugName)
                .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

            if (persianName.Length > 18 && persianName.Contains('+'))
            {
                persianName = persianName.Replace("+", " + ");
            }

            darooyab.Models.Group1? martindaleGroup = null;
            if (genericInfo.GroupId.HasValue)
            {
                martindaleGroup = await _context.Groups1
                    .AsNoTracking()
                    .FirstOrDefaultAsync(item => item.GroupId == genericInfo.GroupId.Value, cancellationToken);
            }

            var ahfsPath = await GetAhfsPathAsync(genericInfo.AhfsId, cancellationToken);

            var dosageForms = await _context.DosageForms
                .AsNoTracking()
                .Where(item => item.DrugId == DrugId.Value)
                .OrderBy(item => item.DosageFormNameFa)
                .Select(item => new DrugDosageForm(
                    item.FdoGenerationCode,
                    item.DosageFormNameFa,
                    item.DosageFormName,
                    item.HighAlert,
                    item.TemperatureType,
                    item.Emergency,
                    item.TopWarningLabel))
                .ToListAsync(cancellationToken);

            var brandRows = await _context.DrugsNames
                .AsNoTracking()
                .Where(item =>
                    item.DrugId == DrugId.Value &&
                    item.DrugNameType == "T" &&
                    item.DrugNameLanguage.HasValue)
                .Select(item => new { item.DrugName, item.DrugNameLanguage })
                .ToListAsync(cancellationToken);

            var relatedDrugs = await GetRelatedDrugsAsync(
                DrugId.Value,
                genericInfo.AhfsId,
                cancellationToken);
            var comments = await _publicCommentService.GetApprovedCommentsAsync(
                "generic",
                DrugId.Value,
                cancellationToken);

            var informationSections = new[]
            {
                new DrugInfoSection("موارد مصرف", LegacyDrugContentDecryptor.DecryptData(genericInfo.MavaredMasraf)),
                new DrugInfoSection("مکانیسم اثر", LegacyDrugContentDecryptor.DecryptData(genericInfo.Mekanism)),
                new DrugInfoSection("فارماکوکینتیک و اجزا", LegacyDrugContentDecryptor.DecryptData(genericInfo.FarmaAjza)),
                new DrugInfoSection("منع مصرف", LegacyDrugContentDecryptor.DecryptData(genericInfo.ManeMasraf)),
                new DrugInfoSection("عوارض جانبی", LegacyDrugContentDecryptor.DecryptData(genericInfo.Avarez)),
                new DrugInfoSection("تداخلات دارویی", LegacyDrugContentDecryptor.DecryptData(genericInfo.Tadakhol)),
                new DrugInfoSection("هشدارها", LegacyDrugContentDecryptor.DecryptData(genericInfo.Hoshdar)),
                new DrugInfoSection("توصیه‌های دارویی", LegacyDrugContentDecryptor.DecryptData(genericInfo.Nokat)),
                new DrugInfoSection("مقدار مصرف", LegacyDrugContentDecryptor.DecryptData(genericInfo.MeghdarMasraf)),
                new DrugInfoSection("دوز بزرگسالان", genericInfo.AdultDosing),
                new DrugInfoSection("دوز کودکان", genericInfo.PediatricDosing),
                new DrugInfoSection("دوز سالمندان", genericInfo.GeriatricDosing),
                new DrugInfoSection("اشکال دارویی", drugContent?.Ashkal),
                new DrugInfoSection("مواد مؤثره و پروفایل", drugContent?.MavadMoaserProfile)
            }
            .Where(item => !string.IsNullOrWhiteSpace(item.Content))
            .ToArray();

            var dosageFormNames = dosageForms
                .Select(item => item.PersianName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.Ordinal)
                .ToArray();
            var defaultTitle = string.Join(
                " | ",
                new[] { persianName, genericName }.Concat(dosageFormNames).Where(name => !string.IsNullOrWhiteSpace(name)));
            var metaDescription = genericInfo.PageDescription;
            if (string.IsNullOrWhiteSpace(metaDescription))
            {
                metaDescription = Regex.Replace(
                    WebUtility.HtmlDecode(LegacyDrugContentDecryptor.DecryptData(genericInfo.MavaredMasraf)),
                    @"<[^>]*>",
                    " ");
                metaDescription = Regex.Replace(metaDescription, @"\s+", " ").Trim();
            }
            if (string.IsNullOrWhiteSpace(metaDescription))
            {
                metaDescription = $"اطلاعات دارویی {persianName} ({genericName}) در دارویاب.";
            }

            var model = new DrugInfoPageViewModel
            {
                DrugId = DrugId.Value,
                EnglishName = genericName,
                PersianName = persianName,
                PageTitle = genericInfo.PageTitle,
                PageDescription = metaDescription,
                MartindaleNameEn = GetMartindaleEnglishName(martindaleGroup),
                MartindaleNameFa = GetMartindalePersianName(martindaleGroup),
                MartindaleGroupId = martindaleGroup?.GroupId,
                AhfsPath = ahfsPath,
                GeneralInformation = genericInfo.GeneralInformation,
                InformationSections = informationSections,
                PregnancyCategoryCode = genericInfo.PregnancyCategory?.PregnancyCategoryCode,
                PregnancyCategoryDescription = genericInfo.PregnancyCategory?.PregnancyCategoryDesFa,
                DosageForms = dosageForms,
                PersianBrandNames = brandRows
                    .Where(item => item.DrugNameLanguage == false && !string.IsNullOrWhiteSpace(item.DrugName))
                    .Select(item => item.DrugName!)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(name => name)
                    .ToArray(),
                EnglishBrandNames = brandRows
                    .Where(item => item.DrugNameLanguage == true && !string.IsNullOrWhiteSpace(item.DrugName))
                    .Select(item => item.DrugName!)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(name => name)
                    .ToArray(),
                RelatedDrugs = relatedDrugs,
                Comments = comments,
                DrugsComUrl = genericInfo.DrugsComURL,
                MedscapeUrl = genericInfo.MedscapeComURL
            };

            ViewData["Title"] = model.PageTitle ?? (string.IsNullOrWhiteSpace(defaultTitle) ? genericName : defaultTitle);
            ViewData["MetaDescription"] = model.PageDescription;
            const string siteUrl = "https://www.darooyab.ir";
            var canonicalUrl = $"{siteUrl}/G-{DrugId.Value}/{canonicalSlug}";
            ViewData["CanonicalUrl"] = canonicalUrl;
            ViewData["OpenGraphTitle"] = ViewData["Title"];
            ViewData["OpenGraphDescription"] = model.PageDescription;
            ViewData["OpenGraphUrl"] = canonicalUrl;
            ViewData["StructuredData"] = JsonSerializer.Serialize(new Dictionary<string, object?>
            {
                ["@context"] = "https://schema.org",
                ["@type"] = "MedicalWebPage",
                ["name"] = $"{model.PersianName} ({model.EnglishName})",
                ["description"] = $"راهنمای کامل مصرف، دوز، عوارض و تداخلات دارویی {model.PersianName}",
                ["about"] = new Dictionary<string, object?>
                {
                    ["@type"] = "Drug",
                    ["name"] = model.EnglishName,
                    ["alternateName"] = model.PersianName,
                    ["drugClass"] = model.MartindaleNameFa
                },
                ["publisher"] = new Dictionary<string, object?>
                {
                    ["@type"] = "Organization",
                    ["name"] = "دارویاب",
                    ["url"] = siteUrl
                }
            });
            ViewData["DrugId"] = DrugId.Value;
            return View(model);
        }
        catch (SqlException exception)
        {
            _logger.LogError(
                "Could not load drug information for drug {DrugId}. SQL error number: {SqlErrorNumber}.",
                DrugId.Value,
                exception.Number);
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
    }

    private async Task<IReadOnlyList<string>> GetAhfsPathAsync(
        int? ahfsId,
        CancellationToken cancellationToken)
    {
        if (!ahfsId.HasValue)
        {
            return [];
        }

        var groups = await _context.Ahfsgroups
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        var currentGroup = groups.FirstOrDefault(item => item.Id == ahfsId.Value);
        if (string.IsNullOrWhiteSpace(currentGroup?.Ahfscode))
        {
            return [];
        }

        var levels = new List<Ahfsgroup>();
        var codeParts = currentGroup.Ahfscode.Split('.').ToList();
        for (var index = codeParts.Count; index > 0; index--)
        {
            var code = string.Join(".", codeParts.Take(index));
            var level = groups.FirstOrDefault(item => item.Ahfscode == code);
            if (level is not null)
            {
                levels.Add(level);
            }
        }

        var rootCode = currentGroup.Ahfscode.Split(':')[0] + ":00";
        var root = groups.FirstOrDefault(item => item.Ahfscode == rootCode);
        if (root is not null)
        {
            levels.Add(root);
        }

        return levels
            .AsEnumerable()
            .Reverse()
            .Select(item => item.NameFa)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.Ordinal)
            .Select(name => name!)
            .ToArray()!;
    }

    private static string? GetMartindaleEnglishName(darooyab.Models.Group1? group)
    {
        if (string.IsNullOrWhiteSpace(group?.GroupName))
        {
            return null;
        }

        var separatorIndex = group.GroupName.IndexOf('<');
        return (separatorIndex >= 0 ? group.GroupName[..separatorIndex] : group.GroupName).Trim();
    }

    private static string? GetMartindalePersianName(darooyab.Models.Group1? group)
    {
        if (!string.IsNullOrWhiteSpace(group?.GroupNameFa))
        {
            return group.GroupNameFa.Trim();
        }

        if (string.IsNullOrWhiteSpace(group?.GroupName))
        {
            return null;
        }

        var separatorIndex = group.GroupName.IndexOf('>');
        return separatorIndex >= 0 ? group.GroupName[(separatorIndex + 1)..].Trim() : null;
    }

    private async Task<IReadOnlyList<DrugRelatedItem>> GetRelatedDrugsAsync(
        int drugId,
        int? ahfsId,
        CancellationToken cancellationToken)
    {
        if (!ahfsId.HasValue)
        {
            return [];
        }

        var relatedDrugIds = await _context.DrugsGenericInfos
            .AsNoTracking()
            .Where(item => item.AhfsId == ahfsId && item.DrugId != drugId && item.DrugId.HasValue)
            .Select(item => item.DrugId!.Value)
            .Distinct()
            .Take(100)
            .ToListAsync(cancellationToken);

        if (relatedDrugIds.Count == 0)
        {
            return [];
        }

        var names = await _context.DrugsNames
            .AsNoTracking()
            .Where(item =>
                relatedDrugIds.Contains(item.DrugId) &&
                (item.DrugNameType == "A" || (item.DrugNameType == "F" && item.DrugNameLanguage == false)))
            .Select(item => new { item.DrugId, item.DrugName, item.DrugNameType })
            .ToListAsync(cancellationToken);

        return names
            .GroupBy(item => item.DrugId)
            .Select(group =>
            {
                var englishName = group.FirstOrDefault(item => item.DrugNameType == "A")?.DrugName;
                var persianName = group.FirstOrDefault(item => item.DrugNameType == "F")?.DrugName;
                return string.IsNullOrWhiteSpace(englishName) || string.IsNullOrWhiteSpace(persianName)
                    ? null
                    : new DrugRelatedItem(group.Key, persianName!, englishName!);
            })
            .Where(item => item is not null)
            .Select(item => item!)
            .OrderBy(item => item.PersianName)
            .Take(12)
            .ToArray();
    }

    private IQueryable<DrugSearchMatch> SearchDrugNames(string searchText)
    {
        var matches =
            from item in _context.DrugsNames.AsNoTracking()
            join generic in _context.DrugsNames
                    .AsNoTracking()
                    .Where(name =>
                        name.DrugNameType == "A" &&
                        name.DrugName != null &&
                        name.DrugName != string.Empty)
                on item.DrugId equals generic.DrugId
            where item.DrugName != null && item.DrugName.Contains(searchText)
            select new
            {
                item.DrugId,
                Name = item.DrugName!,
                GenericName = generic.DrugName!,
                item.Irc
            };

        return matches
            .Distinct()
            .OrderByDescending(item => item.Name.StartsWith(searchText))
            .ThenBy(item => item.Name)
            .ThenBy(item => item.DrugId)
            .Select(item => new DrugSearchMatch(item.DrugId, item.Name, item.GenericName, item.Irc));
    }

    private async Task<IReadOnlyDictionary<long, string>> GetBrandEnglishNamesAsync(
        IReadOnlyCollection<DrugSearchMatch> matches,
        CancellationToken cancellationToken)
    {
        var ircs = matches
            .Where(match => match.Irc.HasValue)
            .Select(match => match.Irc!.Value)
            .Distinct()
            .ToArray();
        if (ircs.Length == 0)
        {
            return new Dictionary<long, string>();
        }

        var rows = await _context.DrugsNames
            .AsNoTracking()
            .Where(item =>
                item.Irc.HasValue &&
                ircs.Contains(item.Irc.Value) &&
                item.DrugNameLanguage == true)
            .OrderByDescending(item => item.Id)
            .Select(item => new { Irc = item.Irc!.Value, item.DrugName })
            .ToListAsync(cancellationToken);

        return rows
            .Where(item => !string.IsNullOrWhiteSpace(item.DrugName))
            .GroupBy(item => item.Irc)
            .ToDictionary(group => group.Key, group => group.First().DrugName!);
    }

    private DrugSearchResult CreateDrugSearchResult(
        DrugSearchMatch match,
        IReadOnlyDictionary<long, string> brandNames)
    {
        var genericName = match.GenericName
            ?? throw new InvalidOperationException("A matching drug record must have a generic name.");
        var detailUrl = match.Irc.HasValue && brandNames.TryGetValue(match.Irc.Value, out var brandEnglishName)
            ? Url.RouteUrl(
                "BrandInfo",
                new
                {
                    IRC = match.Irc.Value,
                    BrandEngName = DrugUrl.CreateSlug(brandEnglishName)
                })
            : Url.RouteUrl(
                "G-{DrugId}/{EngGenericName}",
                new
                {
                    DrugId = match.DrugId,
                    EngGenericName = DrugUrl.CreateSlug(genericName)
                });
        if (detailUrl is null)
        {
            throw new InvalidOperationException("The drug detail route is not registered.");
        }

        return new DrugSearchResult(match.Name, genericName, detailUrl);
    }

    private BrandRelatedItem CreateRelatedBrand(long irc, string englishName, string? persianName)
    {
        var url = Url.RouteUrl(
            "BrandInfo",
            new
            {
                IRC = irc,
                BrandEngName = DrugUrl.CreateSlug(englishName)
            }) ?? throw new InvalidOperationException("The brand detail route is not registered.");

        return new BrandRelatedItem(irc, englishName, persianName, url);
    }

    private static string BuildProductMetaDescription(
        string brandName,
        string pharmacyName,
        string provinceName,
        string cityName,
        DateTime? expireDate,
        DateTime today)
    {
        if (!expireDate.HasValue)
        {
            return $"اطلاعات موجودی فعال {brandName} در داروخانه {pharmacyName} در {cityName} ثبت نشده است؛ برای اطمینان با داروخانه تماس بگیرید.";
        }

        var daysRemaining = Math.Max(0, (expireDate.Value.Date - today).Days);
        var availability = daysRemaining == 0
            ? "تا امروز"
            : $"تقریباً تا {daysRemaining} روز دیگر";
        return $"خرید {brandName} از داروخانه {pharmacyName} در استان {provinceName}، شهر {cityName}؛ موجودی ثبت‌شده {availability} معتبر است.";
    }

    private string ToAbsoluteUrl(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var absoluteUri) &&
            (absoluteUri.Scheme == Uri.UriSchemeHttp || absoluteUri.Scheme == Uri.UriSchemeHttps))
        {
            return url;
        }

        var baseUri = new Uri($"{Request.Scheme}://{Request.Host}{Request.PathBase}/");
        return new Uri(baseUri, url).AbsoluteUri;
    }

    private string? ResolveSchemaUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.StartsWith("~/", StringComparison.Ordinal)
            ? value[2..]
            : value;
        if (Uri.TryCreate(normalized, UriKind.Absolute, out var absoluteUri))
        {
            return absoluteUri.Scheme == Uri.UriSchemeHttp ||
                absoluteUri.Scheme == Uri.UriSchemeHttps
                ? absoluteUri.AbsoluteUri
                : null;
        }

        if (!normalized.StartsWith("/", StringComparison.Ordinal))
        {
            normalized = $"/{normalized}";
        }

        return new Uri(new Uri("https://www.darooyab.ir/"), normalized).AbsoluteUri;
    }

    private static Dictionary<string, object?> CreatePharmacyStructuredData(
        PharmacyDetailViewModel model,
        string canonicalUrl)
    {
        return new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "Pharmacy",
            ["name"] = model.Name,
            ["address"] = new Dictionary<string, object?>
            {
                ["@type"] = "PostalAddress",
                ["addressRegion"] = model.Province,
                ["addressLocality"] = model.City,
                ["streetAddress"] = model.Address
            },
            ["telephone"] = model.Phone ?? string.Empty,
            ["url"] = canonicalUrl
        };
    }

    private static Dictionary<string, object?> CreateProductStructuredData(
        PharmacyDetailViewModel model,
        string pharmacyUrl,
        DateTime? expireDate,
        DateTime today)
    {
        var pharmacy = new Dictionary<string, object?>
        {
            ["@type"] = "Pharmacy",
            ["@id"] = $"{pharmacyUrl}/#pharmacy",
            ["name"] = model.Name,
            ["url"] = pharmacyUrl,
            ["telephone"] = model.Phone ?? string.Empty,
            ["address"] = new Dictionary<string, object?>
            {
                ["@type"] = "PostalAddress",
                ["addressRegion"] = model.Province,
                ["addressLocality"] = model.City,
                ["addressCountry"] = "IR",
                ["streetAddress"] = model.Address
            }
        };

        var data = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@graph"] = new object[]
            {
                pharmacy,
                new Dictionary<string, object?>
                {
                    ["@type"] = "IndividualProduct",
                    ["@id"] = $"{pharmacyUrl}?IRC={model.Irc}#product",
                    ["name"] = model.BrandName,
                    ["alternateName"] = model.GenericName,
                    ["identifier"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "PropertyValue",
                        ["propertyID"] = "IRC",
                        ["value"] = model.Irc?.ToString(
                            System.Globalization.CultureInfo.InvariantCulture)
                    },
                    ["offers"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "Offer",
                        ["seller"] = new Dictionary<string, object?>
                        {
                            ["@id"] = $"{pharmacyUrl}/#pharmacy"
                        },
                        ["availability"] = expireDate.HasValue && expireDate.Value.Date >= today.Date
                            ? "https://schema.org/InStock"
                            : "https://schema.org/OutOfStock",
                        ["validThrough"] = expireDate?.ToString(
                            "yyyy-MM-dd",
                            System.Globalization.CultureInfo.InvariantCulture),
                        ["description"] = expireDate.HasValue && expireDate.Value.Date >= today.Date
                            ? $"اعلام موجودی بر اساس خوداظهاری داروخانه (تعداد روز موجود: {(expireDate.Value.Date - today.Date).Days} روز)"
                            : "اعلام موجودی بر اساس خوداظهاری داروخانه (موجودی به اتمام رسیده است)"
                    }
                }
            }
        };

        return data;
    }

    private sealed record DrugSearchMatch(
        int DrugId,
        string Name,
        string? GenericName,
        long? Irc);

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
