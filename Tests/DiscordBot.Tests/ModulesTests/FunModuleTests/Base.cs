using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using Sanakan.Common;
using Sanakan.DiscordBot.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.DiscordBot.Session;
using Sanakan.Game.Services;
using Sanakan.Common.Cache;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Game.Services.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    [TestClass]
    public abstract class Base : TestBase
    {
        protected readonly FunModule _module;
        protected readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IQuestionRepository> _questionRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IModeratorService> _moderatorServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<ISessionManager> _sessionManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        protected readonly Mock<ISlotMachine> _slotMachineMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
            serviceCollection.AddSingleton(_userRepositoryMock.Object);
            serviceCollection.AddSingleton(_questionRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _module = new(
                new DefaultIconConfiguration(),
                _sessionManagerMock.Object,
                _cacheManagerMock.Object,
                _systemClockMock.Object,
                _randomNumberGeneratorMock.Object,
                _slotMachineMock.Object,
                _taskManagerMock.Object,
                serviceScopeFactory);
            Initialize(_module);
        }
    }
}
