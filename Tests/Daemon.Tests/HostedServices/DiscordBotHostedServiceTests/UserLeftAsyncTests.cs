using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.DiscordBotHostedServiceTests
{
    [TestClass]
    public class UserLeftAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Exit_Not_User_Message()
        {
            var messageMock = new Mock<IMessage>(MockBehavior.Strict);

            _discordSocketClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, messageMock.Object);
        }

    }
}
