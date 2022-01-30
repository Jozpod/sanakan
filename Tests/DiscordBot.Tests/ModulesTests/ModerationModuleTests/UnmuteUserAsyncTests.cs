using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.UnmuteUserAsync(IGuildUser)"/> method.
    /// </summary>
    [TestClass]
    public class UnmuteUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Unmute_User_And_Send_Message()
        {
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildConfig = new GuildOptions(1ul, 50);
            var muteRoleMock = new Mock<IRole>(MockBehavior.Strict);
            guildConfig.MuteRoleId = 1ul;
            var roleIds = new List<ulong> { guildConfig.MuteRoleId };

            muteRoleMock
                .Setup(pr => pr.Id)
                .Returns(guildConfig.MuteRoleId);

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _guildMock
                .Setup(pr => pr.GetRole(0))
                .Returns<IRole?>(null);

            _guildMock
                .Setup(pr => pr.GetRole(guildConfig.MuteRoleId))
                .Returns(muteRoleMock.Object);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildConfig.Id);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedById(guildConfig.Id))
                .ReturnsAsync(guildConfig);

            _moderatorServiceMock
                .Setup(pr => pr.UnmuteUserAsync(guildUserMock.Object, muteRoleMock.Object, null))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.UnmuteUserAsync(guildUserMock.Object);
        }
    }
}
