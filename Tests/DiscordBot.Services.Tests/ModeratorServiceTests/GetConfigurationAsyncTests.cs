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
            var guildOptions = new GuildOptions()
            {
                RaportChannelId = 1ul,
                WaifuRoleId = 1ul,
                GlobalEmotesRoleId = 1ul,
                GreetingChannelId = 1ul,
                LogChannelId = 1ul,
                ToDoChannelId = 1ul,
                NsfwChannelId = 1ul,
                QuizChannelId = 1ul,
            };
            var channelId = 1ul;
            var roleId = 1ul;
            guildOptions.ChannelsWithoutExperience.Add(new WithoutExpChannel { ChannelId = channelId });
            guildOptions.IgnoredChannels.Add(new WithoutMessageCountChannel { ChannelId = channelId });
            guildOptions.CommandChannels.Add(new CommandChannel { ChannelId = channelId });
            guildOptions.ChannelsWithoutSupervision.Add(new WithoutSupervisionChannel { ChannelId = channelId });
            guildOptions.RolesPerLevel.Add(new LevelRole { RoleId = roleId });
            guildOptions.ModeratorRoles.Add(new ModeratorRoles { RoleId = roleId });
            guildOptions.SelfRoles.Add(new SelfRole { RoleId = roleId });
            guildOptions.WaifuConfig = new WaifuConfiguration()
            {
                MarketChannelId = 1ul,
                DuelChannelId = 1ul,
                TrashCommandsChannelId = 1ul,
                TrashSpawnChannelId = 1ul,
                TrashFightChannelId = 1ul,
                SpawnChannelId = 1ul,
            };
            guildOptions.WaifuConfig.FightChannels.Add(new WaifuFightChannel { ChannelId = channelId });
            guildOptions.WaifuConfig.CommandChannels.Add(new WaifuCommandChannel { ChannelId = channelId });
            guildOptions.Lands.Add(new UserLand { ManagerId = 1ul, UnderlingId = 1ul, });

            guildMock
                .Setup(pr => pr.GetRole(It.IsAny<ulong>()))
                .Returns(roleMock.Object);

            guildMock
               .Setup(pr => pr.GetTextChannelAsync(It.IsAny<ulong>(), CacheMode.AllowDownload, null))
               .ReturnsAsync(textChannelMock.Object);

            guildMock
               .Setup(pr => pr.GetTextChannelsAsync(CacheMode.AllowDownload, null))
               .ReturnsAsync(new List<ITextChannel> { textChannelMock.Object });

            textChannelMock
               .Setup(pr => pr.Id)
               .Returns(1ul);

            textChannelMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

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
