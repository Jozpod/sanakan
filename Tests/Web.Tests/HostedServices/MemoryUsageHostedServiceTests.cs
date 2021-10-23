using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Configuration;
using Sanakan.Web.HostedService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Test.HostedServices
{
    [TestClass]
    public class MemoryUsageHostedServiceTests
    {
        private readonly MemoryUsageHostedService _service;
        private readonly Mock<ISystemClock> _systemClockMock;
        private readonly Mock<IOptionsMonitor<SanakanConfiguration>> _optionsMock;
        private readonly IServiceProvider _serviceProvider;
        private readonly Mock<IOperatingSystem> _operatingSystemMock;
        private readonly Mock<ITimer> _timerMock;
        
        public MemoryUsageHostedServiceTests()
        {
            // ILogger<MemoryUsageHostedService> logger,
            //ISystemClock systemClock,
            // IOptionsMonitor<SanakanConfiguration> options,
            // IServiceProvider serviceProvider,
            // IOperatingSystem operatingSystem,
            // ITimer timer)

            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _service = new MemoryUsageHostedService(
                NullLogger<MemoryUsageHostedService>.Instance,
                _systemClockMock.Object,
                _optionsMock.Object,
                serviceScopeFactory,
                _operatingSystemMock.Object,
                _timerMock.Object);
    }

        [TestMethod]
        public async Task Should_Save_Memory_Usage_On_Tick()
        {
            _operatingSystemMock
                .Setup(pr => pr.GetCurrentProcess())
                .Returns(new Process());

            await _service.StartAsync(new CancellationToken());
        }
    }
}
