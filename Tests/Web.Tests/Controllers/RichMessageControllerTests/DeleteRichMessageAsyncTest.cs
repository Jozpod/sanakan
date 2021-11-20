using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.RichMessageControllerTests
{
    [TestClass]
    public class DeleteRichMessageAsyncTest : Base
    {
        [TestMethod]
        public async Task Should_Return_Question()
        {
            var messageId = 1ul;
            var result = await _controller.DeleteRichMessageAsync(messageId);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
