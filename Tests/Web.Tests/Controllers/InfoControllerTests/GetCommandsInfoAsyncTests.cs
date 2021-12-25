using Discord.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Tests.Shared;
using Sanakan.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.InfoControllerTests
{
    [TestClass]
    public class GetCommandsInfoAsyncTests : Base
    {
        [TestMethod]
        public void Should_Return_Module_Info()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var commandService = new CommandService();
            var moduleBuilder = DiscordInternalExtensions.CreateModuleBuilder(commandService, null);

            var moduleName = "test module";
            moduleBuilder.WithName(moduleName);
            var commandName = "test command";
            moduleBuilder.AddAliases("test1");
            moduleBuilder.AddCommand(commandName, (cc, objs, sp, ci) => Task.CompletedTask, cb =>
            {
                cb.AddParameter("param", typeof(string), (pb) =>
                {
                    pb.Summary = "Parameter";
                });
                cb.AddAliases("test command1");
            });

            var moduleInfo = DiscordInternalExtensions.CreateModuleInfo(moduleBuilder, commandService, serviceProvider);

            var moduleInfos = new[]
            {
                moduleInfo,
            };

            _helperServiceMock
                .Setup(pr => pr.GetPublicModules())
                .Returns(moduleInfos);

            var result = _controller.GetCommandsInfo();
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var command = (Commands)okObjectResult.Value;
            command.Should().NotBeNull();
            command.Prefix.Should().Be(_configuration.Prefix);
            var module = command.Modules.First();
            module.Name.Should().Be(moduleName);
            module.SubModules.First().Commands.First().Name.Should().Be(commandName);
        }
    }
}
