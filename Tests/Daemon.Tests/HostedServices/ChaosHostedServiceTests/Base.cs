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
using Sanakan.Daemon.HostedService;

namespace Sanakan.Daemon.Tests.HostedServices.ChaosHostedServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly BackgroundService _service;
        protected readonly Mock<IOptionsMonitor<DaemonsConfiguration>> _daemonsConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClientAccessor> _discordSocketClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        protected readonly FakeTimer _fakeTimer = new();
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);
        protected readonly DiscordConfiguration _discordConfiguration;

        protected readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _discordConfiguration = new DiscordConfiguration();

            _discordConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(_discordConfiguration);

            _service = new ChaosHostedService(
                NullLogger<ChaosHostedService>.Instance,
                _discordConfigurationMock.Object,
                _daemonsConfigurationMock.Object,
                _discordSocketClientAccessorMock.Object,
                serviceScopeFactory,
                _randomNumberGeneratorMock.Object,
                _fakeTimer,
                _taskManagerMock.Object);
        }
    }
}
