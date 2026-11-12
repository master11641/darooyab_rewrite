namespace darooyab_rewrite.Services.PharmacyPrescriptions;

public sealed class PrescriptionSmsOptions
{
    public const string SectionName = "PrescriptionSms";
    public const string ApiKeyPlaceholder = "REPLACE_WITH_KAVENEGAR_API_KEY";

    public string? ApiKey { get; set; }
    public string? ApiBaseUrl { get; set; }
    public string? SendEndpoint { get; set; }
    public string? VerifyLookupEndpoint { get; set; }
    public string? Sender { get; set; }
    public string? OtpTemplate { get; set; }

    public bool IsConfigured =>
        HasApiKey &&
        !string.IsNullOrWhiteSpace(Sender) &&
        HasValidApiBaseUrl &&
        IsSafeRelativeEndpoint(SendEndpoint);

    public bool IsVerifyLookupConfigured =>
        HasApiKey &&
        !string.IsNullOrWhiteSpace(OtpTemplate) &&
        HasValidApiBaseUrl &&
        IsSafeRelativeEndpoint(VerifyLookupEndpoint);

    private bool HasApiKey =>
        !string.IsNullOrWhiteSpace(ApiKey) &&
        !ApiKey.Equals(ApiKeyPlaceholder, StringComparison.OrdinalIgnoreCase);

    private bool HasValidApiBaseUrl =>
        Uri.TryCreate(ApiBaseUrl, UriKind.Absolute, out var baseUri) &&
        baseUri.Scheme == Uri.UriSchemeHttps &&
        baseUri.Host.Equals("api.kavenegar.com", StringComparison.OrdinalIgnoreCase) &&
        string.IsNullOrEmpty(baseUri.Query) &&
        string.IsNullOrEmpty(baseUri.Fragment);

    private static bool IsSafeRelativeEndpoint(string? endpoint) =>
        !string.IsNullOrWhiteSpace(endpoint) &&
        Uri.TryCreate(endpoint, UriKind.Relative, out _) &&
        !endpoint.Split('/').Any(segment => segment == "..");
}
