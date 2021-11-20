using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.Web.HostedService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.HostedServices.ModeratorHostedServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly BackgroundService _service;
        protected readonly Mock<IOptionsMonitor<DaemonsConfiguration>> _daemonsConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClientAccessor> _discordSocketClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClient> _discordClientMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly FakeTimer _fakeTimer = new();
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);

        protected readonly Mock<IPenaltyInfoRepository> _penaltyInfoRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        
        public Base()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_penaltyInfoRepositoryMock.Object);
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _taskManagerMock
              .Setup(pr => pr.Delay(
                  It.IsAny<TimeSpan>(),
                  It.IsAny<CancellationToken>()))
              .Returns(Task.CompletedTask);

            _discordSocketClientAccessorMock
                .Setup(pr => pr.Client)
                .Returns(_discordClientMock.Object);

            _service = new ModeratorHostedService(
                NullLogger<ModeratorHostedService>.Instance,
                _systemClockMock.Object,
                _discordSocketClientAccessorMock.Object,
                _daemonsConfigurationMock.Object,
                serviceScopeFactory,
                _fakeTimer,
                _taskManagerMock.Object,
                _cacheManagerMock.Object);
        }
    }
}
