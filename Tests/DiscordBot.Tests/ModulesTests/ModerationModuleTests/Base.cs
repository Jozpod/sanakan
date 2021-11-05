using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Modules;
using Sanakan.ShindenApi;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using Sanakan.Common;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly ModerationModule _module;
        protected readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IHelperService> _helperServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<IProfileService> _profileServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<IModeratorService> _moderatorServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);
        protected readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);

        public Base()
        {
            _module = new(
                _discordConfigurationMock.Object,
                _helperServiceMock.Object,
                _profileServiceMock.Object,
                _moderatorServiceMock.Object,
                _shindenClientMock.Object,
                _cacheManagerMock.Object,
                _systemClockMock.Object,
                _userRepositoryMock.Object,
                _guildConfigRepositoryMock.Object,
                _randomNumberGeneratorMock.Object,
                _taskManagerMock.Object);
        }
    }
}
