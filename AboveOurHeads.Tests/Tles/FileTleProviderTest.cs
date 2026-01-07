using AboveOurHeads.Services.Tles;
using AboveOurHeads.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace AboveOurHeads.Tests.Tles
{
    public class FileTleProviderTest
    {
        private readonly int noradIdForTesting = 25544;
        private readonly Mock<ILogger<FileTleProvider>> _loggerMock;

        public FileTleProviderTest()
        {
            _loggerMock = new Mock<ILogger<FileTleProvider>>();
        }

        [Fact]
        public async Task GetTle_MustReturn_ValidResult()
        {
            // Arrange
            var folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(folder);
            var path = Path.Combine(folder, "25544.txt");

            File.WriteAllText(path, ValidTle.Content);

            var provider = new FileTleProvider(_loggerMock.Object, path);

            // Act
            var tleData = await provider.GetTleAsync(noradIdForTesting);

            // Assert
            Assert.NotNull(tleData);
            Assert.Equal("ISS (ZARYA)", tleData.Name);
        }

        [Fact]
        public async Task GetTle_WithInvalidNoradId_MustThrowArgumentException()
        {
            // Arrange
            var path = "dummy-path";
            var provider = new FileTleProvider(_loggerMock.Object, path);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                provider.GetTleAsync(-10));
        }

        [Fact]
        public async Task GetTle_WhenFileNameDoesNotMatchNoradId_MustThrowArgumentException()
        {
            // Arrange
            var folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(folder);
            var path = Path.Combine(folder, "123.txt");

            File.WriteAllText(path, ValidTle.Content);

            var provider = new FileTleProvider(_loggerMock.Object, path);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                provider.GetTleAsync(noradIdForTesting));
        }

        [Fact]
        public async Task GetTle_WhenFileNotExist_MustThrowFileNotFoundException()
        {
            // Arrange
            var path = "dummy-path";
            var provider = new FileTleProvider(_loggerMock.Object, path);

            // Act
            var tleData = await provider.GetTleAsync(noradIdForTesting);

            // Assert
            Assert.Null(tleData);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("file operation")),
                    It.IsAny<FileNotFoundException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
