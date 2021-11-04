using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Services.PocketWaifu;
using Sanakan.Game.Services;
using Sanakan.Common;
using Sanakan.ShindenApi;

namespace DiscordBot.ServicesTests.WaifuServiceTests
{
    [TestClass]
    public abstract class Base
    {
        private readonly WaifuService _waifuService;
        private readonly Mock<IImageProcessor> _imageProcessorMock = new(MockBehavior.Strict);
        private readonly Mock<IFileSystem> _fileSystemMock = new(MockBehavior.Strict);
        private readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        private readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);
        private readonly Mock<EventsService> _eventsServiceMock = new(MockBehavior.Strict);
        private readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        private readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        private readonly Mock<IResourceManager> _resourceManagerMock = new(MockBehavior.Strict);
        private readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);

        public Base()
        {
            _waifuService = new(
                _imageProcessorMock.Object,
                _fileSystemMock.Object,
                _systemClockMock.Object,
                _shindenClientMock.Object,
                _eventsServiceMock.Object,
                _cacheManagerMock.Object,
                _randomNumberGeneratorMock.Object,
                _resourceManagerMock.Object,
                _userRepositoryMock.Object);
        }
    }
}
