using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace darooyab_rewrite.Services.Authentication.Pharmacy;

public static class LegacyPharmacyCredentialCompatibility
{
    public static string? NormalizeMobile(string? userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            return null;
        }

        var normalized = NormalizePersianDigits(userName);
        if (!normalized.StartsWith('0'))
        {
            normalized = "0" + normalized;
        }

        return normalized.Length is >= 2 and <= 11 &&
               normalized.All(character => character is >= '0' and <= '9')
            ? normalized
            : null;
    }

    public static string NormalizePassword(string password)
    {
        return NormalizePersianDigits(password);
    }

    public static bool VerifyPassword(string? storedPassword, string submittedPassword)
    {
        if (string.IsNullOrEmpty(storedPassword))
        {
            return false;
        }

        var normalizedPassword = NormalizePassword(submittedPassword);
        var submittedHash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(normalizedPassword)));

        return FixedTimeEquals(storedPassword, submittedHash) |
               FixedTimeEquals(storedPassword, normalizedPassword);
    }

    private static string NormalizePersianDigits(string value)
    {
        var sourceDigits = CultureInfo.GetCultureInfo("fa").NumberFormat.NativeDigits;
        var normalized = value;
        for (var digit = 0; digit < sourceDigits.Length; digit++)
        {
            normalized = normalized.Replace(sourceDigits[digit], digit.ToString(CultureInfo.InvariantCulture));
        }

        return normalized;
    }

    private static bool FixedTimeEquals(string left, string right)
    {
        var leftBytes = Encoding.UTF8.GetBytes(left);
        var rightBytes = Encoding.UTF8.GetBytes(right);
        try
        {
            return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(leftBytes);
            CryptographicOperations.ZeroMemory(rightBytes);
        }
    }
}
