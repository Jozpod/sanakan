using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Session.Abstractions;
using Sanakan.Game.Services.Abstractions;
using Sanakan.ShindenApi;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    [TestClass]
    public abstract class Base : TestBase
    {
        protected readonly PocketWaifuModule _module;
        protected readonly Mock<IOptions<GameConfiguration>> _gameConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IWaifuService> _waifuServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);
        protected readonly Mock<ISessionManager> _sessionManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        protected readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IGameDeckRepository> _gameDeckRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ICardRepository> _cardRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
            serviceCollection.AddSingleton(_gameDeckRepositoryMock.Object);
            serviceCollection.AddSingleton(_userRepositoryMock.Object);
            serviceCollection.AddSingleton(_cardRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _gameConfigurationMock
                .Setup(pr => pr.Value)
                .Returns(new GameConfiguration
                {
                    MinPlayersForPVP = 1,
                    PVPRankMultiplier = 0.45,
                    MinDeckPower = 200,
                    MaxDeckPower = 800,
                });

            _module = new(
                new DefaultIconConfiguration(),
                _gameConfigurationMock.Object,
                _waifuServiceMock.Object,
                _shindenClientMock.Object,
                NullLogger<PocketWaifuModule>.Instance,
                _sessionManagerMock.Object,
                _cacheManagerMock.Object,
                _randomNumberGeneratorMock.Object,
                _systemClockMock.Object,
                _taskManagerMock.Object,
                serviceScopeFactory);
            Initialize(_module);
        }
    }
}
