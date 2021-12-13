using Discord.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Sanakan.Web.Tests.Controllers.InfoControllerTests
{
    [TestClass]
    public class GetCommandsInfoAsyncTests : Base
    {
        [TestMethod]
        public void Should_Return_Module_Info()
        {
            var moduleInfos = Enumerable.Empty<ModuleInfo>();

            _helperServiceMock
                .Setup(pr => pr.GetPublicModules())
                .Returns(moduleInfos);

            var result = _controller.GetCommandsInfoAsync();
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
