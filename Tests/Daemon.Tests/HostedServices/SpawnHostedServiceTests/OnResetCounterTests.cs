using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Daemon.HostedService;
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

namespace Sanakan.Daemon.Tests.HostedServices.SpawnHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="SpawnHostedService.OnResetCounter(object, TimerEventArgs)"/> event handler.
    /// </summary>
    [TestClass]
    public class OnResetCounterTests : Base
    {
        [TestMethod]
        public async Task Should_Reset_Entries()
        {
            var utcNow = DateTime.UtcNow;
            
            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow)
                .Verifiable();

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _timerMock.Raise(pr => pr.Tick += null, null, new TimerEventArgs(null));

            _systemClockMock.Verify();
        }

    }
}
