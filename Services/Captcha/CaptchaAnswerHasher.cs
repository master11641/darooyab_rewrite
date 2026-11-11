using System.Security.Cryptography;
using System.Text;

namespace darooyab_rewrite.Services.Captcha;

internal sealed class CaptchaAnswerHasher : IDisposable
{
    private readonly byte[] _key;

    public CaptchaAnswerHasher(string? configuredKey, bool requireSharedKey)
    {
        if (string.IsNullOrWhiteSpace(configuredKey))
        {
            if (requireSharedKey)
            {
                throw new InvalidOperationException(
                    "Configure a shared 32-byte Base64 key in Captcha:AnswerHashKey when Redis storage is enabled.");
            }

            _key = RandomNumberGenerator.GetBytes(32);
            return;
        }

        try
        {
            _key = Convert.FromBase64String(configuredKey);
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException(
                "Captcha:AnswerHashKey must be a Base64-encoded 32-byte key.",
                exception);
        }

        if (_key.Length != 32)
        {
            CryptographicOperations.ZeroMemory(_key);
            throw new InvalidOperationException(
                "Captcha:AnswerHashKey must be a Base64-encoded 32-byte key.");
        }
    }

    public byte[] Hash(string challengeId, string answer)
    {
        var normalizedAnswer = answer.Trim().ToUpperInvariant();
        var payload = Encoding.ASCII.GetBytes(string.Concat(challengeId, "\n", normalizedAnswer));
        return HMACSHA256.HashData(_key, payload);
    }

    public static string HashBinding(string value)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    }

    public void Dispose()
    {
        CryptographicOperations.ZeroMemory(_key);
    }
}
