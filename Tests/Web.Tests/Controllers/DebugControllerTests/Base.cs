using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DiscordBot;
using Sanakan.Web.Controllers;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.DebugControllerTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly DebugController _controller;
        protected readonly Mock<IFileSystem> _fileSystemMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClientAccessor> _discordClientAccessorMock = new(MockBehavior.Strict);

        public Base()
        {
            _controller = new DebugController(
                _discordClientAccessorMock.Object,
                _fileSystemMock.Object,
                NullLogger<DebugController>.Instance);
        }
    }
}
