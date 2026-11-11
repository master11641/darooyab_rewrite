using System.Globalization;
using StackExchange.Redis;

namespace darooyab_rewrite.Services.Captcha;

internal sealed class RedisCaptchaChallengeStore(IDatabase database) : ICaptchaChallengeStore
{
    private const string CreateScript = """
        local clientGenerateCount = redis.call('INCR', KEYS[3])
        if clientGenerateCount == 1 then redis.call('PEXPIRE', KEYS[3], ARGV[4]) end
        local ipGenerateCount = redis.call('INCR', KEYS[4])
        if ipGenerateCount == 1 then redis.call('PEXPIRE', KEYS[4], ARGV[4]) end
        if clientGenerateCount > tonumber(ARGV[6]) or ipGenerateCount > tonumber(ARGV[7]) then
            return 0
        end
        redis.call('ZREMRANGEBYSCORE', KEYS[2], '-inf', ARGV[1])
        if redis.call('ZCARD', KEYS[2]) >= tonumber(ARGV[5]) then return 0 end
        redis.call('HSET', KEYS[1],
            'client', ARGV[8],
            'answerHash', ARGV[9],
            'createdAt', ARGV[10],
            'expiresAt', ARGV[2],
            'failedAttempts', 0,
            'consumed', 0)
        redis.call('PEXPIRE', KEYS[1], ARGV[3])
        redis.call('ZADD', KEYS[2], ARGV[2], ARGV[11])
        redis.call('PEXPIRE', KEYS[2], tonumber(ARGV[3]) + tonumber(ARGV[4]))
        return 1
        """;

    private const string AcquireValidationScript = """
        local clientCount = redis.call('INCR', KEYS[1])
        if clientCount == 1 then redis.call('PEXPIRE', KEYS[1], ARGV[1]) end
        local ipCount = redis.call('INCR', KEYS[2])
        if ipCount == 1 then redis.call('PEXPIRE', KEYS[2], ARGV[1]) end
        if clientCount > tonumber(ARGV[2]) or ipCount > tonumber(ARGV[3]) then return 0 end
        return 1
        """;

    private const string CommitScript = """
        if redis.call('EXISTS', KEYS[1]) == 0 then return -1 end
        if redis.call('HGET', KEYS[1], 'client') ~= ARGV[1] then return -1 end
        local consumed = tonumber(redis.call('HGET', KEYS[1], 'consumed') or '0')
        if consumed ~= 0 then return 4 end
        local expiresAt = tonumber(redis.call('HGET', KEYS[1], 'expiresAt') or '0')
        if expiresAt <= tonumber(ARGV[4]) then
            redis.call('HSET', KEYS[1], 'consumed', 2)
            redis.call('ZREM', KEYS[2], ARGV[5])
            return 3
        end
        local failedAttempts = tonumber(redis.call('HGET', KEYS[1], 'failedAttempts') or '0')
        if failedAttempts ~= tonumber(ARGV[2]) then return 0 end
        if ARGV[3] == '1' then
            redis.call('HSET', KEYS[1], 'consumed', 1)
            redis.call('ZREM', KEYS[2], ARGV[5])
            return 1
        end
        failedAttempts = failedAttempts + 1
        redis.call('HSET', KEYS[1], 'failedAttempts', failedAttempts)
        if failedAttempts >= tonumber(ARGV[6]) then
            redis.call('HSET', KEYS[1], 'consumed', 2)
            redis.call('ZREM', KEYS[2], ARGV[5])
            return 5
        end
        return 2
        """;

