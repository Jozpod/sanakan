using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.GenerateMissingUsersListAsync"/> method.
    /// </summary>
    [TestClass]
    public class GenerateMissingUsersListAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Messge_Containing_Missing_Users()
        {
            var guilds = new List<IGuild> { _guildMock.Object };
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var users = new List<IGuildUser> { guildUserMock.Object };
            var userId = 1ul;
            var userIds = new List<ulong> { userId };

            _discordClientMock
                .Setup(pr => pr.GetGuildsAsync(CacheMode.AllowDownload, null))
                .ReturnsAsync(guilds);

            _guildMock
                .Setup(pr => pr.GetUsersAsync(CacheMode.AllowDownload, null))
                .ReturnsAsync(users);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            _userRepositoryMock
                .Setup(pr => pr.GetByExcludedDiscordIdsAsync(It.IsAny<IEnumerable<ulong>>()))
                .ReturnsAsync(userIds);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.GenerateMissingUsersListAsync();
        }
    }
}
