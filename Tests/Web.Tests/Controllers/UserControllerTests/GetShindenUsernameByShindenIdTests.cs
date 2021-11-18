using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.Web.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.UserControllerTests
{
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
