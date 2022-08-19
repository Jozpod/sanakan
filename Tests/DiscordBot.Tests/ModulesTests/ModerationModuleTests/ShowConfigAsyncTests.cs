using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Services;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.ShowConfigAsync(ConfigType)"/> method.
    /// </summary>
    [TestClass]
    public class ShowConfigAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_Config()
        {
            var guildConfig = new GuildOptions(1ul, 50);
            guildConfig.WaifuConfig = new WaifuConfiguration();

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildConfig.Id);

            _guildMock
                .Setup(pr => pr.Name)
                .Returns("test guild");

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedById(guildConfig.Id))
                .ReturnsAsync(guildConfig);

            _moderatorServiceMock
                .Setup(pr => pr.GetConfigurationAsync(guildConfig, _guildMock.Object, ConfigType.Global))
                .ReturnsAsync(new EmbedBuilder());

            SetupSendMessage((message, embed) =>
            {
                embed.Title.Should().NotBeNull();
            });

            await _module.ShowConfigAsync();
        }

        [TestMethod]
        public async Task Should_Create_Waifu_Config_And_Send_Message_Containing_Config()
        {
            var guildConfig = new GuildOptions(1ul, 50);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildConfig.Id);

            _guildMock
                .Setup(pr => pr.Name)
                .Returns("test guild");

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedById(guildConfig.Id))
                .ReturnsAsync(guildConfig);

            _moderatorServiceMock
                .Setup(pr => pr.GetConfigurationAsync(guildConfig, _guildMock.Object, ConfigType.Global))
                .ReturnsAsync(new EmbedBuilder());

            _guildConfigRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Title.Should().NotBeNull();
            });

            await _module.ShowConfigAsync();
        }
    }
}
