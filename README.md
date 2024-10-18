# DualCache.NET

![DualCache.NET Logo](icon.png)

**DualCache.NET** is a versatile caching library for .NET that provides seamless integration with both Redis and in-memory caching solutions. Enhance your application’s performance by reducing database load and improving response times effortlessly.

![Build Status](https://github.com/guliv3r/DualCache.NET/actions/workflows/build-test.yml/badge.svg)

## Features

- **Multi-Cache Support**: Switch between Redis and in-memory caching based on your needs.
- **Simple Interface**: Easy-to-use methods for setting, retrieving, and removing cached items.
- **GetOrAdd Functionality**: Automatically retrieve or create cached items, minimizing boilerplate code.
- **Configuration Flexibility**: Easily configure caching options through dependency injection.
- **Seamless Switching**: You can switch from RedisCache to MemoryCache without any code changes!

## Installation

You can install **DualCache.NET** via NuGet Package Manager:

```bash
Install-Package DualCache.NET
```

## Configuration RedisCache
To configure the cache service in your .NET application, you can set it up in your `Startup.cs` or `Program.cs` file. You have two options for providing the Redis connection string: through the `appsettings.json` file or directly as a string.

Here's how you can do it:
### Option 1: Using appsettings.json
If you want to use an `IConfiguration` instance, add the Redis connection string in your `appsettings.json` like this:

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

### Option 2: Using a String Connection
If you want to use an `IConfiguration` instance, add the Redis connection string in your `appsettings.json` like this:

```csharp
builder.Services.AddRedisCache("your_redis_connection_string");
```

## Configuration MemoryCache
To inject memory cache, simply call:
```csharp
builder.Services.AddCustomMemoryCache();
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
	
	public async Task<string> GetOrAddAsync(string key)
    {
       return await _cacheService.GetOrAddAsync(
                "key",
                async () => await Task.FromResult<string>("value"));
    }

    public async Task RemoveCachedValue(string key)
    {
        await _cacheService.RemoveAsync(key);
    }

    public async Task<bool> KeyExist(string key)
    {
        return await _cacheService.ExistsAsync(key);
    }
}
```
