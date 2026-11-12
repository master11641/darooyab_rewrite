using System.Text.Json;
using Microsoft.Extensions.Options;

namespace darooyab_rewrite.Services.PharmacyPrescriptions;

public sealed class KavenegarPrescriptionSmsSender(
    HttpClient httpClient,
    IOptions<PrescriptionSmsOptions> options,
    ILogger<KavenegarPrescriptionSmsSender> logger) : IPharmacyPrescriptionSmsSender
{
    private const int SuccessStatusCode = 200;

    public bool IsConfigured => options.Value.IsConfigured;

    public async Task<PrescriptionSmsProviderResult> SendAsync(
        string recipient,
        string message,
        CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        if (!settings.IsConfigured)
        {
            return NotConfiguredResult();
        }

        var endpoint = CreateEndpoint(settings, settings.SendEndpoint!);
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["receptor"] = recipient,
                ["sender"] = settings.Sender!,
                ["message"] = message
            })
        };
        return await SendRequestAsync(request, cancellationToken);
    }

    public async Task<PrescriptionSmsProviderResult> SendVerifyLookupAsync(
        string recipient,
        string token,
        CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        if (!settings.IsVerifyLookupConfigured)
        {
            return NotConfiguredResult();
        }

        var endpoint = CreateEndpoint(settings, settings.VerifyLookupEndpoint!);
        var query = string.Join(
            "&",
            $"receptor={Uri.EscapeDataString(recipient)}",
            $"token={Uri.EscapeDataString(token)}",
            $"template={Uri.EscapeDataString(settings.OtpTemplate!)}");
        var builder = new UriBuilder(endpoint) { Query = query };
        using var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
        return await SendRequestAsync(request, cancellationToken);
    }

    private async Task<PrescriptionSmsProviderResult> SendRequestAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            using var response = await httpClient.SendAsync(request, cancellationToken);
            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(
                responseStream,
                cancellationToken: cancellationToken);
            if (document.RootElement.ValueKind != JsonValueKind.Object ||
                !document.RootElement.TryGetProperty("return", out var returnData) ||
                returnData.ValueKind != JsonValueKind.Object ||
                !returnData.TryGetProperty("status", out var statusElement) ||
                !statusElement.TryGetInt32(out var statusCode))
            {
                logger.LogWarning("Kavenegar returned an unexpected response for a prescription SMS.");
                return new PrescriptionSmsProviderResult(
                    -2,
                    "پاسخ سرویس پیامک قابل تشخیص نیست.",
                    false);
            }

            var statusMessage = returnData.TryGetProperty("message", out var messageElement)
                && messageElement.ValueKind == JsonValueKind.String
                ? messageElement.GetString()
                : null;
            var isSuccess = response.IsSuccessStatusCode && statusCode == SuccessStatusCode;
            return new PrescriptionSmsProviderResult(
                statusCode,
                string.IsNullOrWhiteSpace(statusMessage)
                    ? isSuccess ? "پیامک پذیرفته شد." : "ارسال پیامک ناموفق بود."
                    : statusMessage,
                isSuccess);
        }
        catch (HttpRequestException exception)
        {
            logger.LogWarning(
                "The Kavenegar request failed ({ExceptionType}).",
                exception.GetType().Name);
            return new PrescriptionSmsProviderResult(
                -1,
                "ارتباط با سرویس پیامک برقرار نشد.",
                false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning("The Kavenegar request timed out.");
            return new PrescriptionSmsProviderResult(
                -1,
                "پاسخ سرویس پیامک در زمان مقرر دریافت نشد.",
                false);
        }
        catch (JsonException exception)
        {
            logger.LogWarning(
                "Kavenegar returned invalid JSON ({ExceptionType}).",
                exception.GetType().Name);
            return new PrescriptionSmsProviderResult(
                -2,
                "پاسخ سرویس پیامک قابل تشخیص نیست.",
                false);
        }
    }

    private static Uri CreateEndpoint(PrescriptionSmsOptions settings, string endpoint)
    {
        if (!Uri.TryCreate(endpoint, UriKind.Relative, out _) ||
            endpoint.Split('/').Any(segment => segment == ".."))
        {
            throw new InvalidOperationException("The configured Kavenegar endpoint is invalid.");
        }

        var baseUrl = settings.ApiBaseUrl!.TrimEnd('/') + "/";
        var baseUri = new Uri(baseUrl, UriKind.Absolute);
        var apiKeySegment = Uri.EscapeDataString(settings.ApiKey!);
        return new Uri(baseUri, $"{apiKeySegment}/{endpoint.TrimStart('/')}");
    }

    private static PrescriptionSmsProviderResult NotConfiguredResult() =>
        new(-3, "ارسال پیامک در این محیط پیکربندی نشده است.", false);
}
