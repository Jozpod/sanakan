using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.Web.Controllers;
using System;

namespace Sanakan.Web.Tests.Controllers.TokenControllerTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly TokenController _controller;
        protected readonly Mock<IJwtBuilder> _jwtBuilderMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<ApiConfiguration>> _apiConfigurationMock = new(MockBehavior.Strict);

        public Base()
        {
            _controller = new (
                _apiConfigurationMock.Object,
                _jwtBuilderMock.Object);
        }
    }
}
