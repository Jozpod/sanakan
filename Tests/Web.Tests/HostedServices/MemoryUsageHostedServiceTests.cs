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
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Web.HostedService;
using Sanakan.Web.Tests.HostedServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Web.Test.HostedServices
{
    [TestClass]
    public class MemoryUsageHostedServiceTests
    {
        private readonly MemoryUsageHostedService _service;
        private readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        private readonly Mock<IOptionsMonitor<DaemonsConfiguration>> _optionsMock = new(MockBehavior.Strict);
        private readonly Mock<IOperatingSystem> _operatingSystemMock = new(MockBehavior.Strict);
        private readonly Mock<ISystemAnalyticsRepository> _systemAnalyticsRepositoryMock = new(MockBehavior.Strict);
        private readonly FakeTimer _fakeTimer = new ();
        private readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);
        private readonly Mock<IDatabaseFacade> _databaseFacadeMock = new(MockBehavior.Strict);
        private readonly Process _process = Process.GetCurrentProcess();

        public MemoryUsageHostedServiceTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped((sp) => _systemAnalyticsRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _operatingSystemMock
                .Setup(pr => pr.GetCurrentProcess())
                .Returns(_process);

            _databaseFacadeMock
                .Setup(pr => pr.EnsureCreatedAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _taskManagerMock
                .Setup(pr => pr.Delay(
                    It.IsAny<TimeSpan>(),
                    It.IsAny<CancellationToken>()));

            _service = new MemoryUsageHostedService(
                NullLogger<MemoryUsageHostedService>.Instance,
                _systemClockMock.Object,
                _optionsMock.Object,
                serviceScopeFactory,
                _operatingSystemMock.Object,
                _taskManagerMock.Object,
                _fakeTimer,
                _databaseFacadeMock.Object);
        }

        [TestMethod]
        public async Task Should_Cancel_Timer()
        {
            _optionsMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new DaemonsConfiguration
                {
                    CaptureMemoryUsageDueTime = TimeSpan.FromMinutes(1),
                    CaptureMemoryUsagePeriod = TimeSpan.FromMinutes(1),
                })
                .Verifiable();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            await _service.StartAsync(cancellationTokenSource.Token);
            _fakeTimer.Stopped.Should().BeTrue();
        }


        [TestMethod]
        public async Task Should_Save_Memory_Usage_On_Tick()
        {
            _optionsMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new DaemonsConfiguration
                {
                    CaptureMemoryUsageDueTime = TimeSpan.FromMinutes(1),
                    CaptureMemoryUsagePeriod = TimeSpan.FromMinutes(1),
                })
                .Verifiable();

            _operatingSystemMock
                .Setup(pr => pr.Refresh(_process))
                .Verifiable();

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow)
                .Verifiable();

            _systemAnalyticsRepositoryMock
                .Setup(pr => pr.Add(It.IsAny<SystemAnalytics>()))
                .Verifiable();

            _systemAnalyticsRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _service.StartAsync(new CancellationToken());
            await Task.Delay(TimeSpan.FromSeconds(1));
            _fakeTimer.RaiseTickEvent();

            _operatingSystemMock.Verify();
            _optionsMock.Verify();
            _systemClockMock.Verify();
            _systemAnalyticsRepositoryMock.Verify();
        }
    }
}
