using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common;
using Sanakan.Common.Builder;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class DefaultSystemClockTests
    {
        private ServiceProvider _serviceProvider;

        public DefaultSystemClockTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSystemClock();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        public void Should_Return_Current_Date()
        {
            var systemClock = _serviceProvider.GetRequiredService<ISystemClock>();
            systemClock.UtcNow.Should().NotBe(DateTime.MinValue);
            systemClock.StartOfMonth.Should().NotBe(DateTime.MinValue);
        }
    }
}
