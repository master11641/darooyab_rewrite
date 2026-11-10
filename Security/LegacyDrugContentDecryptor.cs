using System.Security.Cryptography;
using System.Text;

namespace darooyab_rewrite.Security;

public static class LegacyDrugContentDecryptor
{
    private static readonly byte[] Key = [240, 32, 45, 29, 0, 176, 170, 57];

    public static string DecryptData(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        using var algorithm = DES.Create();
        algorithm.Mode = CipherMode.CBC;
        algorithm.Padding = PaddingMode.PKCS7;
        using var decryptor = algorithm.CreateDecryptor(Key, Key);
        var encryptedBytes = Convert.FromBase64String(value);
        using var output = new MemoryStream();
        try
        {
            using (var cryptoStream = new CryptoStream(output, decryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                cryptoStream.FlushFinalBlock();
            }
        }
        catch (CryptographicException)
        {
            return "Error";
        }

        return Encoding.UTF8.GetString(output.ToArray()).Replace("\n", "<br>");
    }
}
