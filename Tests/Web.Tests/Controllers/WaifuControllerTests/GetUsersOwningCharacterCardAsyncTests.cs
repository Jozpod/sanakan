using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Web.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="WaifuController.GetUserIdsOwningCharacterCardAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class GetUsersOwningCharacterCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_NotFound()
        {
            var userId = 0ul;
            var expected = new List<ulong>();

            _userRepositoryMock
                .Setup(pr => pr.GetUserShindenIdsByHavingCharacterAsync(userId))
                .ReturnsAsync(expected);

            var result = await _controller.GetUserIdsOwningCharacterCardAsync(userId);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [TestMethod]
        public async Task Should_Return_Ok()
        {
            var userId = 0ul;
            var expected = new List<ulong>() { 0ul };

            _userRepositoryMock
                .Setup(pr => pr.GetUserShindenIdsByHavingCharacterAsync(userId))
                .ReturnsAsync(expected);

            var result = await _controller.GetUserIdsOwningCharacterCardAsync(userId);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().Be(expected);
        }
    }
}
