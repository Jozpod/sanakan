using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.AddReactionToMessageOnChannelInGuildAsync(ulong, ulong, ulong, string)"/> method.
    /// </summary>
    [TestClass]
    public class AddReactionToMessageOnChannelInGuildAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Add_Reaction()
        {
            var guildId = 1ul;
            var channelId = 1ul;
            var messageId = 1ul;
            var reaction = "<:Redpill:455880209711759400>";

            var guildChannelMock = new Mock<IGuildChannel>();
            var messageChannelMock = guildChannelMock.As<IMessageChannel>();
            var messageMock = new Mock<IMessage>();

            _discordClientMock
                .Setup(pr => pr.GetGuildAsync(guildId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_guildMock.Object);

            _guildMock
                .Setup(pr => pr.GetChannelAsync(channelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildChannelMock.Object);

            messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(messageMock.Object);

            messageMock
                .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _module.AddReactionToMessageOnChannelInGuildAsync(guildId, channelId, messageId, reaction);

            messageMock.Verify();
        }
    }
}
