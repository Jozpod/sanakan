﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot.Models;
using System.Threading.Tasks;
using Sanakan.Web.Controllers;
using Discord;
using Moq;
using Sanakan.Configuration;

namespace Sanakan.Web.Tests.Controllers.RichMessageControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="RichMessageController.PostRichMessageAsync(RichMessage, bool?)"/> method.
    /// </summary>
    [TestClass]
    public class PostRichMessageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Create_Rich_Message_And_Return_OK()
        {
            var payload = new RichMessage
            {

            };
            var mention = true;
            var richMessageConfig = new RichMessageConfig
            {
                GuildId = 1ul,
                ChannelId = 1ul,
                RoleId = 1ul,
            };
            _configuration.RMConfig.Add(richMessageConfig);
            var messageId = 1ul;
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var textChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);

            _discordClientMock
                .Setup(pr => pr.GetGuildAsync(richMessageConfig.GuildId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildMock.Object);

            guildMock
                .Setup(pr => pr.GetTextChannelAsync(richMessageConfig.ChannelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(textChannelMock.Object);

            guildMock
                .Setup(pr => pr.GetRole(richMessageConfig.RoleId))
                .Returns(roleMock.Object);

            roleMock
                .Setup(pr => pr.Mention)
                .Returns("role mention");

            textChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(userMessageMock.Object);

            userMessageMock
                .Setup(pr => pr.Id)
                .Returns(messageId);

            var result = await _controller.PostRichMessageAsync(payload, mention);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
