using Discord;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.Web.Controllers;
using System.Collections.Generic;

namespace Sanakan.Web.Tests.Controllers.RichMessageControllerTests
{
    [TestClass]
    public class Base
    {
        protected readonly RichMessageController _controller;
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClientAccessor> _discordClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClient> _discordClientMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<SanakanConfiguration>> _sanakanConfigurationMock = new(MockBehavior.Strict);
        protected readonly SanakanConfiguration _configuration;

        public Base()
        {
            _configuration = new SanakanConfiguration
            {
                RMConfig = new List<Configuration.RichMessageConfig>()
            };

            _sanakanConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(_configuration);

            _discordClientAccessorMock
                .Setup(pr => pr.Client)
                .Returns(_discordClientMock.Object);

            _controller = new(
                _systemClockMock.Object,
                _discordClientAccessorMock.Object,
                _sanakanConfigurationMock.Object);
        }
    }
}
