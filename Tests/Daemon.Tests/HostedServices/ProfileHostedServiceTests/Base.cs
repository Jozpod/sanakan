using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Daemon.HostedService;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.ProfileHostedServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly BackgroundService _service;
        protected readonly Mock<IOptionsMonitor<DaemonsConfiguration>> _daemonsConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClientAccessor> _discordClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClient> _discordClientMock = new(MockBehavior.Strict);
        protected readonly Mock<ITimer> _timerMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);

        protected readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ITimeStatusRepository> _timeStatusRepositoryMock = new(MockBehavior.Strict);

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
            serviceCollection.AddSingleton(_timeStatusRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _daemonsConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new DaemonsConfiguration());

            _taskManagerMock
                .Setup(pr => pr.Delay(
                    It.IsAny<TimeSpan>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _discordClientAccessorMock
                .Setup(pr => pr.Client)
                .Returns(_discordClientMock.Object);

            _timerMock
                .Setup(pr => pr.Start(
                    It.IsAny<TimeSpan>(),
                    It.IsAny<TimeSpan>()));

            _service = new ProfileHostedService(
                NullLogger<ProfileHostedService>.Instance,
                _systemClockMock.Object,
                _discordClientAccessorMock.Object,
                _daemonsConfigurationMock.Object,
                serviceScopeFactory,
                _timerMock.Object,
                _taskManagerMock.Object);
        }
    }
}
