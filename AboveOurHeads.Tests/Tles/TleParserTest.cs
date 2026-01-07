using AboveOurHeads.Core.Models;
using AboveOurHeads.Services.Tles;
using AboveOurHeads.Tests.Helpers;

namespace AboveOurHeads.Tests.Tles
{
    public class TleParserTest
    {
        [Fact]
        public async Task ParseValidContent_MustReturn_ValidData()
        {
            // Arrange
            var content = ValidTle.Content;

            // Act
            var tleData = TleParser.Parse(content);

            // Assert
            Assert.NotNull(tleData);
            Assert.Equal("ISS (ZARYA)", tleData.Name);
        }

        [Fact]
        public async Task ParseInvalidContent_MustThrowFormatException()
        {
            // Arrange
            TleData tleData = new();
            var content = "dummy";

            // Act & Assert
            Assert.Throws<FormatException>(
                () => tleData = TleParser.Parse(content));
        }
    }
}
