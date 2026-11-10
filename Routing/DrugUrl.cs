using System.Text.RegularExpressions;

namespace darooyab_rewrite.Routing;

public static class DrugUrl
{
    public static string CreateSlug(string? drugName)
    {
        var normalizedName = Regex.Replace(drugName ?? string.Empty, @"\s+", " ").Trim();

        return normalizedName
            .Replace(" ", "-")
            .Replace("&", string.Empty)
            .Replace("/", "-")
            .Replace(".", "-")
            .Replace("+", "-")
            .Replace("%", string.Empty)
            .Replace(":", string.Empty);
    }
}
