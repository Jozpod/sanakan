using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Sanakan.Web.Tests
{
    [TestClass]
    public class DebugControllerTests
    {
        private readonly DebugController _controller;
        private readonly Mock<DiscordSocketClient> _discordSocketClientMock;

        public DebugControllerTests()
        {
            _discordSocketClientMock = new();
            _controller = new DebugController(
                _discordSocketClientMock,
                NullLogger.Instance);
        }

        [TestMethod]
        public void TestMethod1()
        {
            _controller.RestartBotAsync();
        }
    }
}
