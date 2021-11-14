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
    public class PostRichMessageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Create_Rich_Message()
        {
            var payload = new RichMessage
            {

            };
            var mention = true;
            var result = await _controller.PostRichMessageAsync(payload, mention);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
