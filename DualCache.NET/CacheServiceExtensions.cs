using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;

namespace DualCache.NET
{
    public static class CacheServiceExtensions
    {
        /// <summary>
        /// Adds Redis caching services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance to retrieve configuration settings.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> with Redis caching services added.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the Redis connection string is null or empty.</exception>
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

        /// <summary>
        /// Adds custom memory caching services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> with custom memory caching services added.</returns>
        public static IServiceCollection AddCustomMemoryCache(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();

            return services;
        }

        /// <summary>
        /// Configures the caching service based on the specified <see cref="CacheType"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="cacheType">The type of caching to use (Redis or Memory).</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance to retrieve configuration settings.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> with the appropriate caching services added.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported cache type is provided.</exception>
        public static IServiceCollection AddCacheService(this IServiceCollection services, CacheType cacheType, IConfiguration configuration)
        {
            switch (cacheType)
            {
                case CacheType.Redis:
                    services.AddRedisCache(configuration);
                    break;

                case CacheType.Memory:
                    services.AddCustomMemoryCache();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(cacheType), $"Unsupported cache type: {cacheType}");
            }

            return services;
        }
    }
}