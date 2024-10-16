using Moq;
using StackExchange.Redis;
using System.Text.Json;
using Xunit;

namespace DualCache.NET.UnitTests
{
    public class RedisCacheServiceTests
    {
        private readonly Mock<IDatabase> _redisDbMock;
        private readonly RedisCacheService _redisCacheService;

        public RedisCacheServiceTests()
        {
            _redisDbMock = new Mock<IDatabase>();
            var connectionMultiplexerMock = new Mock<IConnectionMultiplexer>();
            connectionMultiplexerMock.Setup(cm => cm.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_redisDbMock.Object);
            _redisCacheService = new RedisCacheService(connectionMultiplexerMock.Object);
        }

        [Fact]
        public async Task GetAsync_ReturnsValueFromRedis_WhenKeyExists()
        {
            // Arrange
            var key = "test-key";
            var value = JsonSerializer.Serialize("test-value");
            _redisDbMock.Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
                .ReturnsAsync(value);

            // Act
            var result = await _redisCacheService.GetAsync<string>(key);

            // Assert
            Assert.Equal("test-value", result);
        }

        [Fact]
        public async Task GetAsync_ReturnsNull_WhenKeyDoesNotExist()
        {
            // Arrange
            var key = "test-key";
            _redisDbMock.Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
                .ReturnsAsync((RedisValue)RedisValue.Null);

            // Act
            var result = await _redisCacheService.GetAsync<string>(key);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SetAsync_SetsValueInRedis()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";
            var serializedValue = JsonSerializer.Serialize(value);

            // Act
            await _redisCacheService.SetAsync(key, value, TimeSpan.FromMinutes(5));

            // Assert
            _redisDbMock.Verify(db => db.StringSetAsync(key, serializedValue, It.IsAny<TimeSpan>(),
                It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
        }

        [Fact]
        public async Task GetOrAddAsync_AddsToRedis_WhenKeyDoesNotExist()
        {
            // Arrange
            var key = "test-key";
            var factoryValue = "factory-value";
            _redisDbMock.Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
                .ReturnsAsync((RedisValue)RedisValue.Null);

            // Act
            var result = await _redisCacheService.GetOrAddAsync(key, async () => await Task.FromResult(factoryValue));

            // Assert
            Assert.Equal(factoryValue, result);
            _redisDbMock.Verify(db => db.StringSetAsync(key, It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
        }
    }
}