using DNTPersianUtils.Core;

namespace darooyab_rewrite.Utilities;

public static class PersianDateFormatter
{
    public static string Format(DateTime? value, bool yearOnly = false)
    {
        if (!value.HasValue)
        {
            return string.Empty;
        }

        var formattedDate = value.Value.ToShortPersianDateString();
        return yearOnly ? formattedDate[..4] : formattedDate;
    }
}
