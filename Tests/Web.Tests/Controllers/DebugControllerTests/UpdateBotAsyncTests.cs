using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.DebugControllerTests
{
    [TestClass]
    public class UpdateBotAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Update_Bot()
        {
            _discordClientAccessorMock
                .Setup(pr => pr.LogoutAsync())
                .Returns(Task.CompletedTask);

            _fileSystemMock
                .Setup(pr => pr.Create(Placeholders.UpdateNow))
                .Returns(new MemoryStream());

            var result = await _controller.UpdateBotAsync();
            result.Should().BeOfType<OkResult>();
        }
    }
}
