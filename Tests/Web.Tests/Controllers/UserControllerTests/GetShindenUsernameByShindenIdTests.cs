using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.Web.Controllers;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.UserControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="UserController.GetUserByShindenIdSimpleAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class GetShindenUsernameByShindenIdTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Shinden_User_Details()
        {
            var shindenUserId = 1ul;
            var userResult = new Result<UserInfo>
            {
                Value = new UserInfo
                {
                    Id = shindenUserId,
                    Name = "test",
                    Email = "test@test.test",
                }
            };

            _shindenClientMock
                .Setup(pr => pr.GetUserInfoAsync(shindenUserId))
                .ReturnsAsync(userResult);

            var result = await _controller.GetShindenUsernameByShindenId(shindenUserId);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
