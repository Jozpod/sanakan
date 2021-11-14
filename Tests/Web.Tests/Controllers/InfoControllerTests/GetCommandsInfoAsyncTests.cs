using Discord.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.Web.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.InfoControllerTests
{
    [TestClass]
    public class GetCommandsInfoAsyncTests : Base
    {
        public ModuleInfo CreateModuleInfo()
        {
            return null;
        }

        [TestMethod]
        public async Task Should_Return_Module_Info()
        {
            var moduleInfos = new[]
            {
                CreateModuleInfo()
            };

            _helperServiceMock
                .Setup(pr => pr.GetPublicModules())
                .Returns(moduleInfos);

            var result = await _controller.GetCommandsInfoAsync();
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
