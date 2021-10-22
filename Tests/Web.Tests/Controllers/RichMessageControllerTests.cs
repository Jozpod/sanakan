using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Sanakan.Web.Tests
{
    [TestClass]
    public class RichMessageControllerTests
    {
        private readonly RichMessageController _controller;

        public RichMessageControllerTests()
        {
            _controller = new RichMessageController();
        }

        [TestMethod]
        public void TestMethod1()
        {
            _controller = new RichMessageController();

        }
    }
}
