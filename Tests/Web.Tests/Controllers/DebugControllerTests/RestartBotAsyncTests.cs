using Discord.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.DebugControllerTests
{
    [TestClass]
    public class RestartBotAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Restart_Bot()
        {
            _discordClientAccessorMock
                .Setup(pr => pr.LogoutAsync())
                .Returns(Task.CompletedTask);


            var result = await _controller.RestartBotAsync();
            result.Should().BeOfType<OkResult>();
        }
    }
}
