using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DualCache.NET
{
    public static class CacheServiceExtensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetConnectionString("RedisConnection");

            if (string.IsNullOrEmpty(redisConnectionString))
            {
                throw new ArgumentNullException(nameof(redisConnectionString), "Redis connection string cannot be null or empty.");
            }

            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisConnectionString));
            services.AddSingleton<ICacheService, RedisCacheService>();

            return services;
        }

        public static IServiceCollection AddMemoryCache(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();

            return services;
        }

        public static IServiceCollection AddCacheService(this IServiceCollection services, CacheType cacheType, IConfiguration configuration)
        {
            switch (cacheType)
            {
                case CacheType.Redis:
                    services.AddRedisCache(configuration);
                    break;

                case CacheType.Memory:
                    services.AddMemoryCache();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(cacheType), $"Unsupported cache type: {cacheType}");
            }

            return services;
        }
    }
}
