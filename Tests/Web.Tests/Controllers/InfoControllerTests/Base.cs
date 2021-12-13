using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Configuration;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Web.Controllers;

namespace Sanakan.Web.Tests.Controllers.InfoControllerTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly InfoController _controller;
        protected readonly Mock<IHelperService> _helperServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);

        public Base()
        {
            _controller = new InfoController(
                _helperServiceMock.Object,
                _discordConfigurationMock.Object);
        }

        [TestMethod]
        public void Should_Retrieve_Commands()
        {

            var commands = _controller.GetCommandsInfoAsync();
        }
    }
}