    public async Task<bool> TryCreateAsync(
        StoredCaptchaChallenge challenge,
        string clientKey,
        string ipKey,
        CaptchaStorePolicy policy,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var result = await database.ScriptEvaluateAsync(
                CreateScript,
                [
                    ChallengeKey(challenge.ChallengeId),
                    ActiveKey(clientKey),
                    RateKey("generate:client", clientKey, policy.RateWindowNumber),
                    RateKey("generate:ip", ipKey, policy.RateWindowNumber)
                ],
                [
                    policy.NowUnixMilliseconds,
                    challenge.ExpiresAtUnixMilliseconds,
                    policy.ChallengeTtlMilliseconds,
                    policy.RateWindowTtlMilliseconds,
                    policy.MaxActiveChallengesPerClient,
                    policy.MaxGenerateRequestsPerClient,
                    policy.MaxGenerateRequestsPerIp,
                    clientKey,
                    challenge.AnswerHash,
                    challenge.CreatedAtUnixMilliseconds,
                    challenge.ChallengeId
                ]);
            return (long)result == 1;
        }
        catch (RedisException exception)
        {
            throw new CaptchaStoreUnavailableException(exception);
        }
    }

    public async Task<StoredCaptchaChallenge?> GetAsync(
        string challengeId,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var values = await database.HashGetAllAsync(ChallengeKey(challengeId));
            if (values.Length == 0)
            {
                return null;
            }

            var fields = values.ToDictionary(
                item => item.Name.ToString(),
                item => item.Value.ToString(),
                StringComparer.Ordinal);
            return new StoredCaptchaChallenge(
                challengeId,
                fields["client"],
                fields["answerHash"],
                long.Parse(fields["createdAt"], CultureInfo.InvariantCulture),
                long.Parse(fields["expiresAt"], CultureInfo.InvariantCulture),
                int.Parse(fields["failedAttempts"], CultureInfo.InvariantCulture),
                int.Parse(fields["consumed"], CultureInfo.InvariantCulture) != 0);
        }
        catch (RedisException exception)
        {
            throw new CaptchaStoreUnavailableException(exception);
        }
        catch (Exception exception) when (exception is FormatException or KeyNotFoundException)
        {
            throw new CaptchaStoreUnavailableException(exception);
        }
    }

    public async Task<bool> TryAcquireValidationAsync(
        string clientKey,
        string ipKey,
        CaptchaStorePolicy policy,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var result = await database.ScriptEvaluateAsync(
                AcquireValidationScript,
                [
                    RateKey("validate:client", clientKey, policy.RateWindowNumber),
                    RateKey("validate:ip", ipKey, policy.RateWindowNumber)
                ],
                [
                    policy.RateWindowTtlMilliseconds,
                    policy.MaxValidationRequestsPerClient,
                    policy.MaxValidationRequestsPerIp
                ]);
            return (long)result == 1;
        }
        catch (RedisException exception)
        {
            throw new CaptchaStoreUnavailableException(exception);
        }
    }

    public async Task<CaptchaCommitStatus> TryCommitValidationAsync(
        string challengeId,
        string clientBindingHash,
        int expectedFailedAttempts,
        bool answerMatches,
        CaptchaStorePolicy policy,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var result = await database.ScriptEvaluateAsync(
                CommitScript,
                [
                    ChallengeKey(challengeId),
                    ActiveKey(clientBindingHash)
                ],
                [
                    clientBindingHash,
                    expectedFailedAttempts,
                    answerMatches ? 1 : 0,
                    policy.NowUnixMilliseconds,
                    challengeId,
                    policy.MaxFailedAttempts
                ]);

            return (long)result switch
            {
                1 => CaptchaCommitStatus.Accepted,
                2 => CaptchaCommitStatus.Rejected,
                3 => CaptchaCommitStatus.Expired,
                4 => CaptchaCommitStatus.AlreadyConsumed,
                5 => CaptchaCommitStatus.AttemptsExceeded,
                0 => CaptchaCommitStatus.Retry,
                _ => CaptchaCommitStatus.Invalid
            };
        }
        catch (RedisException exception)
        {
            throw new CaptchaStoreUnavailableException(exception);
        }
    }

    private static RedisKey ChallengeKey(string challengeId) => $"captcha:challenge:{challengeId}";
    private static RedisKey ActiveKey(string clientKey) => $"captcha:active:{clientKey}";
    private static RedisKey RateKey(string scope, string key, long windowNumber) =>
        $"captcha:rate:{scope}:{key}:{windowNumber}";
}
