using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Services;
using Sanakan.Web.Controllers;
using System;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests
{
    [TestClass]
    public class InfoControllerTests
    {
        private readonly InfoController _controller;
        private readonly Mock<IHelperService> _helperServiceMock;
        private readonly Mock<IOptionsMonitor<DiscordConfiguration>> _optionMock;

        public InfoControllerTests()
        {
            _controller = new InfoController(
                _helperServiceMock.Object,
                _optionMock.Object);
        }

        [TestMethod]
        public async Task Should_Retrieve_Commands()
        {

            var commands = await _controller.GetCommandsInfoAsync();
        }
    }
}
