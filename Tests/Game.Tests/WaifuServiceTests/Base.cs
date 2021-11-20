using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
        protected readonly IWaifuService _waifuService;
        protected readonly Mock<IEventsService> _eventsServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<IImageProcessor> _imageProcessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IFileSystem> _fileSystemMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);
        protected readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        protected readonly Mock<IResourceManager> _resourceManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);

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
