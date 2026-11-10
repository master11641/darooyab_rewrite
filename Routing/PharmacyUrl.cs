namespace darooyab_rewrite.Routing;

public static class PharmacyUrl
{
    public static string CreateSlug(string? pharmacyName, string? cityName)
    {
        var pharmacyPart = DrugUrl.CreateSlug(pharmacyName);
        var cityPart = DrugUrl.CreateSlug(cityName);
        return $"داروخانه-{pharmacyPart}-{cityPart}";
    }

    public static string NormalizeImageUrl(string? imageUrl)
    {
        var value = imageUrl?.Trim();
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        if (Uri.TryCreate(value, UriKind.Absolute, out var absoluteUri) &&
            (absoluteUri.Scheme == Uri.UriSchemeHttp || absoluteUri.Scheme == Uri.UriSchemeHttps))
        {
            return value;
        }

        if (value.StartsWith("~/", StringComparison.Ordinal))
        {
            value = value[1..];
        }

        return value.StartsWith("/", StringComparison.Ordinal) ? value : string.Empty;
    }
}
