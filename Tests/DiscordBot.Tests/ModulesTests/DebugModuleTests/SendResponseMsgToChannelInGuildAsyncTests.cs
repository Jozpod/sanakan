using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.SendResponseMsgToChannelInGuildAsync(ulong, ulong, ulong, string)"/> method.
    /// </summary>
    [TestClass]
    public class SendResponseMsgToChannelInGuildAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_To_Given_Channel()
        {
            var guildId = 1ul;
            var channelId = 1ul;
            var messageId = 1ul;
            var messageContent = "content";
            var guildChannelMock = new Mock<IGuildChannel>(MockBehavior.Strict);
            var messageChannelMock = guildChannelMock.As<IMessageChannel>();

            _discordClientMock
                .Setup(pr => pr.GetGuildAsync(guildId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_guildMock.Object);

            _guildMock
                .Setup(pr => pr.GetChannelAsync(channelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildChannelMock.Object);

            messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_userMessageMock.Object);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object);

            SetupSendMessage();

            await _module.SendResponseMsgToChannelInGuildAsync(guildId, channelId, messageId, messageContent);
        }
    }
}
