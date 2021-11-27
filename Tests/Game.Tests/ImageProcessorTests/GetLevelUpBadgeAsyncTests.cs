using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using FluentAssertions;
using Sanakan.Game.Services.Abstractions;

namespace Sanakan.Game.Tests
{
    /// <summary>
    /// Defines tests for <see cref="IImageProcessor.GetLevelUpBadgeAsync(string, ulong, string, Discord.Color)"/> method.
    /// </summary>
    [TestClass]
    public class GetLevelUpBadgeAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Generate_Level_Up_Badge()
        {
            var name = "test-user";
            var level = 20ul;
            var avatarUrl = "https://test.com/avatar-url";
            var color = Discord.Color.Blue;

            MockHttpGetImage("TestData/card-image.png");

            var badgeImage = await _imageProcessor.GetLevelUpBadgeAsync(name, level, avatarUrl, color);
            badgeImage.Should().NotBeNull();

            await ShouldBeEqual("TestData/expected-badge.png", badgeImage);
        }
    }
}
