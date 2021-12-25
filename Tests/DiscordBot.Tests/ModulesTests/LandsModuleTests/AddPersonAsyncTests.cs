using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.LandsModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="LandsModule.AddPersonAsync(IGuildUser, string?)"/> method.
    /// </summary>
    [TestClass]
    public class AddPersonAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Tell_When_User_Does_Not_Own_Land()
        {
            var guildOptions = new GuildOptions(1ul, 50);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var roleIds = new List<ulong>();

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildOptions.Id))
                .ReturnsAsync(guildOptions);

            _landManagerMock
                .Setup(pr => pr.DetermineLand(
                    It.IsAny<IEnumerable<UserLand>>(),
                    It.IsAny<IEnumerable<ulong>>(),
                    It.IsAny<string?>()))
                .Returns(null as UserLand);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.AddPersonAsync(guildUserMock.Object, "test land");
        }

        [TestMethod]
        public async Task Should_Add_User_To_Land_And_Send_Confirm_Message()
        {
            var guildOptions = new GuildOptions(1ul, 50);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var roleIds = new List<ulong>();
            var userland = new UserLand
            {
                UnderlingId = 1ul,
            };

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            guildUserMock
                  .Setup(pr => pr.Mention)
                  .Returns("mention");

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildOptions.Id))
                .ReturnsAsync(guildOptions);

            _landManagerMock
                .Setup(pr => pr.DetermineLand(
                    It.IsAny<IEnumerable<UserLand>>(),
                    It.IsAny<IEnumerable<ulong>>(),
                    It.IsAny<string?>()))
                .Returns(userland);

            _guildMock
                .Setup(pr => pr.GetRole(userland.UnderlingId))
                .Returns(roleMock.Object);

            roleMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            guildUserMock
                .Setup(pr => pr.AddRoleAsync(roleMock.Object, null))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.AddPersonAsync(guildUserMock.Object, "test land");
        }
    }
}
