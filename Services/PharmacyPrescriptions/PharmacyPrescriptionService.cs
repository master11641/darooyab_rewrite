using System.Globalization;
using darooyab.Models;
using darooyab_rewrite.Areas.Admin.ViewModels;
using darooyab_rewrite.Services.PharmacyRequests;
using darooyab_rewrite.Utilities;
using Microsoft.EntityFrameworkCore;

namespace darooyab_rewrite.Services.PharmacyPrescriptions;

public sealed class PharmacyPrescriptionService(
    darooConContext context,
    IPharmacyWalletService walletService,
    IPharmacyPrescriptionSmsSender smsSender) : IPharmacyPrescriptionService
{
    private const int PageSize = 10;
    private const long SmsCostInRial = 50_000;

    public async Task<PharmacyPrescriptionPageViewModel> GetPageAsync(
        int pharmacyAccountId,
        string? trackingCode,
        bool? isReviewed,
        int page,
        CancellationToken cancellationToken = default)
    {
        var pharmacyId = await GetPharmacyIdAsync(pharmacyAccountId, cancellationToken);
        var query = context.DarooyabPrescriptions
            .AsNoTracking()
            .Where(item => item.PharmacyId == pharmacyId);

        if (!string.IsNullOrEmpty(trackingCode))
        {
            query = query.Where(item => item.TrackingCode!.Contains(trackingCode));
        }

        if (isReviewed.HasValue)
        {
            query = isReviewed.Value
                ? query.Where(item => item.IsReviewed == true)
                : query.Where(item => item.IsReviewed != true);
        }

        var totalRecords = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);
        var currentPage = Math.Clamp(page, 1, Math.Max(totalPages, 1));
        var rows = await query
            .OrderByDescending(item => item.Id)
            .Skip((currentPage - 1) * PageSize)
            .Take(PageSize)
            .Select(item => new
            {
                item.Id,
                item.IRC,
                item.RegistrationDate,
                item.RegistrationUserName,
                item.MobileNumber,
                item.UploadImageUrl,
                item.InsuranceType,
                item.NationalCode,
                item.TrackingCode,
                item.UserId,
                item.IsReviewed
            })
            .ToListAsync(cancellationToken);

        var ircs = rows
            .Where(item => item.IRC != 0)
            .Select(item => item.IRC)
            .Distinct()
            .ToArray();
        var drugNames = ircs.Length == 0
            ? new Dictionary<long, string>()
            : (await context.DrugsNames
                .AsNoTracking()
                .Where(item => item.Irc.HasValue &&
                    ircs.Contains(item.Irc.Value) &&
                    item.DrugNameLanguage == false)
                .Select(item => new { item.Irc, item.DrugName })
                .ToListAsync(cancellationToken))
                .GroupBy(item => item.Irc!.Value)
                .ToDictionary(group => group.Key, group => group.First().DrugName ?? string.Empty);
        var prescriptionIds = rows.Select(item => item.Id).ToArray();
        var smsSentPrescriptionIds = prescriptionIds.Length == 0
            ? new HashSet<int>()
            : (await context.DarooyabPrescriptionSMSMessages
                .AsNoTracking()
                .Where(message =>
                    message.IsSuccess == true &&
                    message.PharmacyId == pharmacyId &&
                    prescriptionIds.Contains(message.PrescriptionId))
                .Select(message => message.PrescriptionId)
                .Distinct()
                .ToListAsync(cancellationToken))
                .ToHashSet();
        var items = rows.Select(item => new PharmacyPrescriptionItemViewModel(
            item.Id,
            item.TrackingCode ?? string.Empty,
            item.IRC == 0 ? string.Empty : drugNames.GetValueOrDefault(item.IRC, string.Empty),
            item.RegistrationUserName ?? string.Empty,
            item.NationalCode ?? string.Empty,
            item.MobileNumber ?? string.Empty,
            GetInsuranceTypeTitle(item.InsuranceType),
            PersianDateFormatter.Format(item.RegistrationDate),
            item.UserId is null ? "وب سایت" : "اپلیکیشن",
            item.IsReviewed,
            GetUploadImageUrl(item.UploadImageUrl),
            smsSentPrescriptionIds.Contains(item.Id))).ToArray();

        return new PharmacyPrescriptionPageViewModel
        {
            TrackingCode = trackingCode,
            IsReviewed = isReviewed,
            Page = currentPage,
            TotalPages = totalPages,
            TotalRecords = totalRecords,
            Items = items
        };
    }

    public async Task<bool> UpdateReviewedAsync(
        int pharmacyAccountId,
        int prescriptionId,
        bool isReviewed,
        CancellationToken cancellationToken = default)
    {
        var pharmacyId = await GetPharmacyIdAsync(pharmacyAccountId, cancellationToken);
        var prescription = await context.DarooyabPrescriptions
            .SingleOrDefaultAsync(
                item => item.Id == prescriptionId && item.PharmacyId == pharmacyId,
                cancellationToken);
        if (prescription is null)
        {
            return false;
        }

        prescription.IsReviewed = isReviewed;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PharmacyPrescriptionSmsViewModel?> GetSmsPageAsync(
        int pharmacyAccountId,
        int prescriptionId,
        string? statusMessage,
        CancellationToken cancellationToken = default)
    {
        var account = await GetSmsAccountAsync(pharmacyAccountId, cancellationToken);
        var prescription = await context.DarooyabPrescriptions
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.Id == prescriptionId && item.PharmacyId == account.PharmacyIdFk,
                cancellationToken);
        if (prescription is null)
        {
            return null;
        }

        var drugName = prescription.IRC == 0
            ? string.Empty
            : await context.DrugsNames
                .AsNoTracking()
                .Where(item =>
                    item.Irc == prescription.IRC &&
                    item.DrugNameLanguage == false)
                .Select(item => item.DrugName)
                .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;
        var pharmacy = account.PharmacyIdFkNavigation;
        var balanceInRial = await walletService.GetBalanceInRialAsync(
            pharmacyAccountId,
            cancellationToken);

        return new PharmacyPrescriptionSmsViewModel
        {
            PrescriptionId = prescription.Id,
            PharmacyName = pharmacy.Name,
            Recipient = $"{prescription.RegistrationUserName}({prescription.MobileNumber})",
            DrugName = drugName,
            Message = BuildSmsMessage(prescription, pharmacy),
            BalanceInRial = balanceInRial,
            SmsConfigured = smsSender.IsConfigured,
            HasValidRecipient = !string.IsNullOrWhiteSpace(prescription.MobileNumber),
            StatusMessage = statusMessage
        };
    }

    public async Task<PharmacyPrescriptionSmsSendResult> SendSmsAsync(
        int pharmacyAccountId,
        int prescriptionId,
        CancellationToken cancellationToken = default)
    {
        if (!smsSender.IsConfigured)
        {
            return new PharmacyPrescriptionSmsSendResult(
                PharmacyPrescriptionSmsSendStatus.NotConfigured,
                "ارسال پیامک در این محیط پیکربندی نشده است.");
        }

        var account = await GetSmsAccountAsync(pharmacyAccountId, cancellationToken);
        var prescription = await context.DarooyabPrescriptions
            .SingleOrDefaultAsync(
                item => item.Id == prescriptionId && item.PharmacyId == account.PharmacyIdFk,
                cancellationToken);
        if (prescription is null)
        {
            return new PharmacyPrescriptionSmsSendResult(
                PharmacyPrescriptionSmsSendStatus.NotFound,
                "نسخه‌ی موردنظر در دسترس نیست.");
        }

        if (string.IsNullOrWhiteSpace(prescription.MobileNumber))
        {
            return new PharmacyPrescriptionSmsSendResult(
                PharmacyPrescriptionSmsSendStatus.InvalidRecipient,
                "شماره‌ی همراه معتبر برای این نسخه ثبت نشده است.");
        }

        var balanceInRial = await walletService.GetBalanceInRialAsync(
            pharmacyAccountId,
            cancellationToken);
        if (balanceInRial < SmsCostInRial)
        {
            return new PharmacyPrescriptionSmsSendResult(
                PharmacyPrescriptionSmsSendStatus.InsufficientBalance,
                "برای ارسال پیامک، موجودی کیف پول کافی نیست؛ لطفاً کیف پول را شارژ کنید.");
        }

        var pharmacy = account.PharmacyIdFkNavigation;
        var message = BuildSmsMessage(prescription, pharmacy);
        var providerResult = await smsSender.SendAsync(
            prescription.MobileNumber,
            message,
            cancellationToken);
        context.DarooyabPrescriptionSMSMessages.Add(new DarooyabPrescriptionSMSMessage
        {
            StatusCode = providerResult.StatusCode.ToString(CultureInfo.InvariantCulture),
            IsSuccess = providerResult.IsSuccess,
            StatusMessage = providerResult.StatusMessage,
            SendDate = DateTime.Now,
            PharmacyAccountId = account.Id,
            PharmacyId = pharmacy.Id,
            Message = message,
            RecieverPhone = prescription.MobileNumber,
            PrescriptionId = prescription.Id
        });
        await context.SaveChangesAsync(cancellationToken);

        return providerResult.IsSuccess
            ? new PharmacyPrescriptionSmsSendResult(
                PharmacyPrescriptionSmsSendStatus.Succeeded,
                "پیام کوتاه با موفقیت ارسال شد.")
            : new PharmacyPrescriptionSmsSendResult(
                PharmacyPrescriptionSmsSendStatus.Failed,
                providerResult.StatusMessage);
    }

    private async Task<int> GetPharmacyIdAsync(
        int pharmacyAccountId,
        CancellationToken cancellationToken)
    {
        return await context.PharmacyAccounts
            .AsNoTracking()
            .Where(account => account.Id == pharmacyAccountId)
            .Select(account => account.PharmacyIdFk)
            .SingleAsync(cancellationToken);
    }

    private async Task<PharmacyAccount> GetSmsAccountAsync(
        int pharmacyAccountId,
        CancellationToken cancellationToken)
    {
        return await context.PharmacyAccounts
            .AsNoTracking()
            .Include(account => account.PharmacyIdFkNavigation)
                .ThenInclude(pharmacy => pharmacy.CityIdFkNavigation)
            .Include(account => account.PharmacyIdFkNavigation)
                .ThenInclude(pharmacy => pharmacy.ProvinceIdFkNavigation)
            .SingleAsync(account => account.Id == pharmacyAccountId, cancellationToken);
    }

    private static string BuildSmsMessage(DarooyabPrescription prescription, Pharmacy pharmacy)
    {
        var trackingCode = string.IsNullOrEmpty(prescription.TrackingCode)
            ? string.Empty
            : $"کد رهگیری : {prescription.TrackingCode}\r\n";
        var provinceName = pharmacy.ProvinceIdFkNavigation?.Name ?? string.Empty;
        var cityName = pharmacy.CityIdFkNavigation?.Name ?? string.Empty;
        return string.Format(
            CultureInfo.InvariantCulture,
            "دارویاب:\r\n نسخه کامل شما در \"داروخانه {0}\" موجود است.\r\n {1}کدملی: {2}\r\nجهت دریافت نسخه با داروخانه مربوطه تماس حاصل فرمائید:\r\nاستان {3} شهر {4} {5}- {6}\r\n(تضمین موجودی حداکثر 8 ساعت می باشد)\r\n",
            pharmacy.Name ?? string.Empty,
            trackingCode,
            prescription.NationalCode ?? string.Empty,
            provinceName,
            cityName,
            pharmacy.Address ?? string.Empty,
            pharmacy.Phone ?? string.Empty);
    }

    private static string GetInsuranceTypeTitle(InsuranceType insuranceType) =>
        insuranceType switch
        {
            InsuranceType.Ejtemaei => "تامین اجتماعی",
            InsuranceType.Darman => "خدمات درمان",
            InsuranceType.Artesh => "ارتش",
            InsuranceType.Azad => "آزاد",
            _ => insuranceType.ToString()
        };

    private static string? GetUploadImageUrl(string? uploadImageUrl)
    {
        if (string.IsNullOrWhiteSpace(uploadImageUrl))
        {
            return null;
        }

        var path = uploadImageUrl.Trim().Replace('\\', '/');
        if (Uri.TryCreate(path, UriKind.Absolute, out var absoluteUri))
        {
            return absoluteUri.Scheme == Uri.UriSchemeHttps &&
                (absoluteUri.Host.Equals("www.darooyab.ir", StringComparison.OrdinalIgnoreCase) ||
                 absoluteUri.Host.Equals("api.darooyab.ir", StringComparison.OrdinalIgnoreCase))
                    ? absoluteUri.ToString()
                    : null;
        }

        if (path.StartsWith("//", StringComparison.Ordinal) ||
            path.Split('/').Any(segment => segment == ".."))
        {
            return null;
        }

        var baseUrl = path.StartsWith("Content", StringComparison.OrdinalIgnoreCase)
            ? "https://www.darooyab.ir/"
            : "https://api.darooyab.ir/";
        return new Uri(new Uri(baseUrl), path.TrimStart('/')).ToString();
    }
}
