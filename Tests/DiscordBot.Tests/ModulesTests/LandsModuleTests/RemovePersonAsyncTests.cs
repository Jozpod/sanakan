using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;
using Sanakan.DAL.Models.Configuration;
using System.Collections.Generic;
using FluentAssertions;

namespace DiscordBot.ModulesTests.LandsModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="LandsModule.RemovePersonAsync(IGuildUser, string?)"/> method.
    /// </summary>
    [TestClass]
    public class RemovePersonAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Remove_Person()
        {
            var guildOptions = new GuildOptions(1ul, 50);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var userLand = new UserLand
            {
                Name = "test land",
                UnderlingId = 1ul,
            };
            var roleIds = new List<ulong> { userLand.UnderlingId  };

            roleMock
                .Setup(pr => pr.Id)
                .Returns(userLand.UnderlingId);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            _guildMock
                .Setup(pr => pr.GetRole(userLand.UnderlingId))
                .Returns(roleMock.Object);

            _landManagerMock
                .Setup(pr => pr.DetermineLand(
                    It.IsAny<IEnumerable<UserLand>>(),
                    It.IsAny<IEnumerable<ulong>>(),
                    It.IsAny<string?>()))
                .Returns(userLand);

            guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            guildUserMock
                .Setup(pr => pr.RemoveRoleAsync(roleMock.Object, null))
                .Returns(Task.CompletedTask);

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildOptions.Id))
                .ReturnsAsync(guildOptions);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.RemovePersonAsync(guildUserMock.Object);
        }
    }
}
