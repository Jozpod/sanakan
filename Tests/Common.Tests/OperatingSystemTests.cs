﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common.Builder;

namespace Sanakan.Common.Tests
{
    /// <summary>
    /// Defines tests for <see cref="IOperatingSystem.GetCurrentProcess"/> method.
    /// </summary>
    [TestClass]
    public class OperatingSystemTests
    {
        private ServiceProvider _serviceProvider;

        public OperatingSystemTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOperatingSystem();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        public void Should_Return_Process_Info()
        {
            var operatingSystem = _serviceProvider.GetRequiredService<IOperatingSystem>();
            var process = operatingSystem.GetCurrentProcess();
            process.Should().NotBeNull();
        }
    }
}
