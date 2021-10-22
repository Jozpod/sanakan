using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Sanakan.Web.Tests
{
    [TestClass]
    public class TokenControllerTests
    {
        private readonly TokenController _controller;
        private readonly Mock<IJwtBuilder> _jwtBuilderMock;

        public TokenControllerTests()
        {
            _jwtBuilderMock = new ();
            _controller = new ();
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
