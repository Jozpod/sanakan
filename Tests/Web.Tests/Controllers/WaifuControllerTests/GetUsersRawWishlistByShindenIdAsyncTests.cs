using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories;
using Sanakan.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="WaifuController.GetUsersRawWishlistByShindenIdAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class GetUsersRawWishlistByShindenIdAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Ok()
        {
            var shindenUserId = 1ul;
            var user = new User(2ul, DateTime.UtcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetByShindenIdAsync(shindenUserId, It.IsAny<UserQueryOptions>()))
                .ReturnsAsync(user);

            var result = await _controller.GetUsersRawWishlistByShindenIdAsync(shindenUserId);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().BeEquivalentTo(user.GameDeck.Wishes);
        }
    }
}
