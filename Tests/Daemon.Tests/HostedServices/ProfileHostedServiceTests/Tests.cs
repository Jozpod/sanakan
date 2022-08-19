using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Daemon.HostedService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.ProfileHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileHostedService.OnTick(object, Common.TimerEventArgs)"/> event handler.
    /// </summary>
    [TestClass]
    public class Tests : Base
    {
        [TestMethod]
        public async Task Should_Stop_Timer_On_Logout()
        {
            _timerMock
                .Setup(pr => pr.Stop());

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _discordClientAccessorMock.Raise(pr => pr.LoggedOut += null);
        }

        [TestMethod]
        public async Task Should_Start_Timer_When_Connected_To_Discord()
        {
            _timerMock
                .Setup(pr => pr.Start(
                    It.IsAny<TimeSpan>(),
                    It.IsAny<TimeSpan>()));

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _discordClientAccessorMock.Raise(pr => pr.Ready += null);
        }
    }
}
