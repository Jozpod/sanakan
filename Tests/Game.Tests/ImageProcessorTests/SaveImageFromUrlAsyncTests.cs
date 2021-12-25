using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Game.Services.Abstractions;
using SixLabors.Primitives;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.ImageProcessorTests
{
    /// <summary>
    /// Defines tests for <see cref="IImageProcessor.SaveImageFromUrlAsync(string, string)"/> method.
    /// </summary>
    [TestClass]
    public class SaveImageFromUrlAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Stretch_And_Save_Image()
        {
            var imageUrl = "TestData/card-image.png";
            var filePath = "TestData/card-image.png";

            _imageResolverMock
                .Setup(pr => pr.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(Utils.CreateFakeImage);

            _fileSystemMock
                .Setup(pr => pr.OpenWrite(It.IsAny<string>()))
                .Returns(() => new MemoryStream());

            await _imageProcessor.SaveImageFromUrlAsync(imageUrl, filePath, new Size(300, 300), true);
        }

        [TestMethod]
        public async Task Should_Save_Image()
        {
            var imageUrl = "TestData/card-image.png";
            var filePath = "TestData/card-image.png";

            _imageResolverMock
                .Setup(pr => pr.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(Utils.CreateFakeImage);

            _fileSystemMock
                .Setup(pr => pr.OpenWrite(It.IsAny<string>()))
                .Returns(() => new MemoryStream());

            await _imageProcessor.SaveImageFromUrlAsync(imageUrl, filePath);
        }
    }
}
