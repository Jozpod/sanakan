using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="HelperModule.ReportUserAsync(ulong, string)"/> method.
    /// </summary>
    [TestClass]
    public class ReportUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Report_User()
        {
            var messageId = 1ul;
            var reason = "reason";
            var guildOption = new GuildOptions(1ul, 50);
            guildOption.RaportChannelId = 1ul;
            var guildChannelMock = new Mock<IGuildChannel>(MockBehavior.Strict);
            var textChannelMock = guildChannelMock.As<ITextChannel>();
            var messageMock = guildChannelMock.As<IMessage>();
            var userMessageMock = messageMock.As<IUserMessage>();
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var utcNow = DateTime.UtcNow;

            var userMessageMock1 = guildChannelMock.As<IUserMessage>();

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOption.Id);

            _guildMock
                .Setup(pr => pr.GetChannelAsync(guildOption.RaportChannelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildChannelMock.Object);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildOption.Id))
                .ReturnsAsync(guildOption);

            _contextMessageMock
                .Setup(pr => pr.DeleteAsync(null))
                .Returns(Task.CompletedTask);

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(messageMock.Object);

            messageMock
                .Setup(pr => pr.Author)
                .Returns(userMock.Object);

            messageMock
                .Setup(pr => pr.Channel)
                .Returns(textChannelMock.Object);

            guildChannelMock
                .Setup(pr => pr.GuildId)
                .Returns(guildOption.Id);

            guildChannelMock
                .Setup(pr => pr.Id)
                .Returns(9ul);

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            userMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            messageMock
                .Setup(pr => pr.CreatedAt)
                .Returns(utcNow);

            userMock
                .Setup(pr => pr.Id)
                .Returns(2ul);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMock
                .Setup(pr => pr.Username)
                .Returns("username");

            textChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(userMessageMock.Object);

            _helperServiceMock
                .Setup(pr => pr.BuildRaportInfo(
                    messageMock.Object,
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ulong>()))
                .Returns(new EmbedBuilder().Build());

            userMessageMock1
                .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
                .Returns(Task.CompletedTask);

            _guildConfigRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            SetupSendMessage();

            await _module.ReportUserAsync(messageId, reason);
        }
    }
}
