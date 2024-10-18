using System;
using System.Threading.Tasks;

namespace DualCache.NET
{
    public interface ICacheService : IDisposable
    {
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task<T> GetAsync<T>(string key);
        Task RemoveAsync(string key);
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
        Task<bool> ExistsAsync(string key);
    }
}