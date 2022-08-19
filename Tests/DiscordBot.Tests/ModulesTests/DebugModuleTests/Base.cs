using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Game.Services.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.TaskQueue;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule"/> module.
    /// </summary>
    [TestClass]
    public class Base : TestBase
    {
        protected readonly DebugModule _module;
        protected readonly Mock<IApplicationLifetime> _applicationLifetimeMock = new(MockBehavior.Strict);
        protected readonly Mock<IEventIdsImporter> _eventIdsImporterMock = new(MockBehavior.Strict);
        protected readonly Mock<IFileSystem> _fileSystemMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClientAccessor> _discordClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);
        protected readonly Mock<IBlockingPriorityQueue> _blockingPriorityQueueMock = new(MockBehavior.Strict);
        protected readonly Mock<IWaifuService> _waifuServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<IHelperService> _helperServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<IImageProcessor> _imageProcessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IWritableOptions<SanakanConfiguration>> _sanakanConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ICardRepository> _cardRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IQuestionRepository> _questionRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IResourceManager> _resourceManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_userRepositoryMock.Object);
            serviceCollection.AddSingleton(_cardRepositoryMock.Object);
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
            serviceCollection.AddSingleton(_questionRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _sanakanConfigurationMock
                .Setup(pr => pr.Value)
                .Returns(new SanakanConfiguration
                {
                    Discord = new DiscordConfiguration(),
                });

            _module = new(
                _applicationLifetimeMock.Object,
                new DefaultIconConfiguration(),
                _eventIdsImporterMock.Object,
                _fileSystemMock.Object,
                _discordClientAccessorMock.Object,
                _shindenClientMock.Object,
                _blockingPriorityQueueMock.Object,
                _waifuServiceMock.Object,
                _helperServiceMock.Object,
                _imageProcessorMock.Object,
                _sanakanConfigurationMock.Object,
                _systemClockMock.Object,
                _cacheManagerMock.Object,
                _resourceManagerMock.Object,
                _randomNumberGeneratorMock.Object,
                _taskManagerMock.Object,
                serviceScopeFactory);
            Initialize(_module);
        }
    }
}
