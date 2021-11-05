using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Modules;
using Sanakan.Common;
using Sanakan.TaskQueue;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    [TestClass]
    public abstract class Base
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
            _module = new(
                _profileServiceMock.Object,
                _sessionManagerMock.Object,
                _cacheManagerMock.Object,
                _guildConfigRepositoryMock.Object,
                _gameDeckRepositoryMock.Object,
                _userRepositoryMock.Object,
                _systemClockMock.Object);
        }
    }
}
