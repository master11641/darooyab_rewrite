using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.WebUtilities;

namespace darooyab_rewrite.Services.Captcha;

internal sealed class CaptchaService : ICaptchaService
{
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private readonly CaptchaOptions _options;
    private readonly ICaptchaChallengeStore _store;
    private readonly CaptchaAnswerHasher _hasher;
    private readonly ICaptchaAnswerGenerator _answerGenerator;
    private readonly PngCaptchaRenderer _renderer;
    private readonly TimeProvider _timeProvider;

    public CaptchaService(
        IOptions<CaptchaOptions> options,
        ICaptchaChallengeStore store,
        CaptchaAnswerHasher hasher,
        ICaptchaAnswerGenerator answerGenerator,
        PngCaptchaRenderer renderer,
        TimeProvider timeProvider)
    {
        _options = options.Value;
        _store = store;
        _hasher = hasher;
        _answerGenerator = answerGenerator;
        _renderer = renderer;
        _timeProvider = timeProvider;
    }

    public bool IsEnabled => _options.Enabled;

    public async Task<CaptchaChallenge> GenerateAsync(
        string clientBinding,
        string? remoteIpAddress,
        CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            throw new InvalidOperationException("CAPTCHA is disabled.");
        }

        cancellationToken.ThrowIfCancellationRequested();
        var now = _timeProvider.GetUtcNow();
        var expiresAt = now.Add(_options.ChallengeLifetime);
        var challengeId = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
        var answer = _answerGenerator.Generate(_options.TextLength);
        var clientKey = CaptchaAnswerHasher.HashBinding(clientBinding);
        var ipKey = CaptchaAnswerHasher.HashBinding(remoteIpAddress ?? "unknown");
        var policy = CreatePolicy(now);
        var challenge = new StoredCaptchaChallenge(
            challengeId,
            clientKey,
            Convert.ToHexString(_hasher.Hash(challengeId, answer)),
            now.ToUnixTimeMilliseconds(),
            expiresAt.ToUnixTimeMilliseconds(),
            0,
            false);

        if (!await _store.TryCreateAsync(challenge, clientKey, ipKey, policy, cancellationToken))
        {
            throw new CaptchaRateLimitException();
        }

