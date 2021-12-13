using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Models;
using System.Threading.Tasks;
using Sanakan.Web.Controllers;
using Discord;
using System;
using Sanakan.Configuration;

namespace Sanakan.Web.Tests.Controllers.RichMessageControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="RichMessageController.ModifyRichMessageAsync(ulong, RichMessage)"/> method.
    /// </summary>
    [TestClass]
    public class ModifyRichMessageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Modify_Message_And_Return_OK()
        {
            var messageId = 1ul;
            var richMessageConfig = new RichMessageConfig
            {
                GuildId = 1ul,
                ChannelId = 1ul,
                RoleId = 1ul,
            };
            var payload = new RichMessage
            {
                Content = "test",
            };
            _configuration.RMConfig.Add(richMessageConfig);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var textChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var messageMock = new Mock<IUserMessage>(MockBehavior.Strict);

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
                .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
                .Returns(Task.CompletedTask);

            var result = await _controller.ModifyRichMessageAsync(messageId, payload);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
