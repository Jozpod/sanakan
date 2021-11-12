using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.TaskQueue;
using Sanakan.Web.HostedService;
using Sanakan.Web.Tests.HostedServices;

namespace Sanakan.Web.Test.HostedServices.TaskQueueHostedServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly TaskQueueHostedService _service;
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IBlockingPriorityQueue> _blockingPriorityQueueMock = new(MockBehavior.Strict);

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _service = new(
                NullLogger<TaskQueueHostedService>.Instance,
                _systemClockMock.Object,
                serviceScopeFactory,
                _blockingPriorityQueueMock.Object);
        }
    }
}
