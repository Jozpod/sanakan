using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DiscordBot;
using Sanakan.Web.Controllers;

namespace Sanakan.Web.Tests.Controllers.RichMessageControllerTests
{
    [TestClass]
    public class Base
    {
        protected readonly RichMessageController _controller;
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClientAccessor> _discordSocketClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<SanakanConfiguration>> _sanakanConfigurationMock = new(MockBehavior.Strict);

        public Base()
        {
            _controller = new(
                _systemClockMock.Object,
                _discordSocketClientAccessorMock.Object,
                _sanakanConfigurationMock.Object);
        }
    }
}
