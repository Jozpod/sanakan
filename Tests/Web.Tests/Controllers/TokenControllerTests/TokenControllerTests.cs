using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Configuration;
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
        private readonly Mock<IOptionsMonitor<ApiConfiguration>> _configMock;

        public TokenControllerTests()
        {
            _jwtBuilderMock = new ();
            _controller = new (
                _configMock.Object,
                _jwtBuilderMock.Object);
        }

        [TestMethod]
        public void Should_Return_Unauthorized()
        {
            var result = _controller.CreateToken();
        }

        [TestMethod]
        public void Should_Return_Forbidden()
        {
        }

        [TestMethod]
        public void Should_Return_Ok()
        {
        }
    }
}
