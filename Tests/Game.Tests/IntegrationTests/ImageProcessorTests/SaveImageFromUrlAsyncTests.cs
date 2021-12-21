using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Game.Services.Abstractions;

namespace Sanakan.Game.Tests.IntegrationTests.ImageProcessorTests
{
    /// <summary>
    /// Defines integration tests for <see cref="IImageProcessor.SaveImageFromUrlAsync(string, string)"/> method.
    /// </summary>
#if DEBUG
    [TestClass]
#endif
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
