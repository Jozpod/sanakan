using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sanakan.Web.Tests.Controllers.RichMessageControllerTests
{
    [TestClass]
    public class GetExampleMessageTests : Base
    {
        [TestMethod]
        public void Should_Return_Example_Message()
        {
            var result = _controller.GetExampleMessage();
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
