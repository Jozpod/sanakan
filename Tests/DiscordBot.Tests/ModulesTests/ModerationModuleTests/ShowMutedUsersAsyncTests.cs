using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.ShowMutedUsersAsync"/> method.
    /// </summary>
    [TestClass]
    public class ShowMutedUsersAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_Muted_User()
        {
            _moderatorServiceMock
                .Setup(pr => pr.GetMutedListAsync(_guildMock.Object))
                .ReturnsAsync(new EmbedBuilder().Build());

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.ShowMutedUsersAsync();
        }
    }
}
