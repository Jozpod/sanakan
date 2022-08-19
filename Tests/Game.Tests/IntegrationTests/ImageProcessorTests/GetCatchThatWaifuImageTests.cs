using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Game.Services.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.IntegrationTests.ImageProcessorTests
{
    /// <summary>
    /// Defines integration tests for <see cref="IImageProcessor.GetCatchThatWaifuImage(Image{Rgba32}, string, int, int)"/> method.
    /// </summary>
#if DEBUG
    [TestClass]
#endif
    public class GetCatchThatWaifuImageTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Waifu_Image()
        {
            var card = new Image<Rgba32>(100, 100);
            var imageUrl = "TestData/card-image.png";
            var xPos = 0;
            var yPos = 0;

            using var image = await _imageProcessor.GetCatchThatWaifuImageAsync(card, imageUrl, xPos, yPos);
            image.Should().NotBeNull();
        }
    }
}
