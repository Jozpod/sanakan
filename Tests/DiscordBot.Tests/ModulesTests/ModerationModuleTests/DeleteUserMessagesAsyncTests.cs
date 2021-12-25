using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.DeleteUserMessagesAsync(IGuildUser)"/> method.
    /// </summary>
    [TestClass]
    public class DeleteUserMessagesAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Delete_User_Messages_Send_Confirm_Message()
        {
            var count = 10;
            var messageMock = new Mock<IMessage>();
            var guildUserMock = new Mock<IGuildUser>();
            var messages = new List<IMessage> { messageMock.Object };
            var messagesBatch = Enumerable.Repeat(messages, count)
                .ToAsyncEnumerable();

            _contextMessageMock
                .Setup(pr => pr.DeleteAsync(null))
                .Returns(Task.CompletedTask);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            messageMock
                .Setup(pr => pr.Author)
                .Returns(guildUserMock.Object);

            _textChannelMock
                .Setup(pr => pr.GetMessagesAsync(100, CacheMode.AllowDownload, null))
                .Returns(messagesBatch);

            _textChannelMock
                .Setup(pr => pr.DeleteMessagesAsync(It.IsAny<IEnumerable<IMessage>>(), null))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.DeleteUserMessagesAsync(guildUserMock.Object);
        }
    }
}
