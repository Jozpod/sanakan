using Discord.WebSocket;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Models;
using Sanakan.ShindenApi;
using Sanakan.Web.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.RichMessageControllerTests
{
    [TestClass]
    public class ModifyRichMessageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Modify_Message()
        {
            var messageId = 1ul;
            var payload = new RichMessage
            {
                
            };
            var discordSocketClientMock = new Mock<DiscordSocketClient>();

            _discordSocketClientAccessorMock
                .Setup(pr => pr.Client)
                .Returns(discordSocketClientMock.Object);

            var result = await _controller.ModifyRichMessageAsync(messageId, payload);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
