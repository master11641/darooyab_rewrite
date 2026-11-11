using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace darooyab_rewrite.Services.Captcha;

internal sealed class MemoryCaptchaChallengeStore : BackgroundService, ICaptchaChallengeStore
{
    private const int LockCount = 256;
    private readonly object[] _locks = Enumerable.Range(0, LockCount).Select(_ => new object()).ToArray();
    private readonly ConcurrentDictionary<string, ChallengeEntry> _challenges = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, long>> _activeByClient = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, RateBucket> _rateBuckets = new(StringComparer.Ordinal);

    public Task<bool> TryCreateAsync(
        StoredCaptchaChallenge challenge,
        string clientKey,
        string ipKey,
        CaptchaStorePolicy policy,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(WithLocks(
            $"client:{clientKey}",
            $"ip:{ipKey}",
            () => TryCreate(challenge, clientKey, ipKey, policy)));
    }

    public Task<StoredCaptchaChallenge?> GetAsync(
        string challengeId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (GetLock($"challenge:{challengeId}"))
        {
            return Task.FromResult(
                _challenges.TryGetValue(challengeId, out var entry)
                    ? entry.Challenge
                    : null);
        }
    }

    public Task<bool> TryAcquireValidationAsync(
        string clientKey,
        string ipKey,
        CaptchaStorePolicy policy,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(WithLocks(
            $"client:{clientKey}",
            $"ip:{ipKey}",
            () =>
            {
                var clientCount = IncrementRateBucket(
                    $"validate:client:{clientKey}:{policy.RateWindowNumber}",
                    policy);
                var ipCount = IncrementRateBucket(
                    $"validate:ip:{ipKey}:{policy.RateWindowNumber}",
                    policy);

                return clientCount <= policy.MaxValidationRequestsPerClient &&
                       ipCount <= policy.MaxValidationRequestsPerIp;
            }));
    }

    public Task<CaptchaCommitStatus> TryCommitValidationAsync(
        string challengeId,
        string clientBindingHash,
        int expectedFailedAttempts,
        bool answerMatches,
        CaptchaStorePolicy policy,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (GetLock($"challenge:{challengeId}"))
        {
            if (!_challenges.TryGetValue(challengeId, out var entry))
            {
                return Task.FromResult(CaptchaCommitStatus.Invalid);
            }

            var challenge = entry.Challenge;
            if (!FixedTimeHexEquals(challenge.ClientBindingHash, clientBindingHash))
            {
                return Task.FromResult(CaptchaCommitStatus.Invalid);
            }

            if (challenge.ExpiresAtUnixMilliseconds <= policy.NowUnixMilliseconds)
            {
                MarkConsumed(challengeId, entry, consumed: true);
                return Task.FromResult(CaptchaCommitStatus.Expired);
            }

            if (challenge.Consumed)
            {
                return Task.FromResult(CaptchaCommitStatus.AlreadyConsumed);
            }

            if (challenge.FailedAttempts != expectedFailedAttempts)
            {
                return Task.FromResult(CaptchaCommitStatus.Retry);
            }

            if (answerMatches)
            {
                MarkConsumed(challengeId, entry, consumed: true);
                return Task.FromResult(CaptchaCommitStatus.Accepted);
            }

            var failedAttempts = challenge.FailedAttempts + 1;
            entry.Challenge = challenge with { FailedAttempts = failedAttempts };
            if (failedAttempts >= policy.MaxFailedAttempts)
            {
                MarkConsumed(challengeId, entry, consumed: true);
                return Task.FromResult(CaptchaCommitStatus.AttemptsExceeded);
            }

            return Task.FromResult(CaptchaCommitStatus.Rejected);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            SweepExpired();
        }
    }

