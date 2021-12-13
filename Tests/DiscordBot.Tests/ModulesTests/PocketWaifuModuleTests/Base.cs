using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Modules;
using Sanakan.Common;
using Sanakan.ShindenApi;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Sanakan.Game.Services.Abstractions;
using Sanakan.DiscordBot.Session;
using Sanakan.Common.Cache;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.DiscordBot.Abstractions.Configuration;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    [TestClass]
    public abstract class Base : TestBase
    {
        protected readonly PocketWaifuModule _module;
        protected readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
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

            _module = new(
                new DefaultIconConfiguration(),
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
