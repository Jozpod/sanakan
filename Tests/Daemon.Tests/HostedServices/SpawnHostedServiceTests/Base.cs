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
using Sanakan.Game.Services.Abstractions;
using Sanakan.TaskQueue;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.SpawnHostedServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly BackgroundService _service;
        protected readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<ExperienceConfiguration>> _experienceConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClientAccessor> _discordClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClient> _discordClientMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IBlockingPriorityQueue> _blockingPriorityQueueMock = new(MockBehavior.Strict);
        protected readonly Mock<IWaifuService> _waifuServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<ITimer> _timerMock = new(MockBehavior.Strict);
        protected readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
            serviceCollection.AddSingleton(_userRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _experienceConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new ExperienceConfiguration
                {
                    CharPerPacket = 5,
                });

            _discordConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new DiscordConfiguration
                {
                    SafariEnabled = true,
                });

            _discordClientAccessorMock
                .Setup(pr => pr.Client)
                .Returns(_discordClientMock.Object);

            _taskManagerMock
                .Setup(pr => pr.Delay(
                    It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

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

            _service = new SpawnHostedService(
                _randomNumberGeneratorMock.Object,
                _blockingPriorityQueueMock.Object,
                NullLogger<SpawnHostedService>.Instance,
                _discordConfigurationMock.Object,
                _experienceConfigurationMock.Object,
                _discordClientAccessorMock.Object,
                _systemClockMock.Object,
                serviceScopeFactory,
                _taskManagerMock.Object,
                _waifuServiceMock.Object,
                _timerMock.Object);
        }
    }
}
