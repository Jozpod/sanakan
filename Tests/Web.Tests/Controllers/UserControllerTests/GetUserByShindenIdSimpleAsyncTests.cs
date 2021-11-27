﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.Web.Controllers;
using Sanakan.DAL.Models;
using System;
using Moq;
using Sanakan.DAL.Repositories;
using System.Security.Claims;

namespace Sanakan.Web.Tests.Controllers.UserControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="UserController.GetUserByShindenIdSimpleAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class GetUserByShindenIdSimpleAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_OK_With_User_Payload()
        {
            var shindenUserId = 1ul;
            var user = new User(2ul, DateTime.UtcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetByShindenIdAsync(shindenUserId, It.IsAny<UserQueryOptions>()))
                .ReturnsAsync(user);

            _userContextMock
                .Setup(pr => pr.HasWebpageClaim())
                .Returns(true);

            _jwtBuilderMock
                .Setup(pr => pr.Build(It.IsAny<TimeSpan>(), It.IsAny<Claim[]>()))
                .Returns(new Api.Models.TokenData());

            var result = await _controller.GetUserByShindenIdSimpleAsync(shindenUserId);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
