using System.Globalization;
using System.Text.RegularExpressions;
using darooyab.Models;
using darooyab_rewrite.Areas.Admin.ViewModels;
using darooyab_rewrite.Services.Authentication;
using darooyab_rewrite.Services.Authentication.Pharmacy;
using darooyab_rewrite.Services.PharmacySpecialties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace darooyab_rewrite.Areas.Admin.Controllers;

[Area("Admin")]
[Route("/DarooyabPharmacy")]
[Authorize(Policy = PharmacyAuthenticationDefaults.AuthorizationPolicy)]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class DarooyabPharmacyController(
    darooConContext context,
    IAuthenticationService authenticationService,
    PharmacySpecialtiesService specialtiesService,
    IWebHostEnvironment environment,
    ILogger<DarooyabPharmacyController> logger) : Controller
{
    private const string ImageUrlPrefix = "/Content/Images/PharmacyImages/";
    private const int MaximumImageRequestSize = 200 * 1024;
    private static readonly Regex MobilePattern = new("^09[0-9]{9}$", RegexOptions.Compiled);
    private static readonly string PersianDigits = "۰۱۲۳۴۵۶۷۸۹";
    private static readonly string ArabicDigits = "٠١٢٣٤٥٦٧٨٩";

    [HttpGet("PharmacySpecialties")]
    public async Task<IActionResult> PharmacySpecialties(
        int? active = 2,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            return await ExpiredAccountChallengeAsync();
        }

        var model = await specialtiesService.GetPageAsync(
            pharmacyAccountId.Value,
            Math.Clamp(active ?? 2, 1, 6),
            cancellationToken);
        if (model is null)
        {
            return NotFound();
        }

        ViewData["Title"] = "مدیریت اطلاعات داروخانه";
        ViewData["PharmacyName"] = model.PharmacyName;
        return View(model);
    }

    [HttpPost("PharmacySpecialties")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePharmacySpecialties(
        int[]? selectedSpecialtyIds,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            return await ExpiredAccountChallengeAsync();
        }

        var result = await specialtiesService.SaveSpecialtiesAsync(
            pharmacyAccountId.Value,
            selectedSpecialtyIds,
            cancellationToken);
        if (result != PharmacySpecialtiesSaveStatus.Succeeded)
        {
            TempData["PharmacyManagementError"] = result == PharmacySpecialtiesSaveStatus.InvalidCount
                ? "حداکثر می‌توانید ۲ زمینه تخصصی برای داروخانه انتخاب کنید."
                : "یک یا چند زمینه تخصصی انتخاب‌شده معتبر یا فعال نیست.";
            return RedirectToAction(nameof(PharmacySpecialties), new { active = 2 });
        }

        TempData["PharmacyManagementSuccess"] = "زمینه‌های تخصصی داروخانه با موفقیت ثبت شد.";
        return RedirectToAction(nameof(PharmacySpecialties), new { active = 2 });
    }

    [HttpPost("/PharmacyCityDistricts/Index")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveDistrict(
        int? districtId,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            return await ExpiredAccountChallengeAsync();
        }

        var succeeded = await specialtiesService.SaveDistrictAsync(
            pharmacyAccountId.Value,
            districtId,
            cancellationToken);
        TempData[succeeded ? "PharmacyManagementSuccess" : "PharmacyManagementError"] = succeeded
            ? "منطقه داروخانه با موفقیت ثبت شد."
            : districtId.HasValue
                ? "منطقه انتخاب‌شده معتبر نیست."
                : "لطفاً یک منطقه انتخاب کنید.";
        return RedirectToAction(nameof(PharmacySpecialties), new { active = 1 });
    }

    [HttpPost("/PharmacyContactInfo/Save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveContactInfo(
        PharmacyContactForm form,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            return await ExpiredAccountChallengeAsync();
        }

        form.PharmacyManagerMobile = NormalizeMobile(form.PharmacyManagerMobile);
        form.InventoryManagerMobile = NormalizeMobile(form.InventoryManagerMobile);
        form.PharmacyTechnicianMobile = NormalizeMobile(form.PharmacyTechnicianMobile);
        if (!IsValidMobile(form.PharmacyManagerMobile) ||
            !IsValidMobile(form.InventoryManagerMobile) ||
            !IsValidMobile(form.PharmacyTechnicianMobile))
        {
            TempData["PharmacyManagementError"] = "شماره موبایل باید خالی باشد یا با 09 شروع شود و ۱۱ رقم داشته باشد.";
            return RedirectToAction(nameof(PharmacySpecialties), new { active = 5 });
        }

        var succeeded = await specialtiesService.SaveContactsAsync(
            pharmacyAccountId.Value,
            form,
            cancellationToken);
        TempData[succeeded ? "PharmacyManagementSuccess" : "PharmacyManagementError"] = succeeded
            ? "اطلاعات تماس با موفقیت ذخیره شد."
            : "اطلاعات داروخانه یافت نشد.";
        return RedirectToAction(nameof(PharmacySpecialties), new { active = 5 });
    }

    [HttpPost("SavePharmacyInsurances")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePharmacyInsurances(
        PharmacyInsuranceForm form,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            return await ExpiredAccountChallengeAsync();
        }

        var result = await specialtiesService.SaveInsurancesAsync(
            pharmacyAccountId.Value,
            form.SelectedBaseInsuranceIds,
            form.SelectedSupplementaryInsuranceIds,
            cancellationToken);
        if (result != PharmacyInsuranceSaveStatus.Succeeded)
        {
            TempData["PharmacyManagementError"] = result == PharmacyInsuranceSaveStatus.BaseInsuranceRequired
                ? "انتخاب حداقل یک بیمه پایه الزامی است."
                : "یک یا چند بیمه انتخاب‌شده معتبر یا فعال نیست.";
            return RedirectToAction(nameof(PharmacySpecialties), new { active = 6 });
        }

        TempData["PharmacyManagementSuccess"] = "بیمه‌های طرف قرارداد با موفقیت ثبت شدند.";
        return RedirectToAction(nameof(PharmacySpecialties), new { active = 6 });
    }

    [HttpPost("/GeoLocation/SetPharmacyGeoLocarion")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPharmacyGeoLocarion(
        string? latitude,
        string? longitude,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            return Unauthorized(new { success = false, message = "نشست کاربری معتبر نیست." });
        }

        if (!TryParseCoordinate(latitude, out var parsedLatitude) ||
            !TryParseCoordinate(longitude, out var parsedLongitude))
        {
            return BadRequest(new { success = false, message = "مختصات معتبر نیست." });
        }

        var succeeded = await specialtiesService.SaveLocationAsync(
            pharmacyAccountId.Value,
            parsedLatitude,
            parsedLongitude,
            cancellationToken);
        return succeeded
            ? Json(new { success = true })
            : BadRequest(new { success = false, message = "مختصات خارج از محدوده معتبر است." });
    }

    [HttpPost("ManageImages")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaximumImageRequestSize)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaximumImageRequestSize)]
    public async Task<IActionResult> ManageImages(
        IFormFile? image1File,
        IFormFile? image2File,
        IFormFile? image3File,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            return await ExpiredAccountChallengeAsync();
        }

        var currentUrls = await specialtiesService.GetImageUrlsAsync(
            pharmacyAccountId.Value,
            cancellationToken);
        if (currentUrls.Length != 3)
        {
            return NotFound();
        }

        var uploads = new[]
        {
            (Number: 1, File: image1File),
            (Number: 2, File: image2File),
            (Number: 3, File: image3File)
        }.Where(item => item.File is { Length: > 0 }).ToArray();

        if (uploads.Any(item => item.Number == 2) &&
            string.IsNullOrEmpty(currentUrls[0]) &&
            image1File is not { Length: > 0 })
        {
            TempData["PharmacyManagementError"] = "برای بارگذاری تصویر دوم ابتدا تصویر اول را ثبت کنید.";
            return RedirectToAction(nameof(PharmacySpecialties), new { active = 4 });
        }

        if (uploads.Any(item => item.Number == 3) &&
            string.IsNullOrEmpty(currentUrls[1]) &&
            image2File is not { Length: > 0 })
        {
            TempData["PharmacyManagementError"] = "برای بارگذاری تصویر سوم ابتدا تصویر دوم را ثبت کنید.";
            return RedirectToAction(nameof(PharmacySpecialties), new { active = 4 });
        }

        var validatedUploads = new List<(int Number, string Extension, byte[] Content)>();
        foreach (var (number, file) in uploads)
        {
            var extension = Path.GetExtension(file!.FileName);
            await using var stream = new MemoryStream();
            await file.CopyToAsync(stream, cancellationToken);
            var content = stream.ToArray();
            if (!PharmacyImageFileValidator.TryValidate(
                    content,
                    extension,
                    file.ContentType,
                    out var validationError))
            {
                TempData["PharmacyManagementError"] = validationError;
                return RedirectToAction(nameof(PharmacySpecialties), new { active = 4 });
            }

            validatedUploads.Add((number, extension.ToLowerInvariant(), content));
        }

        var savedFiles = new List<string>();
        var imageCleanupWarning = false;
        try
        {
            var replacements = new Dictionary<int, string?>();
            foreach (var upload in validatedUploads)
            {
                var fileName = $"{Guid.NewGuid():N}{upload.Extension}";
                var fullPath = GetManagedImagePath(fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                await System.IO.File.WriteAllBytesAsync(fullPath, upload.Content, cancellationToken);
                savedFiles.Add(fullPath);
                replacements.Add(upload.Number, ImageUrlPrefix + fileName);
            }

            if (!await specialtiesService.SaveImageUrlsAsync(
                    pharmacyAccountId.Value,
                    replacements,
                    cancellationToken))
            {
                DeleteFiles(savedFiles);
                return NotFound();
            }

            foreach (var upload in validatedUploads)
            {
                try
                {
                    DeleteManagedImage(currentUrls[upload.Number - 1]);
                }
                catch (IOException exception)
                {
                    imageCleanupWarning = true;
                    logger.LogError(exception, "Could not remove replaced pharmacy image for account {PharmacyAccountId}.", pharmacyAccountId);
                }
                catch (UnauthorizedAccessException exception)
                {
                    imageCleanupWarning = true;
                    logger.LogError(exception, "Access was denied while removing a replaced pharmacy image for account {PharmacyAccountId}.", pharmacyAccountId);
                }
            }
        }
        catch (IOException exception)
        {
            DeleteFiles(savedFiles);
            logger.LogError(exception, "Could not save pharmacy images for account {PharmacyAccountId}.", pharmacyAccountId);
            TempData["PharmacyManagementError"] = "ذخیره تصاویر انجام نشد؛ لطفاً دوباره تلاش کنید.";
            return RedirectToAction(nameof(PharmacySpecialties), new { active = 4 });
        }
        catch (UnauthorizedAccessException exception)
        {
            DeleteFiles(savedFiles);
            logger.LogError(exception, "Access was denied while saving pharmacy images for account {PharmacyAccountId}.", pharmacyAccountId);
            TempData["PharmacyManagementError"] = "ذخیره تصاویر به دلیل محدودیت دسترسی انجام نشد.";
            return RedirectToAction(nameof(PharmacySpecialties), new { active = 4 });
        }
        catch (DbUpdateException exception)
        {
            DeleteFiles(savedFiles);
            logger.LogError(exception, "Could not update pharmacy image URLs for account {PharmacyAccountId}.", pharmacyAccountId);
            TempData["PharmacyManagementError"] = "ذخیره اطلاعات تصاویر در سامانه انجام نشد.";
            return RedirectToAction(nameof(PharmacySpecialties), new { active = 4 });
        }

        TempData[imageCleanupWarning ? "PharmacyManagementError" : "PharmacyManagementSuccess"] = imageCleanupWarning
            ? "تصاویر جدید ذخیره شدند، اما پاک‌سازی یک یا چند فایل قبلی انجام نشد."
            : "تصاویر با موفقیت ذخیره شدند.";
        return RedirectToAction(nameof(PharmacySpecialties), new { active = 4 });
    }

    [HttpPost("DeleteImage")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(
        int imageNumber,
        CancellationToken cancellationToken = default)
    {
        var pharmacyAccountId = await GetCurrentPharmacyAccountIdAsync(cancellationToken);
        if (!pharmacyAccountId.HasValue)
        {
            return Unauthorized(new { success = false, message = "نشست کاربری معتبر نیست." });
        }

        if (imageNumber is < 1 or > 3)
        {
            return BadRequest(new { success = false, message = "شماره تصویر نامعتبر است." });
        }

        var imageUrls = await specialtiesService.GetImageUrlsAsync(
            pharmacyAccountId.Value,
            cancellationToken);
        if (imageUrls.Length != 3)
        {
            return NotFound(new { success = false, message = "داروخانه یافت نشد." });
        }

        var oldUrl = imageUrls[imageNumber - 1];
        if (!await specialtiesService.SaveImageUrlsAsync(
                pharmacyAccountId.Value,
                new Dictionary<int, string?> { [imageNumber] = null },
                cancellationToken))
        {
            return NotFound(new { success = false, message = "داروخانه یافت نشد." });
        }

        try
        {
            DeleteManagedImage(oldUrl);
        }
        catch (IOException exception)
        {
            logger.LogError(
                exception,
                "Could not remove pharmacy image {ImageNumber} for account {PharmacyAccountId}.",
                imageNumber,
                pharmacyAccountId);
            return Json(new
            {
                success = true,
                message = "تصویر از اطلاعات داروخانه حذف شد، اما پاک‌سازی فایل فیزیکی انجام نشد."
            });
        }
        catch (UnauthorizedAccessException exception)
        {
            logger.LogError(
                exception,
                "Access was denied while removing pharmacy image {ImageNumber} for account {PharmacyAccountId}.",
                imageNumber,
                pharmacyAccountId);
            return Json(new
            {
                success = true,
                message = "تصویر از اطلاعات داروخانه حذف شد، اما پاک‌سازی فایل فیزیکی انجام نشد."
            });
        }

        return Json(new { success = true, message = "تصویر با موفقیت حذف شد." });
    }

    private async Task<int?> GetCurrentPharmacyAccountIdAsync(CancellationToken cancellationToken)
    {
        var user = authenticationService.GetCurrentUser();
        if (user is null ||
            user.UserType != PharmacyAuthenticationDefaults.UserType ||
            !int.TryParse(user.UserId, NumberStyles.None, CultureInfo.InvariantCulture, out var id))
        {
            return null;
        }

        var active = await context.PharmacyAccounts
            .AsNoTracking()
            .AnyAsync(account =>
                    account.Id == id &&
                    account.AccountStatus == "2" &&
                    account.DarooyabAdminApproved != false &&
                    (account.AccountExpireDate == null || account.AccountExpireDate >= DateTime.Now),
                cancellationToken);
        return active ? id : null;
    }

    private async Task<IActionResult> ExpiredAccountChallengeAsync()
    {
        await authenticationService.LogoutAsync();
        return Challenge();
    }

    private string GetManagedImagePath(string fileName) =>
        Path.Combine(
            environment.WebRootPath,
            "Content",
            "Images",
            "PharmacyImages",
            Path.GetFileName(fileName));

    private void DeleteManagedImage(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return;
        }

        var path = imageUrl.StartsWith("~/", StringComparison.Ordinal)
            ? imageUrl[1..]
            : imageUrl;
        if (!path.StartsWith(ImageUrlPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var fileName = path[ImageUrlPrefix.Length..];
        if (string.IsNullOrWhiteSpace(fileName) ||
            Path.GetFileName(fileName) != fileName)
        {
            return;
        }

        var fullPath = GetManagedImagePath(fileName);
        if (System.IO.File.Exists(fullPath))
        {
            System.IO.File.Delete(fullPath);
        }
    }

    private void DeleteFiles(IEnumerable<string> paths)
    {
        foreach (var path in paths)
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            catch (IOException exception)
            {
                logger.LogError(exception, "Could not remove temporary pharmacy image file.");
            }
            catch (UnauthorizedAccessException exception)
            {
                logger.LogError(exception, "Access was denied while removing temporary pharmacy image file.");
            }
        }
    }

    private static string? NormalizeMobile(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim().Select(character =>
        {
            var persianIndex = PersianDigits.IndexOf(character);
            if (persianIndex >= 0)
            {
                return (char)('0' + persianIndex);
            }

            var arabicIndex = ArabicDigits.IndexOf(character);
            return arabicIndex >= 0 ? (char)('0' + arabicIndex) : character;
        });
        return new string(normalized.ToArray());
    }

    private static bool IsValidMobile(string? value) =>
        string.IsNullOrEmpty(value) || MobilePattern.IsMatch(value);

    private static bool TryParseCoordinate(string? value, out decimal coordinate)
    {
        var normalized = value?.Replace(',', '.');
        return decimal.TryParse(
            normalized,
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out coordinate);
    }
}
