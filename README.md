# DualCache.NET

![DualCache.NET Logo](https://gulivera.net/DualCache.Net-90x90.png)

**DualCache.NET** is a versatile caching library for .NET that provides seamless integration with both Redis and in-memory caching solutions. Enhance your application’s performance by reducing database load and improving response times effortlessly.

## Features

- **Multi-Cache Support**: Switch between Redis and in-memory caching based on your needs.
- **Simple Interface**: Easy-to-use methods for setting, retrieving, and removing cached items.
- **GetOrAdd Functionality**: Automatically retrieve or create cached items, minimizing boilerplate code.
- **Configuration Flexibility**: Easily configure caching options through dependency injection.

## Installation

You can install **DualCache.NET** via NuGet Package Manager:

```bash
Install-Package DualCache.NET
```

## Configuration RedisCache
To configure the cache service in your .NET application, you need to set it up in your Startup.cs or Program.cs file and in your appsettings.json, add the RedisConnection. 
Here's how you can do it:
```json
{
  "ConnectionStrings": {
    "RedisConnection": "your_redis_connection_string"
  }
}
```
```csharp
builder.Services.AddRedisCache(Configuration);
```

## Configuration MemoryCache
To inject memory cache, simply call:
```csharp
builder.Services.AddMemoryCache();
```

## Usage
Once the cache is configured, you can use it in your application as follows:

Example Service with Caching:
```csharp
using DualCache.NET;

public class ExampleService
{
    private readonly ICacheService _cacheService;

    public ExampleService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<string> GetCachedValue(string key)
    {
        return await _cacheService.GetAsync<string>(key);
    }

    public async Task SetCachedValue(string key, string value)
    {
        await _cacheService.SetAsync(key, value);
    }

    public async Task RemoveCachedValue(string key)
    {
        await _cacheService.RemoveAsync(key);
    }

    public async Task<string> GetCachedOrDefaultValueAsync(string key)
    {
        // Use GetOrAddAsync to either get the cached value or add a new one if it doesn't exist
        var value = await _cacheService.GetOrAddAsync(key, TimeSpan.FromMinutes(15));  // Set cache expiration to 15 minutes

        return value;
    }
}
```
