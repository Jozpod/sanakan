using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Api.Models;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.UserControllerTests
{
    [TestClass]
    public class RegisterUserAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Return_Ok()
        {
            var payload = new UserRegistration
            {

            };
            var result = await _controller.RegisterUserAsync(payload);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
