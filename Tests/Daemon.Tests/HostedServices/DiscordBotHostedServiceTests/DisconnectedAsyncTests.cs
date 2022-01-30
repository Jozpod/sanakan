using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Daemon.HostedService;
using System;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.DiscordBotHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="DiscordBotHostedService.DisconnectedAsync"/> event handler.
    /// </summary>
    [TestClass]
    public class DisconnectedAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Try_Reconnect()
        {
            await StartAsync();
            var exception = new Exception("test");

            _taskManagerMock
                .Setup(pr => pr.Delay(It.IsAny<TimeSpan>()))
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

            _discordClientAccessorMock.Raise(pr => pr.Disconnected += null, exception);

            _taskManagerMock.Verify();
            _discordClientMock.Verify();
        }

    }
}
