using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DiscordBot;
using Sanakan.Web.Controllers;
using System;

namespace Sanakan.Web.Tests
{
    [TestClass]
    public class RichMessageControllerTests
    {
        private readonly RichMessageController _controller;
        private readonly Mock<ISystemClock> _systemClockMock;
        private readonly Mock<IDiscordSocketClientAccessor> _discordSocketClientAccessorMock;
        private readonly Mock<IOptionsMonitor<SanakanConfiguration>> _config;

        public RichMessageControllerTests()
        {
            _systemClockMock = new(MockBehavior.Strict);
            _discordSocketClientAccessorMock = new(MockBehavior.Strict);
            _config = new(MockBehavior.Strict);

            _controller = new RichMessageController(
                _systemClockMock.Object,
                _discordSocketClientAccessorMock.Object,
                _config.Object);
        }

        [TestMethod]
        public void TestMethod1()
        {
            
        }
    }
}
