using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using darooyab.Models;
using darooyab_rewrite.Areas.Admin.ViewModels;
using darooyab_rewrite.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace darooyab_rewrite.Services.PharmacyRequests;

public sealed class PharmacyWalletService(
    darooConContext context,
    HttpClient httpClient,
    IOptions<ZarinPalOptions> options,
    ILogger<PharmacyWalletService> logger) : IPharmacyWalletService
{
    private const int PageSize = 20;
    private const int PurchasedRankingsPageSize = 5;
    private const long MinimumTopUpToman = 1_100;
    private const long MaximumTopUpToman = 20_000_000;
    private const string RequestPaymentEndpoint = "https://api.zarinpal.com/pg/v4/payment/request.json";
    private const string VerifyPaymentEndpoint = "https://api.zarinpal.com/pg/v4/payment/verify.json";
    private const string StartPaymentUrl = "https://www.zarinpal.com/pg/StartPay/";

    public async Task<long> GetBalanceInRialAsync(
        int pharmacyAccountId,
        CancellationToken cancellationToken = default)
    {
        var paidAmount = await GetPaidAmountInRialAsync(pharmacyAccountId, cancellationToken);
        var deductions = await GetDeductionsInRialAsync(pharmacyAccountId, cancellationToken);
        return paidAmount - deductions;
    }

    public async Task<PharmacyWalletPageViewModel> GetPageAsync(
        int pharmacyAccountId,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var paidAmount = await GetPaidAmountInRialAsync(pharmacyAccountId, cancellationToken);
        var deductions = await GetDeductionsInRialAsync(pharmacyAccountId, cancellationToken);
        var transactionQuery = context.WalletTransactions
            .AsNoTracking()
            .Where(item => item.AccountId == pharmacyAccountId);
        var count = await transactionQuery.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(count / (double)PageSize));
        var currentPage = Math.Clamp(page, 1, totalPages);
        var rows = await transactionQuery
            .OrderByDescending(item => item.RegisterDate)
            .ThenByDescending(item => item.Id)
            .Skip((currentPage - 1) * PageSize)
            .Take(PageSize)
            .Select(item => new
            {
                item.Id,
                item.RegisterDate,
                item.TransactionType,
                item.Amount,
                item.Authority,
                item.RefId,
                item.PaymentDate
            })
            .ToListAsync(cancellationToken);

        var transactions = rows
            .Select(item =>
            {
                var paid = !string.IsNullOrWhiteSpace(item.RefId) && item.PaymentDate.HasValue;
                var amountToman = (item.Amount ?? 0) / 10L;
                var type = item.TransactionType switch
                {
                    TransactionType.Cach => "شارژ",
                    TransactionType.Gift => "هدیه",
                    _ => "نامشخص"
                };
                return new PharmacyWalletTransactionItem(
                    item.Id,
                    item.RegisterDate,
                    PersianDateFormatter.Format(item.RegisterDate),
                    type,
                    amountToman,
                    item.Authority,
                    item.RefId,
                    item.PaymentDate,
                    PersianDateFormatter.Format(item.PaymentDate),
                    paid ? "پرداخت موفق" : "در انتظار تأیید پرداخت",
                    !paid && !string.IsNullOrWhiteSpace(item.Authority));
            })
            .ToArray();

        var optionsValue = options.Value;
        return new PharmacyWalletPageViewModel
        {
            PaidAmountToman = Math.Max(0, paidAmount) / 10,
            SpentAmountToman = Math.Max(0, deductions) / 10,
            BalanceToman = Math.Max(0, paidAmount - deductions) / 10,
            Transactions = transactions,
            Page = currentPage,
            TotalPages = totalPages,
            CanCharge = IsPaymentConfigured(optionsValue)
        };
    }

    public async Task<PharmacyPurchasedRankingsPageViewModel> GetPurchasedRankingsPageAsync(
        int pharmacyAccountId,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var rankingQuery = context.RankingTransactions
            .AsNoTracking()
            .Where(item => item.PharmacyAccountId == pharmacyAccountId);
        var count = await rankingQuery.CountAsync(cancellationToken);
        var totalPages = Math.Max(
            1,
            (int)Math.Ceiling(count / (double)PurchasedRankingsPageSize));
        var currentPage = Math.Clamp(page, 1, totalPages);
        var rows = await rankingQuery
            .OrderByDescending(item => item.Id)
            .Skip((currentPage - 1) * PurchasedRankingsPageSize)
            .Take(PurchasedRankingsPageSize)
            .Select(item => new
            {
                item.Irc,
                item.StartDate,
                item.EndDate,
                item.DurationInDays,
                item.RankNumber,
                item.Amount,
                item.ModifyDescription,
                DrugName = context.DrugsNames
                    .Where(drug => drug.Irc == item.Irc && drug.DrugNameLanguage == false)
                    .Select(drug => drug.DrugName)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return new PharmacyPurchasedRankingsPageViewModel
        {
            Rankings = rows
                .Select(item => new PharmacyPurchasedRankingItem(
                    item.DrugName ?? "نامشخص",
                    FormatLegacyPersianDate(item.StartDate),
                    item.DurationInDays,
                    FormatLegacyPersianExpiry(item.EndDate),
                    item.RankNumber,
                    item.Amount / 10L,
                    item.ModifyDescription))
                .ToArray(),
            Page = currentPage,
            TotalPages = totalPages
        };
    }

    public async Task<PharmacyWalletPaymentStartResult> StartTopUpAsync(
        int pharmacyAccountId,
        long amountToman,
        CancellationToken cancellationToken = default)
    {
        var configuration = options.Value;
        if (!IsPaymentConfigured(configuration))
        {
            return new PharmacyWalletPaymentStartResult(
                PharmacyWalletPaymentStartStatus.NotConfigured);
        }

        if (amountToman is < MinimumTopUpToman or > MaximumTopUpToman ||
            amountToman > int.MaxValue / 10)
        {
            return new PharmacyWalletPaymentStartResult(
                PharmacyWalletPaymentStartStatus.InvalidAmount);
        }

        var amountRial = checked((int)(amountToman * 10));
        try
        {
            using var response = await httpClient.PostAsJsonAsync(
                RequestPaymentEndpoint,
                new
                {
                    merchant_id = configuration.MerchantId,
                    amount = amountRial,
                    callback_url = configuration.CallbackUrl,
                    description = "شارژ کیف پول داروخانه"
                },
                cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Wallet payment request provider returned HTTP status {StatusCode}.",
                    (int)response.StatusCode);
                return new PharmacyWalletPaymentStartResult(
                    PharmacyWalletPaymentStartStatus.ProviderUnavailable);
            }

            using var document = await ReadJsonAsync(response, cancellationToken);
            var data = GetObjectProperty(document.RootElement, "data");
            if (data is null ||
                !TryGetInt32(data.Value, "code", out var code) ||
                code != 100 ||
                !TryGetString(data.Value, "authority", out var authority) ||
                string.IsNullOrWhiteSpace(authority))
            {
                logger.LogWarning("Wallet payment request was rejected by the payment provider.");
                return new PharmacyWalletPaymentStartResult(
                    PharmacyWalletPaymentStartStatus.ProviderUnavailable);
            }

            context.WalletTransactions.Add(new WalletTransaction
            {
                AccountId = pharmacyAccountId,
                Amount = amountRial,
                RegisterDate = DateTime.Now,
                Authority = authority,
                TransactionType = TransactionType.Cach,
                Description = "شارژ کیف پول"
            });
            await context.SaveChangesAsync(cancellationToken);

            return new PharmacyWalletPaymentStartResult(
                PharmacyWalletPaymentStartStatus.Started,
                StartPaymentUrl + Uri.EscapeDataString(authority));
        }
        catch (HttpRequestException exception)
        {
            logger.LogWarning(exception, "Wallet payment request could not reach the payment provider.");
            return new PharmacyWalletPaymentStartResult(
                PharmacyWalletPaymentStartStatus.ProviderUnavailable);
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(exception, "Wallet payment request timed out.");
            return new PharmacyWalletPaymentStartResult(
                PharmacyWalletPaymentStartStatus.ProviderUnavailable);
        }
        catch (JsonException exception)
        {
            logger.LogWarning(exception, "Wallet payment provider returned an invalid response.");
            return new PharmacyWalletPaymentStartResult(
                PharmacyWalletPaymentStartStatus.ProviderUnavailable);
        }
    }

    public Task<PharmacyWalletPaymentVerificationStatus> VerifyTopUpAsync(
        int pharmacyAccountId,
        int transactionId,
        CancellationToken cancellationToken = default) =>
        VerifyPendingTransactionAsync(
            pharmacyAccountId,
            transactionId,
            authority: null,
            cancellationToken);

    public Task<PharmacyWalletPaymentVerificationStatus> VerifyCallbackAsync(
        int pharmacyAccountId,
        string authority,
        CancellationToken cancellationToken = default) =>
        VerifyPendingTransactionAsync(
            pharmacyAccountId,
            transactionId: null,
            authority,
            cancellationToken);

    private async Task<PharmacyWalletPaymentVerificationStatus> VerifyPendingTransactionAsync(
        int pharmacyAccountId,
        int? transactionId,
        string? authority,
        CancellationToken cancellationToken)
    {
        var query = context.WalletTransactions.Where(item =>
            item.AccountId == pharmacyAccountId &&
            (transactionId.HasValue
                ? item.Id == transactionId.Value
                : item.Authority == authority));
        var transaction = await query
            .Select(item => new
            {
                item.Id,
                item.Amount,
                item.Authority,
                item.RefId,
                item.PaymentDate
            })
            .SingleOrDefaultAsync(cancellationToken);
        if (transaction is null)
        {
            return PharmacyWalletPaymentVerificationStatus.NotFound;
        }

        if (!string.IsNullOrWhiteSpace(transaction.RefId) && transaction.PaymentDate.HasValue)
        {
            return PharmacyWalletPaymentVerificationStatus.AlreadyPaid;
        }

        if (string.IsNullOrWhiteSpace(transaction.Authority) ||
            transaction.Amount is not > 0)
        {
            return PharmacyWalletPaymentVerificationStatus.NotPending;
        }

        var configuration = options.Value;
        if (!IsPaymentConfigured(configuration))
        {
            return PharmacyWalletPaymentVerificationStatus.NotConfigured;
        }

        try
        {
            using var response = await httpClient.PostAsJsonAsync(
                VerifyPaymentEndpoint,
                new
                {
                    merchant_id = configuration.MerchantId,
                    amount = transaction.Amount.Value,
                    authority = transaction.Authority
                },
                cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Wallet payment verification provider returned HTTP status {StatusCode}.",
                    (int)response.StatusCode);
                return PharmacyWalletPaymentVerificationStatus.ProviderUnavailable;
            }

            using var document = await ReadJsonAsync(response, cancellationToken);
            var data = GetObjectProperty(document.RootElement, "data");
            if (data is null ||
                !TryGetInt32(data.Value, "code", out var code) ||
                code is not (100 or 101) ||
                !TryGetStringOrNumber(data.Value, "ref_id", out var referenceId) ||
                string.IsNullOrWhiteSpace(referenceId))
            {
                return PharmacyWalletPaymentVerificationStatus.Rejected;
            }

            var paymentDate = DateTime.Now;
            var updated = await context.WalletTransactions
                .Where(item =>
                    item.Id == transaction.Id &&
                    item.AccountId == pharmacyAccountId &&
                    item.Authority == transaction.Authority &&
                    item.PaymentDate == null &&
                    (item.RefId == null || item.RefId == string.Empty))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(item => item.RefId, referenceId)
                    .SetProperty(item => item.PaymentDate, paymentDate),
                    cancellationToken);
            if (updated > 0)
            {
                return PharmacyWalletPaymentVerificationStatus.Succeeded;
            }

            var alreadyUpdated = await context.WalletTransactions
                .AsNoTracking()
                .AnyAsync(item =>
                    item.Id == transaction.Id &&
                    item.AccountId == pharmacyAccountId &&
                    item.Authority == transaction.Authority &&
                    item.PaymentDate != null &&
                    item.RefId != null &&
                    item.RefId != string.Empty,
                    cancellationToken);
            return alreadyUpdated
                ? PharmacyWalletPaymentVerificationStatus.AlreadyPaid
                : PharmacyWalletPaymentVerificationStatus.Rejected;
        }
        catch (HttpRequestException exception)
        {
            logger.LogWarning(exception, "Wallet payment verification could not reach the payment provider.");
            return PharmacyWalletPaymentVerificationStatus.ProviderUnavailable;
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(exception, "Wallet payment verification timed out.");
            return PharmacyWalletPaymentVerificationStatus.ProviderUnavailable;
        }
        catch (JsonException exception)
        {
            logger.LogWarning(exception, "Wallet payment verification returned an invalid response.");
            return PharmacyWalletPaymentVerificationStatus.ProviderUnavailable;
        }
    }

    private async Task<long> GetPaidAmountInRialAsync(
        int pharmacyAccountId,
        CancellationToken cancellationToken) =>
        await context.WalletTransactions
            .Where(transaction =>
                transaction.AccountId == pharmacyAccountId &&
                transaction.RefId != null &&
                transaction.RefId != string.Empty &&
                transaction.PaymentDate != null)
            .SumAsync(transaction => (long?)transaction.Amount, cancellationToken) ?? 0;

    private async Task<long> GetDeductionsInRialAsync(
        int pharmacyAccountId,
        CancellationToken cancellationToken)
    {
        var rankingCosts = await context.RankingTransactions
            .Where(rank => rank.PharmacyAccountId == pharmacyAccountId)
            .SumAsync(rank => (long?)rank.Amount, cancellationToken) ?? 0;
        var successfulSmsCount = await context.DarooyabPrescriptionSMSMessages
            .CountAsync(message =>
                message.PharmacyAccountId == pharmacyAccountId &&
                message.IsSuccess == true,
                cancellationToken);
        return rankingCosts + successfulSmsCount * 50_000L;
    }

    private static bool IsPaymentConfigured(ZarinPalOptions configuration) =>
        !string.IsNullOrWhiteSpace(configuration.MerchantId) &&
        Uri.TryCreate(configuration.CallbackUrl, UriKind.Absolute, out var callbackUri) &&
        callbackUri.Scheme == Uri.UriSchemeHttps;

    private static string FormatLegacyPersianDate(DateTime value)
    {
        var calendar = new PersianCalendar();
        return $"{calendar.GetYear(value)}/{calendar.GetMonth(value)}/{calendar.GetDayOfMonth(value)}";
    }

    private static string FormatLegacyPersianExpiry(DateTime value)
    {
        var calendar = new PersianCalendar();
        return $"{calendar.GetYear(value)}/{calendar.GetMonth(value):00}/{calendar.GetDayOfMonth(value):00}" +
               $"[{calendar.GetHour(value):00}:{calendar.GetMinute(value):00}:{calendar.GetSecond(value):00}]";
    }

    private static async Task<JsonDocument> ReadJsonAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
    }

    private static JsonElement? GetObjectProperty(JsonElement element, string propertyName) =>
        element.ValueKind == JsonValueKind.Object &&
        element.TryGetProperty(propertyName, out var property) &&
        property.ValueKind == JsonValueKind.Object
            ? property
            : null;

    private static bool TryGetInt32(JsonElement element, string propertyName, out int value)
    {
        value = 0;
        return element.ValueKind == JsonValueKind.Object &&
               element.TryGetProperty(propertyName, out var property) &&
               property.TryGetInt32(out value);
    }

    private static bool TryGetString(JsonElement element, string propertyName, out string? value)
    {
        value = null;
        if (element.ValueKind != JsonValueKind.Object ||
            !element.TryGetProperty(propertyName, out var property) ||
            property.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        value = property.GetString();
        return !string.IsNullOrWhiteSpace(value);
    }

    private static bool TryGetStringOrNumber(
        JsonElement element,
        string propertyName,
        out string? value)
    {
        value = null;
        if (element.ValueKind != JsonValueKind.Object ||
            !element.TryGetProperty(propertyName, out var property))
        {
            return false;
        }

        value = property.ValueKind switch
        {
            JsonValueKind.String => property.GetString(),
            JsonValueKind.Number => property.GetRawText(),
            _ => null
        };
        return !string.IsNullOrWhiteSpace(value);
    }
}

public sealed class ZarinPalOptions
{
    public const string SectionName = "Wallet:ZarinPal";

    public string? MerchantId { get; set; }
    public string? CallbackUrl { get; set; }
}
