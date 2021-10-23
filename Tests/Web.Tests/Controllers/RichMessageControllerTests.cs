using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
        private readonly Mock<IDiscordSocketClientAccessor> _client;
        private readonly Mock<IOptionsMonitor<SanakanConfiguration>> _config;

        public RichMessageControllerTests()
        {
            _controller = new RichMessageController(
                _client.Object,
                _config.Object);
        }

        [TestMethod]
        public void TestMethod1()
        {
            
        }
    }
}
