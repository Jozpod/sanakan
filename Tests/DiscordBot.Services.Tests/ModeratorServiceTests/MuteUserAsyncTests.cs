using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using System.Threading.Tasks;
using System;

namespace DiscordBot.ServicesTests.ModeratorServiceTests
{
    [TestClass]
    public class MuteUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Success()
        {
            var userMock = new Mock<IGuildUser>();
            var muteRoleMock = new Mock<IRole>();
            var muteModRoleMock = new Mock<IRole>();
            var userRoleMock = new Mock<IRole>();
            var duration = TimeSpan.FromMinutes(1);
            var reason = "reason";
            var modRoles = new[]
            {
                new ModeratorRoles()
            };

            await _moderatorService.MuteUserAsync(
                userMock.Object,
                muteRoleMock.Object,
                muteModRoleMock.Object,
                userRoleMock.Object,
                duration,
                reason,
                modRoles);
        }
    }
}
