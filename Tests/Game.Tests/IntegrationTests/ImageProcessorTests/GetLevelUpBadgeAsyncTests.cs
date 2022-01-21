using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Game.Services.Abstractions;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.IntegrationTests.ImageProcessorTests
{
    /// <summary>
    /// Defines integration tests for <see cref="IImageProcessor.GetLevelUpBadgeAsync(string, ulong, string, Discord.Color)"/> method.
    /// </summary>
#if DEBUG
    [TestClass]
#endif
    public class GetLevelUpBadgeAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Generate_Level_Up_Badge()
        {
            var name = "test-user";
            var level = 20ul;
            var avatarUrl = "https://test.com/avatar-url";
            var color = Discord.Color.Blue;

            MockHttpGetImage("card-image.png");

            var badgeImage = await _imageProcessor.GetLevelUpBadgeAsync(name, level, avatarUrl, color);
            badgeImage.Should().NotBeNull();

            await SaveImageAsync(badgeImage, "actual-badge.png");
            await ShouldBeEqual("expected-badge.png", badgeImage);
        }
    }
}