    private bool TryCreate(
        StoredCaptchaChallenge challenge,
        string clientKey,
        string ipKey,
        CaptchaStorePolicy policy)
    {
        var clientCount = IncrementRateBucket(
            $"generate:client:{clientKey}:{policy.RateWindowNumber}",
            policy);
        var ipCount = IncrementRateBucket(
            $"generate:ip:{ipKey}:{policy.RateWindowNumber}",
            policy);
        if (clientCount > policy.MaxGenerateRequestsPerClient ||
            ipCount > policy.MaxGenerateRequestsPerIp)
        {
            return false;
        }

        var active = _activeByClient.GetOrAdd(
            clientKey,
            static _ => new ConcurrentDictionary<string, long>(StringComparer.Ordinal));
        foreach (var item in active)
        {
            if (item.Value <= policy.NowUnixMilliseconds)
            {
                active.TryRemove(item.Key, out _);
            }
        }

        if (active.Count >= policy.MaxActiveChallengesPerClient)
        {
            return false;
        }

        if (!_challenges.TryAdd(challenge.ChallengeId, new ChallengeEntry(challenge)))
        {
            throw new InvalidOperationException("A duplicate CAPTCHA challenge ID was generated.");
        }

        active[challenge.ChallengeId] = challenge.ExpiresAtUnixMilliseconds;
        return true;
    }

    private int IncrementRateBucket(string key, CaptchaStorePolicy policy)
    {
        var bucket = _rateBuckets.GetOrAdd(
            key,
            _ => new RateBucket(policy.NowUnixMilliseconds + policy.RateWindowTtlMilliseconds));
        if (bucket.ExpiresAtUnixMilliseconds <= policy.NowUnixMilliseconds)
        {
            bucket = new RateBucket(policy.NowUnixMilliseconds + policy.RateWindowTtlMilliseconds);
            _rateBuckets[key] = bucket;
        }

        return ++bucket.Count;
    }

    private void MarkConsumed(string challengeId, ChallengeEntry entry, bool consumed)
    {
        entry.Challenge = entry.Challenge with { Consumed = consumed };
        if (_activeByClient.TryGetValue(entry.Challenge.ClientBindingHash, out var active))
        {
            active.TryRemove(challengeId, out _);
        }
    }

    private void SweepExpired()
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        foreach (var item in _challenges)
        {
            if (item.Value.Challenge.ExpiresAtUnixMilliseconds > now)
            {
                continue;
            }

            lock (GetLock($"challenge:{item.Key}"))
            {
                if (_challenges.TryGetValue(item.Key, out var entry) &&
                    entry.Challenge.ExpiresAtUnixMilliseconds <= now)
                {
                    _challenges.TryRemove(item.Key, out _);
                    if (_activeByClient.TryGetValue(
                            entry.Challenge.ClientBindingHash,
                            out var active))
                    {
                        active.TryRemove(item.Key, out _);
                    }
                }
            }
        }

        foreach (var bucket in _rateBuckets)
        {
            if (bucket.Value.ExpiresAtUnixMilliseconds <= now)
            {
                _rateBuckets.TryRemove(bucket.Key, out _);
            }
        }

        foreach (var client in _activeByClient)
        {
            lock (GetLock($"client:{client.Key}"))
            {
                foreach (var item in client.Value)
                {
                    if (item.Value <= now)
                    {
                        client.Value.TryRemove(item.Key, out _);
                    }
                }

                if (client.Value.IsEmpty)
                {
                    _activeByClient.TryRemove(
                        new KeyValuePair<string, ConcurrentDictionary<string, long>>(
                            client.Key,
                            client.Value));
                }
            }
        }
    }

    private object GetLock(string key)
    {
        return _locks[(key.GetHashCode(StringComparison.Ordinal) & int.MaxValue) % LockCount];
    }

    private TResult WithLocks<TResult>(string firstKey, string secondKey, Func<TResult> action)
    {
        var firstLock = GetLock(firstKey);
        var secondLock = GetLock(secondKey);
        if (ReferenceEquals(firstLock, secondLock))
        {
            lock (firstLock)
            {
                return action();
            }
        }

        var firstIndex = Array.IndexOf(_locks, firstLock);
        var secondIndex = Array.IndexOf(_locks, secondLock);
        var lowerLock = firstIndex < secondIndex ? firstLock : secondLock;
        var upperLock = firstIndex < secondIndex ? secondLock : firstLock;
        lock (lowerLock)
        {
            lock (upperLock)
            {
                return action();
            }
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

    private sealed class ChallengeEntry(StoredCaptchaChallenge challenge)
    {
        public StoredCaptchaChallenge Challenge { get; set; } = challenge;
    }

    private sealed class RateBucket(long expiresAtUnixMilliseconds)
    {
        public long ExpiresAtUnixMilliseconds { get; } = expiresAtUnixMilliseconds;
        public int Count { get; set; }
    }
}
