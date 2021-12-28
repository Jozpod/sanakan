using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using Sanakan.DiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.MuteUserAsync(IGuildUser, string, string)"/> method.
    /// </summary>
    [TestClass]
    public class MuteUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_No_Guild()
        {
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildId = 1ul;
            var duration = TimeSpan.FromHours(1);
            var penaltyInfo = new PenaltyInfo();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(null as GuildOptions);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.MuteUserAsync(guildUserMock.Object, duration);
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_No_User_Role()
        {
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildConfig = new GuildOptions(1ul, 50);
            var duration = TimeSpan.FromHours(1);
            var penaltyInfo = new PenaltyInfo();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildConfig.Id))
                .ReturnsAsync(guildConfig);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildConfig.Id);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.MuteUserAsync(guildUserMock.Object, duration);
        }

        [TestMethod]
        public async Task Should_Mute_User_And_Send_Confirm_Message()
        {
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildConfig = new GuildOptions(1ul, 50);
            var muteRoleMock = new Mock<IRole>(MockBehavior.Strict);
            var textChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var guildChannelMock = textChannelMock.As<IGuildChannel>();
            guildConfig.MuteRoleId = 1ul;
            guildConfig.UserRoleId = 2ul;
            guildConfig.NotificationChannelId = 3ul;
            var roleIds = new List<ulong> { };
            var duration = TimeSpan.FromHours(1);
            var penaltyInfo = new PenaltyInfo();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildConfig.Id))
                .ReturnsAsync(guildConfig);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildConfig.Id);

            _guildMock
                .Setup(pr => pr.GetChannelAsync(guildConfig.NotificationChannelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(textChannelMock.Object);

            _guildMock
                .Setup(pr => pr.GetRole(It.IsAny<ulong>()))
                .Returns(muteRoleMock.Object);

            muteRoleMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _moderatorServiceMock
                .Setup(pr => pr.MuteUserAsync(
                    guildUserMock.Object,
                    muteRoleMock.Object,
                    null,
                    muteRoleMock.Object,
                    duration,
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<ModeratorRoles>>()))
                .ReturnsAsync(penaltyInfo);

            _moderatorServiceMock
                .Setup(pr => pr.NotifyAboutPenaltyAsync(
                    guildUserMock.Object,
                    textChannelMock.Object,
                    penaltyInfo,
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.MuteUserAsync(guildUserMock.Object, duration);
        }
    }
}
