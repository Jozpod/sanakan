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
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Supervisor;
using Sanakan.Daemon.HostedService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.SupervisorHostedServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly BackgroundService _service;
        protected readonly Mock<IDiscordClientAccessor> _discordClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<DaemonsConfiguration>> _daemonsConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserMessageSupervisor> _userMessageSupervisorMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserJoinedGuildSupervisor> _userJoinedGuildSupervisorMock = new(MockBehavior.Strict);
        protected readonly Mock<ITimer> _timerMock = new(MockBehavior.Strict);

        protected readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IModeratorService> _moderatorServiceMock = new(MockBehavior.Strict);
        
        public Base()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
            serviceCollection.AddSingleton(_moderatorServiceMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _daemonsConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new DaemonsConfiguration());

            _discordConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new DiscordConfiguration
                {
                    FloodSpamSupervisionEnabled = true,
                });

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

            _service = new SupervisorHostedService(
                NullLogger<SupervisorHostedService>.Instance,
                _discordClientAccessorMock.Object,
                _daemonsConfigurationMock.Object,
                _discordConfigurationMock.Object,
                serviceScopeFactory,
                _taskManagerMock.Object,
                _timerMock.Object,
                _userMessageSupervisorMock.Object,
                _userJoinedGuildSupervisorMock.Object);
        }
    }
}
