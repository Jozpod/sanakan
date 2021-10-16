using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Test.HostedServices
{
    [TestClass]
    public class MemoryUsageHostedServiceTests
    {
        private readonly MemoryUsageHostedService _service;
        private readonly Mock<ISystemClock> _systemClockMock;
        private readonly Mock<IOptions<SanakanConfiguration>> _optionsMock;
        private readonly IServiceProvider _serviceProvider;
        private readonly Mock<IOperatingSystem> _operatingSystemMock;
        private readonly Mock<ITimer> _timerMock;
        
        public MemoryUsageHostedServiceTests()
        {
            _service = new MemoryUsageHostedService(
                _systemClockMock.Object,
                _operatingSystemMock.Object,
                _systemClockMock.Object,
                _timerMock.Object,
                _optionsMock.Object
                NullLogger.Instance);
    }

        [TestMethod]
        public async Task Should_Save_Memory_Usage_On_Tick()
        {
            _operatingSystem.Setup();

            await _service.ExecuteAsync();
        }
    }
}
