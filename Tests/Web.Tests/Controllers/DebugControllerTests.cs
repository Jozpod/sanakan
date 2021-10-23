using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot;
using Sanakan.Web.Controllers;
using System;

namespace Sanakan.Web.Tests
{
    [TestClass]
    public class DebugControllerTests
    {
        private readonly DebugController _controller;
        private readonly Mock<IDiscordSocketClientAccessor> _discordSocketClientMock;

        public DebugControllerTests()
        {
            _discordSocketClientMock = new();
            _controller = new DebugController(
                _discordSocketClientMock.Object,
                NullLogger<DebugController>.Instance);
        }

        [TestMethod]
        public void TestMethod1()
        {
            _controller.RestartBotAsync();
        }
    }
}
