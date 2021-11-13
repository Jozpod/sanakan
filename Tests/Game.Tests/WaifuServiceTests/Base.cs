using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Services.PocketWaifu;
using Sanakan.Game.Services;
using Sanakan.Common;
using Sanakan.ShindenApi;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Game.Services.Abstractions;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    [TestClass]
    public abstract class Base
    {
        private readonly IWaifuService _waifuService;
        private readonly Mock<IEventsService> _eventsServiceMock = new(MockBehavior.Strict);
        private readonly Mock<IImageProcessor> _imageProcessorMock = new(MockBehavior.Strict);
        private readonly Mock<IFileSystem> _fileSystemMock = new(MockBehavior.Strict);
        private readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        private readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);
        private readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        private readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        private readonly Mock<IResourceManager> _resourceManagerMock = new(MockBehavior.Strict);
        private readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _waifuService = new WaifuService(
                _eventsServiceMock.Object,
                _imageProcessorMock.Object,
                _fileSystemMock.Object,
                _systemClockMock.Object,
                _shindenClientMock.Object,
                _cacheManagerMock.Object,
                _randomNumberGeneratorMock.Object,
                _resourceManagerMock.Object,
                _taskManagerMock.Object,
                serviceScopeFactory);
        }
    }
}
