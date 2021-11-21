using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.DiscordBotHostedServiceTests
{
    [TestClass]
    public class DisconnectedAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Try_Reconnect()
        {
            await StartAsync();
            var exception = new Exception("test");

            _taskManagerMock
                .Setup(pr => pr.Delay(
                    It.IsAny<TimeSpan>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _discordClientMock
                .Setup(pr => pr.ConnectionState)
                .Returns(ConnectionState.Disconnected)
                .Verifiable();

            _discordClientMock
                .Setup(pr => pr.StartAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();

            _discordSocketClientAccessorMock.Raise(pr => pr.Disconnected += null, exception);

            _taskManagerMock.Verify();
            _discordClientMock.Verify();
        }

    }
}
