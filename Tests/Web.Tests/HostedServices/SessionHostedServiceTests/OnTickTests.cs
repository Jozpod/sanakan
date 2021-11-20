using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Web.HostedService;
using System.Threading.Tasks;
using System.Reflection;
using Discord.WebSocket;
using Sanakan.Tests.Shared;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Session;
using System;
using Discord.Commands;
using System.Threading;
using Sanakan.Common;

namespace Sanakan.Web.Tests.HostedServices.SessionHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="SessionHostedService.OnTick(object, Common.TimerEventArgs)"/> event handler.
    /// </summary>

    [TestClass]
    public class OnTickTests : Base
    {
        [TestMethod]
        public async Task Should_Remove_Session()
        {
            var sessionMock = new Mock<IInteractionSession>(MockBehavior.Strict);
            var sessions = new[]
            {
                sessionMock.Object
            };
            var utcNow = DateTime.UtcNow;

            _sessionManagerMock
                .Setup(pr => pr.GetExpired(utcNow))
                .Returns(sessions)
                .Verifiable();

            _sessionManagerMock
                .Setup(pr => pr.Remove(sessionMock.Object))
                .Verifiable();

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            sessionMock
                .Setup(pr => pr.IsRunning)
                .Returns(false)
                .Verifiable();

            sessionMock
                .Setup(pr => pr.DisposeAsync())
                .Returns(ValueTask.CompletedTask)
                .Verifiable();

            _timerMock
                .Setup(pr => pr.Start(
                    It.IsAny<TimeSpan>(),
                    It.IsAny<TimeSpan>()));

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _timerMock.Raise(pr => pr.Tick += null, null, new TimerEventArgs(null));

            _sessionManagerMock.Verify();
            sessionMock.Verify();
        }

    }
}