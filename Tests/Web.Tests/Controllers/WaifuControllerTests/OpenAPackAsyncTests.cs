using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    [TestClass]
    public class OpenAPackAsyncTests : Base
    {
       
        [TestMethod]
        public async Task Should_Return_Ok()
        {
            var packNumber = 1;
            var result = await _controller.OpenAPackAsync(packNumber);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
