using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Game.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ServicesTests.ProfileServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IProfileService.BuildListViewAsync(IEnumerable{User}, TopType, IGuild)"/> method.
    /// </summary>
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
            var topType = TopType.AcCount;
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
