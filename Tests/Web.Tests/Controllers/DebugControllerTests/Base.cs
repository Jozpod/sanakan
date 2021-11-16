using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot;
using Sanakan.Web.Controllers;
using System;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.DebugControllerTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly DebugController _controller;
        protected readonly Mock<IDiscordClientAccessor> _discordSocketClientMock = new(MockBehavior.Strict);

        public Base()
        {
            _controller = new DebugController(
                _discordSocketClientMock.Object,
                NullLogger<DebugController>.Instance);
        }

        [TestMethod]
        public async Task Should_Restart_Bot()
        {
            await _controller.RestartBotAsync();
        }
    }
}
