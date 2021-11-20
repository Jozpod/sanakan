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
        public ModuleInfo CreateModuleInfo()
        {
            return null;
        }

        [TestMethod]
        public async Task Should_Return_Module_Info()
        {
            
            var result = await _controller.RestartBotAsync();
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
