using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Web.HostedService;

namespace Sanakan.Web.Test.HostedServices.ProfileHostedServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly ProfileHostedService _service;
        protected readonly Mock<IOptionsMonitor<DaemonsConfiguration>> _daemonsConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _service = new(
                NullLogger<ProfileHostedService>.Instance,
                _daemonsConfigurationMock.Object,
                _systemClockMock.Object,
                serviceScopeFactory,
                _taskManagerMock.Object);
        }
    }
}
