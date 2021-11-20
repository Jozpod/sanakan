using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    [TestClass]
    public class GetCardsWithTagAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Ok()
        {
            var tag = "tag";

            var result = await _controller.GetCardsWithTagAsync(tag);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
