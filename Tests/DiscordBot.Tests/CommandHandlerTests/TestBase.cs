using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Session.Abstractions;
using Sanakan.Game.Services.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.TaskQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sanakan.DiscordBot.Tests.CommandHandlerTests
{
    public class TestBase
    {
        protected readonly ICommandHandler _commandHandler;
        protected readonly Mock<IDiscordClientAccessor> _discordClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IBlockingPriorityQueue> _blockingPriorityQueueMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClient> _discordClientMock = new(MockBehavior.Strict);
        protected readonly Mock<IHelperService> _helperServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<ICommandService> _commandServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IResourceManager> _resourceManagerMock = new(MockBehavior.Strict);

        protected readonly Mock<IModeratorService> _moderatorServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<ISessionManager> _sessionManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ICommandsAnalyticsRepository> _commandsAnalyticsRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IQuestionRepository> _guestionRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IGameDeckRepository> _gameDeckRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ICardRepository> _cardRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IProfileService> _profileServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IOperatingSystem> _operatingSystemMock = new(MockBehavior.Strict);
        protected readonly Mock<ILandManager> _landManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IWaifuService> _waifuServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<IImageProcessor> _imageProcessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IWritableOptions<SanakanConfiguration>> _sanakanConfigurationWritableMock = new(MockBehavior.Strict);
        protected readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);
        protected readonly DiscordConfiguration _configuration;

        public TestBase()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_moderatorServiceMock.Object);
            serviceCollection.AddSingleton(_sessionManagerMock.Object);
            serviceCollection.AddSingleton(_cacheManagerMock.Object);
            serviceCollection.AddSingleton(_userRepositoryMock.Object);
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
            serviceCollection.AddSingleton(_commandsAnalyticsRepositoryMock.Object);
            serviceCollection.AddSingleton(_guestionRepositoryMock.Object);
            serviceCollection.AddSingleton(_systemClockMock.Object);
            serviceCollection.AddSingleton(_operatingSystemMock.Object);
            serviceCollection.AddSingleton(_helperServiceMock.Object);
            serviceCollection.AddSingleton(_randomNumberGeneratorMock.Object);
            serviceCollection.AddSingleton(_taskManagerMock.Object);
            serviceCollection.AddSingleton(_landManagerMock.Object);
            serviceCollection.AddSingleton(_waifuServiceMock.Object);
            serviceCollection.AddSingleton(_shindenClientMock.Object);
            serviceCollection.AddSingleton(_gameDeckRepositoryMock.Object);
            serviceCollection.AddSingleton(_cardRepositoryMock.Object);
            serviceCollection.AddSingleton(_profileServiceMock.Object);
            serviceCollection.AddSingleton(_imageProcessorMock.Object);
            serviceCollection.AddSingleton(_resourceManagerMock.Object);
            serviceCollection.AddSingleton(_sanakanConfigurationWritableMock.Object);
            serviceCollection.AddSingleton(_profileServiceMock.Object);
            serviceCollection.AddSingleton(_blockingPriorityQueueMock.Object);
            serviceCollection.AddSingleton(_discordConfigurationMock.Object);
            serviceCollection.AddSingleton<ILogger<PocketWaifuModule>>(NullLogger<PocketWaifuModule>.Instance);
            serviceCollection.AddSingleton<ILogger<HelperModule>>(NullLogger<HelperModule>.Instance);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _configuration = new DiscordConfiguration
            {
                Prefix = ".",
            };

            _discordConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(_configuration);

            _discordClientAccessorMock
                .Setup(pr => pr.Client)
                .Returns(_discordClientMock.Object);

            _commandServiceMock
                .Setup(pr => pr.AddTypeReader<It.IsAnyType>(It.IsAny<TypeReader>()));

            _commandServiceMock
                .Setup(pr => pr.AddModulesAsync(It.IsAny<Assembly>(), It.IsAny<IServiceProvider>()))
                .ReturnsAsync(Enumerable.Empty<ModuleInfo>());

            ModuleInfo moduleInfo = null!;

            _commandServiceMock
                .Setup(pr => pr.AddModuleAsync<It.IsAnyType>(It.IsAny<IServiceProvider>()))
                .ReturnsAsync(moduleInfo);

            _helperServiceMock
                .Setup(pr => pr.AddPublicModuleInfo(It.IsAny<IEnumerable<ModuleInfo>>()));

            _helperServiceMock
                .Setup(pr => pr.AddPrivateModuleInfo(It.IsAny<(string, ModuleInfo)[]>()));

            _commandHandler = new CommandHandler(
                _discordClientAccessorMock.Object,
                _blockingPriorityQueueMock.Object,
                _helperServiceMock.Object,
                _commandServiceMock.Object,
                _discordConfigurationMock.Object,
                NullLogger<CommandHandler>.Instance,
                _systemClockMock.Object,
                _resourceManagerMock.Object,
                serviceProvider,
                serviceScopeFactory);
        }
    }
}
