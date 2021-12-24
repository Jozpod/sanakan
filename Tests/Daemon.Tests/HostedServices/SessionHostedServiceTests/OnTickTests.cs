using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Daemon.HostedService;
using Sanakan.DiscordBot.Session;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.SessionHostedServiceTests
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
               .SetupSet(pr => pr.ServiceProvider = It.IsAny<IServiceProvider>());

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