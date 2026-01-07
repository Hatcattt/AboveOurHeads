using AboveOurHeads.Core.Interfaces;
using AboveOurHeads.Core.Models;
using AboveOurHeads.Services.Tles;
using AboveOurHeads.Tests.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace AboveOurHeads.Tests.Tles
{
    public class CachedTleProviderTests
    {
        private readonly int _noradIdForTesting = 25544;
        private readonly Mock<ILogger<CachedTleProvider>> _loggerMock;
        private readonly Mock<ITleProvider> _innerProviderMock;

        public CachedTleProviderTests()
        {
            _loggerMock = new Mock<ILogger<CachedTleProvider>>();
            _innerProviderMock = new Mock<ITleProvider>();
        }

        [Fact]
        public async Task GetTle_OnFirstCall_MustFetchFromInnerProvider()
        {
            // Arrange
            var expectedTle = new TleData
            {
                Name = ValidTle.Data().Name,
                Line1 = ValidTle.Data().Line1,
                Line2 = ValidTle.Data().Line2
            };

            _innerProviderMock
                .Setup(p => p.GetTleAsync(_noradIdForTesting))
                .ReturnsAsync(expectedTle);

            var cachedProvider = new CachedTleProvider(
                _loggerMock.Object,
                _innerProviderMock.Object,
                new MemoryCache(new MemoryCacheOptions()),
                TimeSpan.FromMinutes(10));

            // Act
            var result = await cachedProvider.GetTleAsync(_noradIdForTesting);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ISS (ZARYA)", result.Name);
            _innerProviderMock.Verify(
                p => p.GetTleAsync(_noradIdForTesting),
                Times.Once);
        }

        [Fact]
        public async Task GetTle_OnSecondCall_MustReturnFromCache()
        {
            // Arrange
            var expectedTle = new TleData
            {
                Name = ValidTle.Data().Name,
                Line1 = ValidTle.Data().Line1,
                Line2 = ValidTle.Data().Line2
            };

            _innerProviderMock
                .Setup(p => p.GetTleAsync(_noradIdForTesting))
                .ReturnsAsync(expectedTle);

            var cachedProvider = new CachedTleProvider(
                _loggerMock.Object,
                _innerProviderMock.Object,
                new MemoryCache(new MemoryCacheOptions()),
                TimeSpan.FromMinutes(10));

            // Act
            var firstResult = await cachedProvider.GetTleAsync(_noradIdForTesting);
            var secondResult = await cachedProvider.GetTleAsync(_noradIdForTesting);

            // Assert
            Assert.NotNull(firstResult);
            Assert.NotNull(secondResult);
            Assert.Equal(firstResult.Name, secondResult.Name);

            // Le provider interne ne doit être appelé qu'une seule fois
            _innerProviderMock.Verify(
                p => p.GetTleAsync(_noradIdForTesting),
                Times.Once);

            // Vérifier le log de cache hit
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("retrieved from cache")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetTle_AfterCacheExpiration_MustFetchAgainFromInnerProvider()
        {
            // Arrange
            var expectedTle = new TleData
            {
                Name = ValidTle.Data().Name,
                Line1 = ValidTle.Data().Line1,
                Line2 = ValidTle.Data().Line2
            };

            _innerProviderMock
                .Setup(p => p.GetTleAsync(_noradIdForTesting))
                .ReturnsAsync(expectedTle);

            var cachedProvider = new CachedTleProvider(
                _loggerMock.Object,
                _innerProviderMock.Object,
                new MemoryCache(new MemoryCacheOptions()),
                TimeSpan.FromMilliseconds(100)); // Cache très court pour le test

            // Act
            var firstResult = await cachedProvider.GetTleAsync(_noradIdForTesting);

            // Attendre l'expiration du cache
            await Task.Delay(TimeSpan.FromMilliseconds(150));

            var secondResult = await cachedProvider.GetTleAsync(_noradIdForTesting);

            // Assert
            Assert.NotNull(firstResult);
            Assert.NotNull(secondResult);

            // Le provider interne doit être appelé deux fois (cache expiré)
            _innerProviderMock.Verify(
                p => p.GetTleAsync(_noradIdForTesting),
                Times.Exactly(2));
        }

        [Fact]
        public async Task GetTle_WithMultipleSatellites_MustCacheEachSeparately()
        {
            // Arrange
            var tle1 = new TleData { Name = "ISS (ZARYA)", Line1 = "Line1A", Line2 = "Line2A" };
            var tle2 = new TleData { Name = "HST", Line1 = "Line1B", Line2 = "Line2B" };

            _innerProviderMock
                .Setup(p => p.GetTleAsync(25544))
                .ReturnsAsync(tle1);

            _innerProviderMock
                .Setup(p => p.GetTleAsync(20580))
                .ReturnsAsync(tle2);

            var cachedProvider = new CachedTleProvider(
                _loggerMock.Object,
                _innerProviderMock.Object,
                new MemoryCache(new MemoryCacheOptions()),
                TimeSpan.FromMinutes(10));

            // Act
            var result1 = await cachedProvider.GetTleAsync(25544);
            var result2 = await cachedProvider.GetTleAsync(20580);
            var result1Again = await cachedProvider.GetTleAsync(25544);
            var result2Again = await cachedProvider.GetTleAsync(20580);

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.Equal("ISS (ZARYA)", result1.Name);
            Assert.Equal("HST", result2.Name);

            // Chaque satellite doit être récupéré une seule fois
            _innerProviderMock.Verify(p => p.GetTleAsync(25544), Times.Once);
            _innerProviderMock.Verify(p => p.GetTleAsync(20580), Times.Once);
        }

        [Fact]
        public async Task GetTle_WhenInnerProviderReturnsNull_MustReturnNull()
        {
            // Arrange
            _innerProviderMock
                .Setup(p => p.GetTleAsync(_noradIdForTesting))
                .ReturnsAsync((TleData?)null);

            var cachedProvider = new CachedTleProvider(
                _loggerMock.Object,
                _innerProviderMock.Object,
                new MemoryCache(new MemoryCacheOptions()),
                TimeSpan.FromMinutes(10));

            // Act
            var result = await cachedProvider.GetTleAsync(_noradIdForTesting);

            // Assert
            Assert.Null(result);
            _innerProviderMock.Verify(
                p => p.GetTleAsync(_noradIdForTesting),
                Times.Once);
        }

        [Fact]
        public async Task GetTle_WhenInnerProviderReturnsNull_MustNotCache()
        {
            // Arrange
            _innerProviderMock
                .Setup(p => p.GetTleAsync(_noradIdForTesting))
                .ReturnsAsync((TleData?)null);

            var cachedProvider = new CachedTleProvider(
                _loggerMock.Object,
                _innerProviderMock.Object,
                new MemoryCache(new MemoryCacheOptions()),
                TimeSpan.FromMinutes(10));

            // Act
            var firstResult = await cachedProvider.GetTleAsync(_noradIdForTesting);
            var secondResult = await cachedProvider.GetTleAsync(_noradIdForTesting);

            // Assert
            Assert.Null(firstResult);
            Assert.Null(secondResult);

            // Le provider doit être appelé deux fois (pas de cache de null)
            _innerProviderMock.Verify(
                p => p.GetTleAsync(_noradIdForTesting),
                Times.Exactly(2));
        }

        [Fact]
        public async Task GetTle_WhenInnerProviderThrowsException_MustPropagateException()
        {
            // Arrange
            _innerProviderMock
                .Setup(p => p.GetTleAsync(_noradIdForTesting))
                .ThrowsAsync(new HttpRequestException("Network error"));

            var cachedProvider = new CachedTleProvider(
                _loggerMock.Object,
                _innerProviderMock.Object,
                new MemoryCache(new MemoryCacheOptions()),
                TimeSpan.FromMinutes(10));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                cachedProvider.GetTleAsync(_noradIdForTesting));
        }

        [Fact]
        public async Task GetTle_MustLogCacheMissWithProviderType()
        {
            // Arrange
            var expectedTle = new TleData
            {
                Name = ValidTle.Data().Name,
                Line1 = ValidTle.Data().Line1,
                Line2 = ValidTle.Data().Line2
            };

            _innerProviderMock
                .Setup(p => p.GetTleAsync(_noradIdForTesting))
                .ReturnsAsync(expectedTle);

            var cachedProvider = new CachedTleProvider(
                _loggerMock.Object,
                _innerProviderMock.Object,
                new MemoryCache(new MemoryCacheOptions()),
                TimeSpan.FromMinutes(10));

            // Act
            await cachedProvider.GetTleAsync(_noradIdForTesting);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString()!.Contains("Cache miss") &&
                        v.ToString()!.Contains("ITleProviderProxy")), // Mock type name
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetTle_MustLogCacheStorageWithExpiryTime()
        {
            // Arrange
            var expectedTle = new TleData
            {
                Name = ValidTle.Data().Name,
                Line1 = ValidTle.Data().Line1,
                Line2 = ValidTle.Data().Line2
            };

            _innerProviderMock
                .Setup(p => p.GetTleAsync(_noradIdForTesting))
                .ReturnsAsync(expectedTle);

            var cachedProvider = new CachedTleProvider(
                _loggerMock.Object,
                _innerProviderMock.Object,
                new MemoryCache(new MemoryCacheOptions()),
                TimeSpan.FromMinutes(10));

            // Act
            await cachedProvider.GetTleAsync(_noradIdForTesting);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString()!.Contains("cached for")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetTle_WithZeroCacheDuration_MustAlwaysFetchFromProvider()
        {
            // Arrange
            var expectedTle = new TleData
            {
                Name = ValidTle.Data().Name,
                Line1 = ValidTle.Data().Line1,
                Line2 = ValidTle.Data().Line2
            };

            _innerProviderMock
                .Setup(p => p.GetTleAsync(_noradIdForTesting))
                .ReturnsAsync(expectedTle);

            var cachedProvider = new CachedTleProvider(
                _loggerMock.Object,
                _innerProviderMock.Object,
                new MemoryCache(new MemoryCacheOptions()),
                TimeSpan.Zero); // Pas de cache

            // Act
            await cachedProvider.GetTleAsync(_noradIdForTesting);
            await cachedProvider.GetTleAsync(_noradIdForTesting);
            await cachedProvider.GetTleAsync(_noradIdForTesting);

            // Assert
            _innerProviderMock.Verify(
                p => p.GetTleAsync(_noradIdForTesting),
                Times.Exactly(3));
        }
    }
}