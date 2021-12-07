using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;
using Sanakan.DiscordBot.Services;
using Sanakan.DAL.Models.Configuration;
using FluentAssertions;

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

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildConfig.Id);

            _guildMock
                .Setup(pr => pr.Name)
                .Returns("test guild");

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildConfig.Id))
                .ReturnsAsync(guildConfig);

            _moderatorServiceMock
                .Setup(pr => pr.GetConfigurationAsync(guildConfig, _commandContextMock.Object, ConfigType.Global))
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
