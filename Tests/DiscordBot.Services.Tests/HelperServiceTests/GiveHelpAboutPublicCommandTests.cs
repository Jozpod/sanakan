using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using FluentAssertions;

namespace DiscordBot.ServicesTests.HelperServiceTests
{
    [TestClass]
    public class GiveHelpAboutPublicCommandTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Success()
        {
            var command = "command";
            var prefix = ".";

            var result = _helperService.GiveHelpAboutPublicCommand(command, prefix);
            result.Should().NotBeEmpty();
        }
    }
}
