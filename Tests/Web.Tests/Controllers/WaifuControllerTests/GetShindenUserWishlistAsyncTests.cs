using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    [TestClass]
    public class GetShindenUserWishlistAsyncTests : Base
    {
       
        [TestMethod]
        public async Task Should_Return_Ok()
        {
            var shindenUserId = 1ul;
            var result = await _controller.GetShindenUserWishlistAsync(shindenUserId);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
