using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot.Models;
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
