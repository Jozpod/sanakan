using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Modules;
using Sanakan.ShindenApi;
using Sanakan.Common;
using Sanakan.DiscordBot.Session;
using Sanakan.Common.Cache;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Game.Services.Abstractions;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
    [TestClass]
    public abstract class Base : TestBase
    {
        protected readonly ShindenModule _module;
        protected readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);
        protected readonly Mock<ISessionManager> _sessionManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IImageProcessor> _imageProcessorMock = new(MockBehavior.Strict);

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_userRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _module = new(
                _shindenClientMock.Object,
                _sessionManagerMock.Object,
                _cacheManagerMock.Object,
                _systemClockMock.Object,
                _taskManagerMock.Object,
                _imageProcessorMock.Object,
                serviceScopeFactory);
            Initialize(_module);
        }
    }
}
