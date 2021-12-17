using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.BanUserAsync(IGuildUser, string, string)"/> method.
    /// </summary>
    [TestClass]
    public class BanUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Invalid_Duration()
        {
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.BanUserAsync(guildUserMock.Object, null);
        }

        [TestMethod]
        public async Task Should_Ban_User_And_Send_Confirm_Message()
        {
            var guildConfig = new GuildOptions(1ul, 50);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var textChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var duration = TimeSpan.FromHours(1);
            var penaltyInfo = new PenaltyInfo();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildConfig.Id))
                .ReturnsAsync(guildConfig);

            _guildConfigRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildConfig.Id);

            _guildMock
                .Setup(pr => pr.GetChannelAsync(guildConfig.NotificationChannelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(textChannelMock.Object);

            _moderatorServiceMock
                .Setup(pr => pr.BanUserAysnc(guildUserMock.Object, duration, Sanakan.DiscordBot.Constants.NoReason))
                .ReturnsAsync(penaltyInfo);

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("invoking user nickname");

            guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _moderatorServiceMock
                .Setup(pr => pr.NotifyAboutPenaltyAsync(guildUserMock.Object, textChannelMock.Object, penaltyInfo, It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.BanUserAsync(guildUserMock.Object, duration);
        }
    }
}
