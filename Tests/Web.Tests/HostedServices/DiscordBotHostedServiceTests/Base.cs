using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DAL;
using Sanakan.DiscordBot;
using Sanakan.TaskQueue;
using Sanakan.Web.HostedService;

namespace Sanakan.Web.Tests.HostedServices.DiscordBotHostedServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly DiscordBotHostedService _service;
        protected readonly Mock<IFileSystem> _fileSystemMock = new(MockBehavior.Strict);
        protected readonly Mock<IBlockingPriorityQueue> _blockingPriorityQueueMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClientAccessor> _discordSocketClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<ExperienceConfiguration>> _experienceConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<ICommandHandler> _commandHandlerMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IDatabaseFacade> _databaseFacadeMock = new(MockBehavior.Strict);
        
        public Base()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _service = new(
                _fileSystemMock.Object,
                _blockingPriorityQueueMock.Object,
                NullLogger<DiscordBotHostedService>.Instance,
                serviceScopeFactory,
                _discordSocketClientAccessorMock.Object,
                _discordConfigurationMock.Object,
                _experienceConfigurationMock.Object,
                _systemClockMock.Object,
                _commandHandlerMock.Object,
                _taskManagerMock.Object,
                _databaseFacadeMock.Object);
        }
    }
}
