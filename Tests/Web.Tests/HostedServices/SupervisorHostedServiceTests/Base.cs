using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Supervisor;
using Sanakan.Web.HostedService;
using Sanakan.Web.Tests.HostedServices;

namespace Sanakan.Web.Tests.HostedServices.SupervisorHostedServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly SupervisorHostedService _service;
        protected readonly Mock<IDiscordClientAccessor> _discordSocketClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<DaemonsConfiguration>> _daemonsConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserMessageSupervisor> _userMessageSupervisorMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserJoinedGuildSupervisor> _userJoinedGuildSupervisorMock = new(MockBehavior.Strict);
        protected readonly FakeTimer _fakeTimer = new();

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _service = new(
                NullLogger<SupervisorHostedService>.Instance,
                _discordSocketClientAccessorMock.Object,
                _daemonsConfigurationMock.Object,
                _discordConfigurationMock.Object,
                _systemClockMock.Object,
                serviceScopeFactory,
                _taskManagerMock.Object,
                _fakeTimer,
                _userMessageSupervisorMock.Object,
                _userJoinedGuildSupervisorMock.Object);
        }
    }
}
