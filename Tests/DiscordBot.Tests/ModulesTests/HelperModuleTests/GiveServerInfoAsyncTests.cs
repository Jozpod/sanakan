using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="HelperModule.GetServerInfoAsync"/> method.
    /// </summary>
    [TestClass]
    public class GiveServerInfoAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Give_Server_Info()
        {
            var embed = new EmbedBuilder().Build();

            _helperServiceMock
                .Setup(pr => pr.GetInfoAboutServerAsync(_guildMock.Object))
                .ReturnsAsync(embed);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.GetServerInfoAsync();
        }
    }
}
