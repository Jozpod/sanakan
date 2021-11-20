using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    [TestClass]
    public class ToggleCardStatusAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Ok()
        {
            var cardId = 1ul;

            var result = await _controller.ToggleCardStatusAsync(cardId);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
