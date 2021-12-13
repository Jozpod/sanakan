using Microsoft.VisualStudio.TestTools.UnitTesting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using FluentAssertions;
using Sanakan.Game.Services.Abstractions;

namespace Sanakan.Game.Tests.ImageProcessorTests
{
    /// <summary>
    /// Defines tests for <see cref="IImageProcessor.GetCatchThatWaifuImage(Image{Rgba32}, string, int, int)"/> method.
    /// </summary>
#if DEBUG
    [TestClass]
#endif
    public class GetCatchThatWaifuImageTests : Base
    {
        [TestMethod]
        public void Should_Return_Waifu_Image()
        {
            var card = new Image<Rgba32>(100, 100);
            var imageUrl = "TestData/card-image.png";
            var xPos = 0;
            var yPos = 0;

            using var image = _imageProcessor.GetCatchThatWaifuImage(card, imageUrl, xPos, yPos);
            image.Should().NotBeNull();
        }
    }
}
