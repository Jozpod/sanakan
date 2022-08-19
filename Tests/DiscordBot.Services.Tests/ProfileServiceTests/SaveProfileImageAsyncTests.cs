using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.Abstractions;
using SixLabors.ImageSharp;
using System.Threading.Tasks;

namespace DiscordBot.ServicesTests.ProfileServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IProfileService.SaveProfileImageAsync(string, string, int, int, bool)"/> method.
    /// </summary>
    [TestClass]
    public class SaveProfileImageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Save_Image()
        {
            var imageUrl = "https://test.com/image.png";
            var filePath = "test";

            _fileSystemMock
                .Setup(pr => pr.Exists(filePath))
                .Returns(false);

            _imageProcessorMock
                .Setup(pr => pr.SaveImageFromUrlAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Size>(),
                    false))
                .Returns(Task.CompletedTask);

            var result = await _profileService.SaveProfileImageAsync(imageUrl, filePath, 300, 300);
            result.Should().Be(SaveResult.Success);
        }
    }
}
