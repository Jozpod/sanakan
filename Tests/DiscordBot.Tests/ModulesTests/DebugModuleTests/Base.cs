using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Modules;
using DiscordBot.Services.PocketWaifu.Abstractions;
using Sanakan.TaskQueue;
using Sanakan.ShindenApi;
using Sanakan.Game.Services;
using Sanakan.Common;
using Sanakan.Common.Configuration;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly DebugModule _module;
        protected readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);
        protected readonly Mock<IBlockingPriorityQueue> _blockingPriorityQueueMock = new(MockBehavior.Strict);
        protected readonly Mock<IWaifuService> _waifuServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<IHelperService> _helperServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<IImageProcessor> _imageProcessorMock = new(MockBehavior.Strict);
        protected readonly Mock<WritableOptions<SanakanConfiguration>> _sanakanConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ICardRepository> _cardRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IGuildConfigRepository> _guildConfigRepository = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IResourceManager> _resourceManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);

        public Base()
        {
            _module = new(
                _shindenClientMock.Object,
                _blockingPriorityQueueMock.Object,
                _waifuServiceMock.Object,
                _helperServiceMock.Object,
                _imageProcessorMock.Object,
                _sanakanConfigurationMock.Object,
                _userRepositoryMock.Object,
                _cardRepositoryMock.Object,
                _guildConfigRepository.Object,
                _systemClockMock.Object,
                _resourceManagerMock.Object,
                _randomNumberGeneratorMock.Object,
                _taskManagerMock.Object);
        }
    }
}
