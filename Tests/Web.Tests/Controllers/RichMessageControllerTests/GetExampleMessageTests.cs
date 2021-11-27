using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sanakan.Web.Controllers;

namespace Sanakan.Web.Tests.Controllers.RichMessageControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="RichMessageController.GetExampleMessage"/> method.
    /// </summary>
    [TestClass]
    public class GetExampleMessageTests : Base
    {
        [TestMethod]
        public void Should_Return_Example_Message()
        {
            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            var result = _controller.GetExampleMessage();
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
