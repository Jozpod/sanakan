using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.QuoteAndSendAsync(ulong, ulong)"/> method.
    /// </summary>
    [TestClass]
    public class QuoteAndSendAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message()
        {
            var messageId = 1ul;
            var channelId = 1ul;
            var textChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var messageChannelMock = textChannelMock.As<IMessageChannel>();

            _guildMock
                .Setup(pr => pr.GetChannelAsync(channelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(textChannelMock.Object);

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_userMessageMock.Object);

            _moderatorServiceMock
                .Setup(pr => pr.BuildTodo(_userMessageMock.Object, _guildUserMock.Object))
                .Returns(new EmbedBuilder().Build());

            _contextMessageMock
                .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                .Returns(Task.CompletedTask);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object);

            textChannelMock
                .Setup(pr => pr.GuildId)
                .Returns(1ul);

            textChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            textChannelMock
               .Setup(pr => pr.SendMessageAsync(
                   It.IsAny<string>(),
                   It.IsAny<bool>(),
                   It.IsAny<Embed>(),
                   It.IsAny<RequestOptions>(),
                   It.IsAny<AllowedMentions>(),
                   It.IsAny<MessageReference>()))
               .ReturnsAsync(_userMessageMock.Object);

            await _module.QuoteAndSendAsync(messageId, channelId);
        }
    }
}
