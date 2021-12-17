using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Web.Controllers;
using Sanakan.Web.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.UserControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="UserController.GetUserByShindenIdAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class GetUserByShindenIdAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Ok_With_Token_Payload()
        {
            var shindenUserId = 1ul;
            var user = new User(1ul, DateTime.UtcNow);
            var tokenData = new TokenData();

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserByShindenIdAsync(shindenUserId))
                .ReturnsAsync(user);

            _userContextMock
                .Setup(pr => pr.HasWebpageClaim())
                .Returns(true);

            _jwtBuilderMock
                .Setup(pr => pr.Build(It.IsAny<TimeSpan>(), It.IsAny<Claim[]>()))
                .Returns(tokenData);

            var result = await _controller.GetUserByShindenIdAsync(shindenUserId);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().BeOfType<UserWithToken>();
        }
    }
}