        return new CaptchaChallenge(
            challengeId,
            _renderer.Render(answer, _options),
            expiresAt);
    }

    public async Task<CaptchaValidationResult> ValidateAsync(
        string? challengeId,
        string? answer,
        string clientBinding,
        string? remoteIpAddress,
        CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return new CaptchaValidationResult(CaptchaValidationStatus.Valid);
        }

        if (!IsValidChallengeId(challengeId) ||
            string.IsNullOrWhiteSpace(answer) ||
            answer.Length > _options.TextLength)
        {
            return new CaptchaValidationResult(CaptchaValidationStatus.Invalid);
        }

        var now = _timeProvider.GetUtcNow();
        var clientKey = CaptchaAnswerHasher.HashBinding(clientBinding);
        var ipKey = CaptchaAnswerHasher.HashBinding(remoteIpAddress ?? "unknown");
        var policy = CreatePolicy(now);
        if (!await _store.TryAcquireValidationAsync(
                clientKey,
                ipKey,
                policy,
                cancellationToken))
        {
            return new CaptchaValidationResult(CaptchaValidationStatus.RateLimited);
        }

        for (var attempt = 0; attempt < 4; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var storedChallenge = await _store.GetAsync(challengeId!, cancellationToken);
            if (storedChallenge is null)
            {
                return new CaptchaValidationResult(CaptchaValidationStatus.Invalid);
            }

            if (!FixedTimeHexEquals(storedChallenge.ClientBindingHash, clientKey))
            {
                return new CaptchaValidationResult(CaptchaValidationStatus.Invalid);
            }

            if (storedChallenge.ExpiresAtUnixMilliseconds <= now.ToUnixTimeMilliseconds())
            {
                return new CaptchaValidationResult(CaptchaValidationStatus.Expired);
            }

            if (storedChallenge.Consumed)
            {
                return new CaptchaValidationResult(CaptchaValidationStatus.AlreadyConsumed);
            }

            var suppliedHash = _hasher.Hash(challengeId!, answer);
            var answerMatches = FixedTimeHexEquals(
                storedChallenge.AnswerHash,
                Convert.ToHexString(suppliedHash));
            var commitPolicy = CreatePolicy(_timeProvider.GetUtcNow());
            var commit = await _store.TryCommitValidationAsync(
                challengeId!,
                clientKey,
                storedChallenge.FailedAttempts,
                answerMatches,
                commitPolicy,
                cancellationToken);

            switch (commit)
            {
                case CaptchaCommitStatus.Accepted:
                    return new CaptchaValidationResult(CaptchaValidationStatus.Valid);
                case CaptchaCommitStatus.Rejected:
                    return new CaptchaValidationResult(CaptchaValidationStatus.Invalid);
                case CaptchaCommitStatus.AttemptsExceeded:
                    return new CaptchaValidationResult(CaptchaValidationStatus.AttemptsExceeded);
                case CaptchaCommitStatus.Expired:
                    return new CaptchaValidationResult(CaptchaValidationStatus.Expired);
                case CaptchaCommitStatus.AlreadyConsumed:
                    return new CaptchaValidationResult(CaptchaValidationStatus.AlreadyConsumed);
                case CaptchaCommitStatus.Invalid:
                    return new CaptchaValidationResult(CaptchaValidationStatus.Invalid);
                case CaptchaCommitStatus.Retry:
                    continue;
                default:
                    throw new InvalidOperationException("Unknown CAPTCHA commit result.");
            }
        }

        return new CaptchaValidationResult(CaptchaValidationStatus.Invalid);
    }

    private CaptchaStorePolicy CreatePolicy(DateTimeOffset now)
    {
        var windowTicks = _options.RateLimitWindow.Ticks;
        var windowNumber = now.UtcTicks / windowTicks;
        var windowEndTicks = (windowNumber + 1) * windowTicks;
        var windowTtlMilliseconds = Math.Max(
            1,
            TimeSpan.FromTicks(windowEndTicks - now.UtcTicks).TotalMilliseconds);

        return new CaptchaStorePolicy(
            _options.MaxActiveChallengesPerClient,
            _options.MaxGenerateRequestsPerClient,
            _options.MaxGenerateRequestsPerIp,
            _options.MaxValidationRequestsPerClient,
            _options.MaxValidationRequestsPerIp,
            windowNumber,
            (long)Math.Ceiling(windowTtlMilliseconds),
            now.ToUnixTimeMilliseconds(),
            (long)Math.Ceiling(_options.ChallengeLifetime.TotalMilliseconds),
            _options.MaxFailedAttempts);
    }

    private bool IsValidChallengeId(string? challengeId)
    {
        if (string.IsNullOrEmpty(challengeId) || challengeId.Length != 43)
        {
            return false;
        }

        try
        {
            return WebEncoders.Base64UrlDecode(challengeId).Length == 32;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static bool FixedTimeHexEquals(string expectedHex, string actualHex)
    {
        try
        {
            var expected = Convert.FromHexString(expectedHex);
            var actual = Convert.FromHexString(actualHex);
            return expected.Length == actual.Length &&
                   CryptographicOperations.FixedTimeEquals(expected, actual);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

public sealed class CaptchaRateLimitException : Exception
{
}

internal interface ICaptchaAnswerGenerator
{
    string Generate(int length);
}

internal sealed class CryptographicCaptchaAnswerGenerator : ICaptchaAnswerGenerator
{
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public string Generate(int length)
    {
        return string.Create(
            length,
            Alphabet,
            static (characters, alphabet) =>
            {
                for (var index = 0; index < characters.Length; index++)
                {
                    characters[index] = alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];
                }
            });
    }
}
