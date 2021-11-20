using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using Sanakan.Common;
using Sanakan.DiscordBot.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.DiscordBot.Session;
using Sanakan.Game.Services;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    [TestClass]
    public abstract class Base : TestBase
    {
        protected readonly FunModule _module;
        protected readonly Mock<IModeratorService> _moderatorServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<ISessionManager> _sessionManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<SlotMachine> _slotMachineMock = new(MockBehavior.Strict);

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _module = new(
                _moderatorServiceMock.Object,
                _sessionManagerMock.Object,
                _cacheManagerMock.Object,
                _systemClockMock.Object,
                serviceScopeFactory,
                _slotMachineMock.Object,
                _taskManagerMock.Object);
            Initialize(_module);
        }
    }
}
