using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using FluentAssertions;

namespace DiscordBot.ServicesTests.HelperServiceTests
{
    [TestClass]
    public class GiveHelpAboutPrivateCommandTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Success()
        {
            var module = "module";
            var command = "command";
            var prefix = ".";

            var result = _helperService.GiveHelpAboutPrivateCommand(module, command, prefix);
            result.Should().NotBeEmpty();
        }
    }
}
