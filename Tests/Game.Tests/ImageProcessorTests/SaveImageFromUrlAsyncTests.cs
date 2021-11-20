using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Sanakan.Game.Tests.ImageProcessorTests
{
    [TestClass]
    public class SaveImageFromUrlAsyncTests : Base
    {
        [TestMethod]
        public void Should_Return_Waifu_Image()
        {
            var imageUrl = "TestData/card-image.png";
            var filePath = "TestData/card-image.png";

            using var image = _imageProcessor.SaveImageFromUrlAsync(imageUrl, filePath);
            image.Should().NotBeNull();
        }
    }
}
