using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Daemon.HostedService;
using Sanakan.DAL;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Services;
using Sanakan.TaskQueue;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.DiscordBotHostedServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly BackgroundService _service;
        protected readonly Mock<IFileSystem> _fileSystemMock = new(MockBehavior.Strict);
        protected readonly Mock<IBlockingPriorityQueue> _blockingPriorityQueueMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClientAccessor> _discordClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClient> _discordClientMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<ExperienceManager> _experienceManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<AuditService> _auditServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<ICommandHandler> _commandHandlerMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IDatabaseFacade> _databaseFacadeMock = new(MockBehavior.Strict);
        protected readonly Mock<IHostApplicationLifetime> _hostApplicationLifetimeMock = new(MockBehavior.Strict);

        protected readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ITimeStatusRepository> _timeStatusRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IPenaltyInfoRepository> _penaltyInfoRepositoryMock = new(MockBehavior.Strict);

        public Base()
        {
            _discordClientAccessorMock
                .Setup(pr => pr.Client)
                .Returns(_discordClientMock.Object);

            //_experienceConfigurationMock
            //    .Setup(pr => pr.CurrentValue)
            //    .Returns(new ExperienceConfiguration
            //    {
            //        CharPerPoint = 100,
            //        MinPerMessage = 50,
            //        MaxPerMessage = 25,
            //    });

            _discordConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new DiscordConfiguration
                {
                    BotToken = "token",
                    Prefix = "prefix",
                    RestartWhenDisconnected = true,
                });

            _hostApplicationLifetimeMock
                .Setup(pr => pr.ApplicationStopping)
                .Returns(new CancellationToken());

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
            serviceCollection.AddSingleton(_timeStatusRepositoryMock.Object);
            serviceCollection.AddSingleton(_penaltyInfoRepositoryMock.Object);
            serviceCollection.AddSingleton(_userRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            
            _service = new DiscordBotHostedService(
                _fileSystemMock.Object,
                _blockingPriorityQueueMock.Object,
                NullLogger<DiscordBotHostedService>.Instance,
                serviceScopeFactory,
                _discordClientAccessorMock.Object,
                _discordConfigurationMock.Object,
                _commandHandlerMock.Object,
                _taskManagerMock.Object,
                _databaseFacadeMock.Object,
                _experienceManagerMock.Object,
                _auditServiceMock.Object,
                _hostApplicationLifetimeMock.Object);
        }

        public async Task StartAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            _databaseFacadeMock
                .Setup(pr => pr.EnsureCreatedAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var fakeDirectory = new System.IO.DirectoryInfo(Path.GetDirectoryName(typeof(ExecuteAsyncTests).Assembly.Location)!);

            _fileSystemMock
                .Setup(pr => pr.CreateDirectory(It.IsAny<string>()))
                .Returns(fakeDirectory);

            _discordClientAccessorMock
                .Setup(pr => pr.LoginAsync(TokenType.Bot, It.IsAny<string>(), true))
                .Returns(Task.CompletedTask);

            _discordClientAccessorMock
                .Setup(pr => pr.SetGameAsync(It.IsAny<string>(), null, ActivityType.Playing))
                .Returns(Task.CompletedTask);

            _discordClientMock
                .Setup(pr => pr.StartAsync())
                .Returns(Task.CompletedTask);

            _commandHandlerMock
                .Setup(pr => pr.InitializeAsync())
                .Returns(Task.CompletedTask);

            _taskManagerMock
                .Setup(pr => pr.Delay(
                    It.IsAny<TimeSpan>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.StartAsync(cancellationTokenSource.Token);
        }
    }
}
