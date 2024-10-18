using System.Threading.Tasks;
using System;

namespace DualCache.NET
{
    /// <summary>
    /// Represents a caching service interface for managing cache operations.
    /// </summary>
    public interface ICacheService : IDisposable
    {
        /// <summary>
        /// Asynchronously sets a value in the cache with an optional expiration time.
        /// </summary>
        /// <typeparam name="T">The type of the value to be cached.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to be cached.</param>
        /// <param name="expiration">Optional. The time span after which the cache entry should expire.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary>
        /// Asynchronously retrieves a value from the cache.
        /// </summary>
        /// <typeparam name="T">The type of the value to be retrieved.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <returns>A task that represents the asynchronous operation, containing the cached value.</returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Asynchronously removes a value from the cache by its key.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RemoveAsync(string key);

        /// <summary>
        /// Asynchronously retrieves a value from the cache or adds it if it does not exist.
        /// </summary>
        /// <typeparam name="T">The type of the value to be retrieved or added.</typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="factory">A function that generates the value if it does not exist in the cache.</param>
        /// <param name="expiration">Optional. The time span after which the cache entry should expire.</param>
        /// <returns>A task that represents the asynchronous operation, containing the cached or newly added value.</returns>
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);

        /// <summary>
        /// Asynchronously checks if a value exists in the cache by its key.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>A task that represents the asynchronous operation, containing a boolean indicating whether the key exists in the cache.</returns>
        Task<bool> ExistsAsync(string key);
    }
}