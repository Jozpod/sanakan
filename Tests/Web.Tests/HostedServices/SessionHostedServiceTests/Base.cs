using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Session;
using Sanakan.Web.HostedService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.HostedServices.SessionHostedServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly BackgroundService _service;
        protected readonly Mock<IOptionsMonitor<DaemonsConfiguration>> _daemonsConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClientAccessor> _discordClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClient> _discordClientMock = new(MockBehavior.Strict);
        protected readonly Mock<ISessionManager> _sessionManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<ITimer> _timerMock = new(MockBehavior.Strict);

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _discordClientAccessorMock
                .Setup(pr => pr.Client)
                .Returns(_discordClientMock.Object);

            _daemonsConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new DaemonsConfiguration());

            _taskManagerMock
                .Setup(pr => pr.Delay(
                    It.IsAny<TimeSpan>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _timerMock
                .Setup(pr => pr.Start(
                    It.IsAny<TimeSpan>(),
                    It.IsAny<TimeSpan>()));

            _service = new SessionHostedService(
                NullLogger<SessionHostedService>.Instance,
                _daemonsConfigurationMock.Object,
                _systemClockMock.Object,
                _discordClientAccessorMock.Object,
                serviceScopeFactory,
                _sessionManagerMock.Object,
                _taskManagerMock.Object,
                _timerMock.Object);
        }
    }
}
