using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace darooyab_rewrite.Services.Captcha;

public static class CaptchaServiceCollectionExtensions
{
    public static IServiceCollection AddCaptcha(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var storageMode = configuration["Captcha:StorageMode"] ?? "Memory";
        var useRedis = string.Equals(storageMode, "Redis", StringComparison.OrdinalIgnoreCase);
        if (!useRedis && !string.Equals(storageMode, "Memory", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Captcha:StorageMode must be either Memory or Redis.");
        }

        if (useRedis)
        {
            if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("CaptchaRedis")))
            {
                throw new InvalidOperationException(
                    "Configure ConnectionStrings:CaptchaRedis when Captcha:StorageMode is Redis.");
            }

            ValidateSharedHashKey(configuration["Captcha:AnswerHashKey"]);
        }
        else if (!string.IsNullOrWhiteSpace(configuration["Captcha:AnswerHashKey"]))
        {
            ValidateSharedHashKey(configuration["Captcha:AnswerHashKey"]);
        }

        services
            .AddOptions<CaptchaOptions>()
            .Bind(configuration.GetSection("Captcha"))
            .Validate(HasValidBounds, "CAPTCHA options contain unsupported values.")
            .ValidateOnStart();

        services.AddSingleton<PngCaptchaRenderer>();
        services.TryAddSingleton<ICaptchaAnswerGenerator, CryptographicCaptchaAnswerGenerator>();
        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<CaptchaAnswerHasher>(_ =>
        {
            var configuredKey = configuration["Captcha:AnswerHashKey"];
            return new CaptchaAnswerHasher(configuredKey, useRedis);
        });

        if (!useRedis)
        {
            services.AddSingleton<MemoryCaptchaChallengeStore>();
            services.AddSingleton<ICaptchaChallengeStore>(
                provider => provider.GetRequiredService<MemoryCaptchaChallengeStore>());
            services.AddHostedService(
                provider => provider.GetRequiredService<MemoryCaptchaChallengeStore>());
        }
        else if (string.Equals(storageMode, "Redis", StringComparison.OrdinalIgnoreCase))
        {
            var redisConnectionString = configuration.GetConnectionString("CaptchaRedis");
            services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                var options = ConfigurationOptions.Parse(redisConnectionString);
                options.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(options);
            });
            services.AddSingleton<ICaptchaChallengeStore>(provider =>
                new RedisCaptchaChallengeStore(
                    provider.GetRequiredService<IConnectionMultiplexer>().GetDatabase()));
        }
        services.AddSingleton<ICaptchaService, CaptchaService>();
        return services;
    }

    private static bool HasValidBounds(CaptchaOptions options)
    {
        return options.TextLength is >= 4 and <= 8 &&
               options.ImageWidth is >= 160 and <= 300 &&
               options.ImageHeight is >= 48 and <= 100 &&
               options.ChallengeLifetime >= TimeSpan.FromSeconds(30) &&
               options.ChallengeLifetime <= TimeSpan.FromMinutes(15) &&
               options.MaxFailedAttempts is >= 1 and <= 10 &&
               options.MaxActiveChallengesPerClient is >= 1 and <= 50 &&
               options.RateLimitWindow >= TimeSpan.FromMinutes(1) &&
               options.RateLimitWindow <= TimeSpan.FromHours(1) &&
               options.MaxGenerateRequestsPerClient is >= 1 and <= 1000 &&
               options.MaxGenerateRequestsPerIp is >= 1 and <= 10000 &&
               options.MaxValidationRequestsPerClient is >= 1 and <= 1000 &&
               options.MaxValidationRequestsPerIp is >= 1 and <= 10000 &&
               options.MaxApiRequestsPerIp is >= 1 and <= 100000 &&
               options.ApiRateLimitWindow >= TimeSpan.FromMinutes(1) &&
               options.ApiRateLimitWindow <= TimeSpan.FromHours(1) &&
               options.MaxPublicFeedbackRequestsPerIp is >= 1 and <= 10000 &&
               options.PublicFeedbackRateLimitWindow >= TimeSpan.FromMinutes(1) &&
               options.PublicFeedbackRateLimitWindow <= TimeSpan.FromHours(1) &&
               options.NoisePixelCount is >= 0 and <= 1000 &&
               options.InterferenceLineCount is >= 0 and <= 12 &&
               (string.Equals(options.StorageMode, "Memory", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(options.StorageMode, "Redis", StringComparison.OrdinalIgnoreCase));
    }

    private static void ValidateSharedHashKey(string? configuredKey)
    {
        if (string.IsNullOrWhiteSpace(configuredKey))
        {
            throw new InvalidOperationException(
                "Configure a shared 32-byte Base64 key in Captcha:AnswerHashKey when Redis storage is enabled.");
        }

        Span<byte> key = stackalloc byte[32];
        if (Convert.TryFromBase64String(configuredKey, key, out var bytesWritten) &&
            bytesWritten == key.Length)
        {
            System.Security.Cryptography.CryptographicOperations.ZeroMemory(key);
            return;
        }

        System.Security.Cryptography.CryptographicOperations.ZeroMemory(key);
        throw new InvalidOperationException(
            "Captcha:AnswerHashKey must be a Base64-encoded 32-byte key.");
    }
}
