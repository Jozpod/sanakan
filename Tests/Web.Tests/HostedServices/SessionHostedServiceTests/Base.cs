﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
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

namespace Sanakan.Web.Test.HostedServices.SessionHostedServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly SessionHostedService _service;
        protected readonly Mock<IOptionsMonitor<DaemonsConfiguration>> _daemonsConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly FakeTimer _fakeTimer = new();

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _service = new(
                NullLogger<SessionHostedService>.Instance,
                _daemonsConfigurationMock.Object,
                _systemClockMock.Object,
                serviceScopeFactory,
                _fakeTimer);
        }
    }
}
