using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using FluentAssertions;
using Sanakan.Game.Models;
using Sanakan.DAL.Models;
using System;

namespace DiscordBot.ServicesTests.ProfileServiceTests
{
    [TestClass]
    public class BuildListViewAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Generate_List_View()
        {
            var user = new User(1ul, DateTime.UtcNow);
            var userList = new[]
            {
                user,
            };
            var topType = TopType.AcCnt;
            var guildMock = new Mock<IGuild>();
            var guildUserMock = new Mock<IGuildUser>();

            guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("user");

            guildMock
                .Setup(pr => pr.GetUserAsync(
                    user.Id,
                    It.IsAny<CacheMode>(),
                    It.IsAny<RequestOptions>()))
                .ReturnsAsync(guildUserMock.Object);

            var result = await _profileService.BuildListViewAsync(userList, topType, guildMock.Object);
            result.Should().NotBeEmpty();
        }
    }
}
