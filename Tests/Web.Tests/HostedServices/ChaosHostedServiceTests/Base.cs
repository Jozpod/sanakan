﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DiscordBot;
using Sanakan.Web.HostedService;
using Sanakan.Web.Tests.HostedServices;

namespace Sanakan.Web.Test.HostedServices.ChaosHostedServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly ChaosHostedService _service;
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<DaemonsConfiguration>> _daemonsConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordSocketClientAccessor> _discordSocketClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        protected readonly FakeTimer _fakeTimer = new();
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _service = new(
                NullLogger<ChaosHostedService>.Instance,
                _systemClockMock.Object,
                _discordConfigurationMock.Object,
                _daemonsConfigurationMock.Object,
                _discordSocketClientAccessorMock.Object,
                serviceScopeFactory,
                _randomNumberGeneratorMock.Object,
                _fakeTimer,
                _taskManagerMock.Object);
        }
    }
}
