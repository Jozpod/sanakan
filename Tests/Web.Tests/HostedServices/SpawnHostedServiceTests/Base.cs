using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DiscordBot;
using Sanakan.Game.Services.Abstractions;
using Sanakan.TaskQueue;
using Sanakan.Web.HostedService;
using Sanakan.Web.Tests.HostedServices;

namespace Sanakan.Web.Tests.HostedServices.SpawnHostedServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly SpawnHostedService _service;
        protected readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<ExperienceConfiguration>> _experienceConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClientAccessor> _discordSocketClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IBlockingPriorityQueue> _blockingPriorityQueueMock = new(MockBehavior.Strict);
        protected readonly Mock<IWaifuService> _waifuServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);
        protected readonly FakeTimer _fakeTimer = new();

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _service = new(
                _randomNumberGeneratorMock.Object,
                _blockingPriorityQueueMock.Object,
                NullLogger<SpawnHostedService>.Instance,
                _discordConfigurationMock.Object,
                _experienceConfigurationMock.Object,
                _discordSocketClientAccessorMock.Object,
                _systemClockMock.Object,
                serviceScopeFactory,
                _taskManagerMock.Object,
                _waifuServiceMock.Object,
                _fakeTimer);
        }
    }
}
