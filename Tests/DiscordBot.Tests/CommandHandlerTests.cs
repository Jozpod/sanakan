using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Session;
using Sanakan.Game.Services.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.TaskQueue;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests
{
    [TestClass]
    public class CommandHandlerTests
    {
        private readonly ICommandHandler _commandHandler;
        private readonly Mock<IDiscordClientAccessor> _discordSocketClientAccessorMock = new(MockBehavior.Strict);
        private readonly Mock<IDiscordClient> _discordClientMock = new(MockBehavior.Strict);
        private readonly Mock<IHelperService> _helperServiceMock = new(MockBehavior.Strict);
        private readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        private readonly Mock<CommandService> _commandServiceMock = new(MockBehavior.Strict);
        private readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);

        private readonly Mock<IModeratorService> _moderatorServiceMock = new(MockBehavior.Strict);
        private readonly Mock<ISessionManager> _sessionManagerMock = new(MockBehavior.Strict);
        private readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        private readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<IQuestionRepository> _guestionRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<IGameDeckRepository> _gameDeckRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<ICardRepository> _cardRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<IProfileService> _profileServiceMock = new(MockBehavior.Strict);
        private readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        private readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);
        private readonly Mock<IOperatingSystem> _operatingSystemMock = new(MockBehavior.Strict);
        private readonly Mock<ILandManager> _landManagerMock = new(MockBehavior.Strict);
        private readonly Mock<IWaifuService> _waifuServiceMock = new(MockBehavior.Strict);
        private readonly Mock<IImageProcessor> _imageProcessorMock = new(MockBehavior.Strict);
        private readonly Mock<IWritableOptions<SanakanConfiguration>> _sanakanConfigurationWritableMock = new(MockBehavior.Strict);
        private readonly Mock<IResourceManager> _resourceManagerMock = new(MockBehavior.Strict);
        private readonly Mock<IBlockingPriorityQueue> _blockingPriorityQueueMock = new(MockBehavior.Strict);
        private readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);

        public CommandHandlerTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_moderatorServiceMock.Object);
            serviceCollection.AddSingleton(_sessionManagerMock.Object);
            serviceCollection.AddSingleton(_cacheManagerMock.Object);
            serviceCollection.AddSingleton(_userRepositoryMock.Object);
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
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

            _discordSocketClientAccessorMock
                .Setup(pr => pr.Client)
                .Returns(_discordClientMock.Object);

            _helperServiceMock
                .Setup(pr => pr.AddPublicModuleInfo(It.IsAny<IEnumerable<ModuleInfo>>()));

            _helperServiceMock
                .Setup(pr => pr.AddPrivateModuleInfo(It.IsAny<(string, ModuleInfo)[]>()));

            _commandHandler = new CommandHandler(
                _discordSocketClientAccessorMock.Object,
                _helperServiceMock.Object,
                _commandServiceMock.Object,
                _discordConfigurationMock.Object,
                NullLogger<CommandHandler>.Instance,
                _systemClockMock.Object,
                serviceProvider,
                serviceScopeFactory);
        }

        [TestMethod]
        public async Task Should_Initialize_Correctly()
        {
            await _commandHandler.InitializeAsync();
        }

        [TestMethod]
        public async Task Should_Handle_Command_Not_User_Message()
        {
            var messageMock = new Mock<IMessage>(MockBehavior.Strict);

            await _commandHandler.InitializeAsync();
            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, messageMock.Object);
        }

        [TestMethod]
        public async Task Should_Handle_Command_User_Message_Bot()
        {
            var userMmessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);

            userMmessageMock
                .Setup(pr => pr.Author)
                .Returns(userMock.Object);

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(true);

            await _commandHandler.InitializeAsync();
            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMmessageMock.Object);
        }

        [TestMethod]
        public async Task Should_Handle_Command_User_Message_No_Guild_User()
        {
            var userMmessageMock = new Mock<IUserMessage>(MockBehavior.Strict);

            await _commandHandler.InitializeAsync();
            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMmessageMock.Object);
        }
    }
}
