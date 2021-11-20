using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    [TestClass]
    public class GetUserWaifuProfileAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Question()
        {
            var shindenUserId = 1ul;
            var result = await _controller.GetUserWaifuProfileAsync(shindenUserId);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
