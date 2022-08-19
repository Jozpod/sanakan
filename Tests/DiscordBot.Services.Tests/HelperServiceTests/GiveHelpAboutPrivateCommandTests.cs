using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot.Services.Abstractions;

namespace DiscordBot.ServicesTests.HelperServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IHelperService.GiveHelpAboutPrivateCommand(string, string, string, bool)"/> method.
    /// </summary>
    [TestClass]
    public class GiveHelpAboutPrivateCommandTests : Base
    {
        [TestMethod]
        public void Should_Return_Private_Command_Info()
        {
            var module = "module";
            var command = "command";
            var prefix = ".";

            var result = _helperService.GiveHelpAboutPrivateCommand(module, command, prefix);
            result.Should().BeEmpty();
        }
    }
}
