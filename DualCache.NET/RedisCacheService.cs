using StackExchange.Redis;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DualCache.NET
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _redisDb;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private bool _disposed;

        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.Preserve
        };

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
            _redisDb = _connectionMultiplexer.GetDatabase();
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            if (expiration.HasValue && expiration.Value <= TimeSpan.Zero)
            {
                expiration = null;
            }

            var serializedValue = JsonSerializer.Serialize(value, SerializerOptions);
            await _redisDb.StringSetAsync(key, serializedValue, expiration);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            RedisValue value = await _redisDb.StringGetAsync(key);

            return Deserialize<T>(value);
        }

        public async Task RemoveAsync(string key)
        {
            await _redisDb.KeyDeleteAsync(key);
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            RedisValue value = await _redisDb.StringGetAsync(key);
            if (value.HasValue)
            {
                return Deserialize<T>(value);
            }

            var newValue = await factory();
            await SetAsync(key, newValue, expiration);

            return newValue;
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return await _redisDb.KeyExistsAsync(key);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static T Deserialize<T>(RedisValue? value)
        {
            if(value is null) { return default; }

            try
            {
                return JsonSerializer.Deserialize<T>(value, SerializerOptions);
            }
            catch (Exception)
            {
                return default;
            }
            
        }

        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [ExcludeFromCodeCoverage]
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _connectionMultiplexer?.Dispose();
                }
                _disposed = true;
            }
        }

        [ExcludeFromCodeCoverage]
        ~RedisCacheService()
        {
            Dispose(false);
        }
    }
}