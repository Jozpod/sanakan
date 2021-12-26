using Discord.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Tests.Shared;
using System.Threading.Tasks;

namespace DiscordBot.ServicesTests.HelperServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IHelperService.GivePrivateHelp(string)"/> method.
    /// </summary>
    [TestClass]
    public class GivePrivateHelpTests : Base
    {
        [TestMethod]
        public void Should_Return_Info()
        {
            var moduleName = "private-module";
            var moduleInfo = DiscordInternalExtensions.CreateModuleWithCommand(moduleName, "command");

            _helperService.AddPrivateModuleInfo((moduleName, moduleInfo));
            var result = _helperService.GivePrivateHelp(moduleName);
            result.Should().NotBeNullOrEmpty();
        }
    }
}
