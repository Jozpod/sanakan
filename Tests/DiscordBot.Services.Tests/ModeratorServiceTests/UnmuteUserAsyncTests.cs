using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace DiscordBot.ServicesTests.ModeratorServiceTests
{
    [TestClass]
    public class UnmuteUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Success()
        {
            var userMock = new Mock<IGuildUser>();
            var muteRoleMock = new Mock<IRole>();
            var muteModRoleMock = new Mock<IRole>();

            await _moderatorService.UnmuteUserAsync(
                userMock.Object,
                muteRoleMock.Object,
                muteModRoleMock.Object);
        }
    }
}
