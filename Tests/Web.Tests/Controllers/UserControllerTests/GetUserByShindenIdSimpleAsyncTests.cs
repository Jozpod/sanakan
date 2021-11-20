using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.UserControllerTests
{
    [TestClass]
    public class GetUserByShindenIdSimpleAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Question()
        {
            var userId = 1ul;
            var result = await _controller.GetUserByShindenIdSimpleAsync(userId);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
