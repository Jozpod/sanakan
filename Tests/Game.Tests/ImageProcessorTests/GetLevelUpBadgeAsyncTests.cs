using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using FluentAssertions;

namespace Sanakan.Game.Tests
{
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
