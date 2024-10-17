using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Xunit;
using Assert = Xunit.Assert;

namespace DualCache.NET.UnitTests
{
    public class MemoryCacheServiceTests
    {
        private readonly IMemoryCache _memoryCacheMock;
        private readonly MemoryCacheService _memoryCacheService;

        public MemoryCacheServiceTests()
        {
            _memoryCacheMock = Substitute.For<IMemoryCache>();
            _memoryCacheService = new MemoryCacheService(_memoryCacheMock);
        }

        [Fact]
        public async Task GetAsync_ReturnsNull_WhenKeyDoesNotExist()
        {
            // Arrange
            var key = "test-key";
            string? cachedValue = null;

            _memoryCacheMock.TryGetValue(key, out Arg.Any<object>()).Returns(false);

            // Act
            var result = await _memoryCacheService.GetAsync<string>(key);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveAsync_RemovesValueFromCache()
        {
            // Arrange
            var key = "test-key";

            // Act
            await _memoryCacheService.RemoveAsync(key);

            // Assert
            _memoryCacheMock.Received(1).Remove(key);
        }

        [Fact]
        public async Task GetOrAddAsync_AddsValue_WhenKeyDoesNotExist()
        {
            // Arrange
            var key = "test-key";
            var factoryValue = "factory-value";

            _memoryCacheMock.TryGetValue(key, out Arg.Any<object>()).Returns(false);

            // Act
            var result = await _memoryCacheService.GetOrAddAsync(key, () => Task.FromResult(factoryValue));

            // Assert
            Assert.Equal(factoryValue, result);
        }
    }
}