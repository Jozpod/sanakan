using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.UserControllerTests
{
    [TestClass]
    public class GetUserIdByNameAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Change_Nickname()
        {
            var name = "test";
            var result = await _controller.GetUserIdByNameAsync(name);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
