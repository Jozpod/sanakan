using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Game.Services.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace Sanakan.Game.Tests.ImageProcessorTests
{
    /// <summary>
    /// Defines tests for <see cref="IImageProcessor.GetCatchThatWaifuImage(Image{Rgba32}, string, int, int)"/> method.
    /// </summary>
    [TestClass]
    public class GetCatchThatWaifuImageTests : Base
    {
        [TestMethod]
        public void Should_Return_Waifu_Image()
        {
            var cardImage = new Image<Rgba32>(100, 100);
            var imageUrl = "TestData/card-image.png";
            var xPos = 0;
            var yPos = 0;
            var stream = new MemoryStream();
            cardImage.Save(stream, new PngEncoder());
            stream.Seek(0, SeekOrigin.Begin);

            _fileSystemMock
                .Setup(pr => pr.OpenRead(It.IsAny<string>()))
                .Returns(stream);

            using var image = _imageProcessor.GetCatchThatWaifuImage(cardImage, imageUrl, xPos, yPos);
            image.Should().NotBeNull();
        }
    }
}
