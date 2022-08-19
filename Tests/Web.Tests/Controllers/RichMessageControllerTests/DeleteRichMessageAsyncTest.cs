using Discord;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Configuration;
using Sanakan.Web.Controllers;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.RichMessageControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="RichMessageController.DeleteRichMessageAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class DeleteRichMessageAsyncTest : Base
    {
        [TestMethod]
        public async Task Should_Remove_Message_And_Return_OK_Response()
        {
            var messageId = 1ul;
            var richMessageConfig = new RichMessageConfig
            {
                GuildId = 1ul,
                ChannelId = 1ul,
                RoleId = 1ul,
            };
            _configuration.RMConfig.Add(richMessageConfig);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var textChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var messageMock = new Mock<IMessage>(MockBehavior.Strict);

            _discordClientMock
                .Setup(pr => pr.GetGuildAsync(richMessageConfig.GuildId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildMock.Object);

            guildMock
               .Setup(pr => pr.GetTextChannelAsync(richMessageConfig.ChannelId, CacheMode.AllowDownload, null))
               .ReturnsAsync(textChannelMock.Object);

            textChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(messageMock.Object);

            messageMock
                .Setup(pr => pr.DeleteAsync(null))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteRichMessageAsync(messageId);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
