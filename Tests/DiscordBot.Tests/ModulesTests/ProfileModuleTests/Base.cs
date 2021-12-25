using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Session.Abstractions;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    [TestClass]
    public abstract class Base : TestBase
    {
        protected readonly ProfileModule _module;
        protected readonly Mock<IProfileService> _profileServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<ISessionManager> _sessionManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IGameDeckRepository> _gameDeckRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
            serviceCollection.AddSingleton(_gameDeckRepositoryMock.Object);
            serviceCollection.AddSingleton(_userRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _module = new(
                new DefaultIconConfiguration(),
                _profileServiceMock.Object,
                _sessionManagerMock.Object,
                _cacheManagerMock.Object,
                _systemClockMock.Object,
                serviceScopeFactory);
            Initialize(_module);
        }
    }
}
