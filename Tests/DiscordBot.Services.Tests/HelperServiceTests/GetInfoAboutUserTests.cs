using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using FluentAssertions;

namespace DiscordBot.ServicesTests.HelperServiceTests
{
    [TestClass]
    public class GetInfoAboutUserTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Success()
        {
            var guildUserMock = new Mock<IGuildUser>();
            var result = _helperService.GetInfoAboutUser(guildUserMock.Object);
            result.Should().NotBeNull();
        }
    }
}
