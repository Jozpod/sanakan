using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using System;

namespace DiscordBot.ServicesTests.ModeratorServiceTests
{
    [TestClass]
    public class NotifyUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Notify_User()
        {
            var userMock = new Mock<IUser>();
            var duration = TimeSpan.FromMinutes(1);
            var reason = "reason";

            await _moderatorService.NotifyUserAsync(userMock.Object, reason);
        }
    }
}
