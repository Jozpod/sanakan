using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DiscordBot;
using Sanakan.Web.Controllers;
using System;

namespace Sanakan.Web.Tests.Controllers.RichMessageControllerTests
{
    [TestClass]
    public class Base
    {
        protected readonly RichMessageController _controller;
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordSocketClientAccessor> _discordSocketClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<SanakanConfiguration>> _sanakanConfigurationMock = new(MockBehavior.Strict);

        public Base()
        {
            _controller = new(
                _systemClockMock.Object,
                _discordSocketClientAccessorMock.Object,
                _sanakanConfigurationMock.Object);
        }

        [TestMethod]
        public void TestMethod1()
        {
            
        }
    }
}
