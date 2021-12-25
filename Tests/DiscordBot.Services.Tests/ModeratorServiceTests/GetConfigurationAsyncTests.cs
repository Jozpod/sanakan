using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.Services.Tests.ModeratorServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IModeratorService.GetConfigurationAsync(GuildOptions, IGuild, ConfigType)"/> method.
    /// </summary>
    [TestClass]
    public class GetConfigurationAsyncTests : Base
    {

        [TestMethod]
        [DataRow(ConfigType.CommandChannels)]
        [DataRow(ConfigType.Global)]
        [DataRow(ConfigType.IgnoredChannels)]
        [DataRow(ConfigType.Lands)]
        [DataRow(ConfigType.LevelRoles)]
        [DataRow(ConfigType.ModeratorRoles)]
        [DataRow(ConfigType.NonSupChannels)]
        [DataRow(ConfigType.RichMessages)]
        [DataRow(ConfigType.NonExpChannels)]
        [DataRow(ConfigType.SelfRoles)]
        [DataRow(ConfigType.WaifuCmdChannels)]
        [DataRow(ConfigType.WaifuFightChannels)]
        public async Task Should_Return_Configuration(ConfigType configType)
        {
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var textChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var guildOptions = new GuildOptions
            {

            };

            guildMock
                .Setup(pr => pr.GetRole(It.IsAny<ulong>()))
                .Returns(roleMock.Object);

            guildMock
               .Setup(pr => pr.GetTextChannelAsync(It.IsAny<ulong>(), CacheMode.AllowDownload, null))
               .ReturnsAsync(textChannelMock.Object);

            guildMock
               .Setup(pr => pr.GetTextChannelsAsync(CacheMode.AllowDownload, null))
               .ReturnsAsync(new List<ITextChannel>());

            textChannelMock
                .Setup(pr => pr.Mention)
                .Returns("channel mention");

            roleMock
                .Setup(pr => pr.Name)
                .Returns("role name");

            roleMock
                .Setup(pr => pr.Mention)
                .Returns("role mention");

            var result = await _moderatorService.GetConfigurationAsync(guildOptions, guildMock.Object, configType);
            result.Should().NotBeNull();
        }
    }
}
