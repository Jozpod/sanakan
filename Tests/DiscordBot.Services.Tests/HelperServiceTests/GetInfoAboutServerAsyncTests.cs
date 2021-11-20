using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using FluentAssertions;

namespace DiscordBot.ServicesTests.HelperServiceTests
{
    [TestClass]
    public class GetInfoAboutServerAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Success()
        {
            var guildMock = new Mock<IGuild>();

            var result = await _helperService.GetInfoAboutServerAsync(guildMock.Object);
            result.Should().NotBeNull();
        }
    }
}
