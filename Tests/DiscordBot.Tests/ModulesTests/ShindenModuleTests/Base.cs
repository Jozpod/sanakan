using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Modules;
using Sanakan.TaskQueue;
using Sanakan.ShindenApi;
using Sanakan.Common;

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

        public Base()
        {
            _module = new(
                _shindenClientMock.Object,
                _sessionManagerMock.Object,
                _cacheManagerMock.Object,
                _userRepositoryMock.Object,
                _systemClockMock.Object,
                _taskManagerMock.Object);
            Initialize(_module);
        }
    }
}
