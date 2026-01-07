using AboveOurHeads.Core.Models;
using AboveOurHeads.Services.Tles;
using AboveOurHeads.Tests.Helpers.HttpHandler;
using Microsoft.Extensions.Logging;
using Moq;

namespace AboveOurHeads.Tests.Tles
{
    public class HttpTleProviderTest
    {
        private readonly int noradIdForTesting = 25544;
        private readonly Mock<ILogger<HttpTleProvider>> _loggerMock;

        public HttpTleProviderTest()
        {
            _loggerMock = new Mock<ILogger<HttpTleProvider>>();
        }

        [Fact]
        public async Task GetTle_MustReturn_ValidResult()
        {
            // Arrange
            var handler = new SuccessTleHttpMessageHandler();
            var httpClientMock = new Mock<HttpClient>(handler);

            var tleProvider = new HttpTleProvider(_loggerMock.Object, httpClientMock.Object);

            // Act
            var tleData = await tleProvider.GetTleAsync(noradIdForTesting);

            // Assert
            Assert.NotNull(tleData);
            Assert.Equal("ISS (ZARYA)", tleData.Name);
        }

        [Fact]
        public async Task GetTle_WithInvalidNoradId_MustThrowArgumentException()
        {
            // Arrange
            TleData data = new();
            var handler = new SuccessTleHttpMessageHandler();
            var httpClientMock = new Mock<HttpClient>(handler);

            var tleProvider = new HttpTleProvider(_loggerMock.Object, httpClientMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                async () => await tleProvider.GetTleAsync(-10));
        }

        // TODO: Tester quand TleData retourn null (non trouvé parmis les noradId DEPUIS le site)

        [Fact]
        public async Task GetTleWithNoInternet_MustReturnNull_WithLogging()
        {
            // Arrange
            var handler = new NoInternetHttpMessageHandler();
            var httpClientMock = new Mock<HttpClient>(handler);

            var tleProvider = new HttpTleProvider(_loggerMock.Object, httpClientMock.Object);

            // Act
            var result = await tleProvider.GetTleAsync(noradIdForTesting);

            // Assert
            Assert.Null(result);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Network error")),
                    It.IsAny<HttpRequestException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetTleWithRequestTimeout_MustReturnNull_WithLogging()
        {
            // Arrange
            var handler = new TimeoutHttpMessageHandler();
            var httpClientMock = new Mock<HttpClient>(handler);

            var tleProvider = new HttpTleProvider(_loggerMock.Object, httpClientMock.Object);

            // Act
            var result = await tleProvider.GetTleAsync(noradIdForTesting);

            // Assert
            Assert.Null(result);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Request timeout")),
                    It.IsAny<TaskCanceledException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
                    }
    }
}
