using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Configuration;
using Sanakan.Web.Controllers;
using System;

namespace Sanakan.Web.Tests
{
    [TestClass]
    public class TokenControllerTests
    {
        private readonly TokenController _controller;
        private readonly Mock<IJwtBuilder> _jwtBuilderMock;
        private readonly Mock<IOptionsMonitor<SanakanConfiguration>> _configMock;

        public TokenControllerTests()
        {
            _jwtBuilderMock = new ();
            _controller = new (
                _configMock.Object,
                _jwtBuilderMock.Object);
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
