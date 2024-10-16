using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DualCache.NET
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _redisDb;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private bool _disposed;

        private static readonly JsonSerializerOptions SerializerOptions = new(new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        });

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _redisDb = _connectionMultiplexer.GetDatabase();
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var serializedValue = JsonSerializer.Serialize(value, SerializerOptions);
            await _redisDb.StringSetAsync(key, serializedValue, expiration);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            RedisValue value = await _redisDb.StringGetAsync(key);

            if (!value.HasValue)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value, SerializerOptions);
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
                return JsonSerializer.Deserialize<T>(value, SerializerOptions);
            }

            var newValue = await factory();
            await SetAsync(key, newValue, expiration);

            return newValue;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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

        ~RedisCacheService()
        {
            Dispose(false);
        }
    }
}