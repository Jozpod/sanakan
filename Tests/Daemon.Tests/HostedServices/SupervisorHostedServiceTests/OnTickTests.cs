using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common;
using Sanakan.Daemon.HostedService;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.SupervisorHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="SupervisorHostedService.OnTick(object, TimerEventArgs)"/> event handler.
    /// </summary>
    [TestClass]
    public class OnTickTests : Base
    {
        [TestMethod]
        public async Task Should_Reset_Supervisors()
        {
            _userJoinedGuildSupervisorMock
                .Setup(pr => pr.Refresh())
                .Verifiable();

            _userMessageSupervisorMock
                .Setup(pr => pr.Refresh())
                .Verifiable();

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _timerMock.Raise(pr => pr.Tick += null, null, new TimerEventArgs(null));

            _userJoinedGuildSupervisorMock.Verify();
            _userMessageSupervisorMock.Verify();
        }

    }
}
