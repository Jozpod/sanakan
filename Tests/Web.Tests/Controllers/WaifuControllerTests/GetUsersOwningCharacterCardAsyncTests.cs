using DiscordBot.Services.PocketWaifu.Abstractions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
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

            var result = await _controller.GetUsersOwningCharacterCardAsync(userId);
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

            var result = await _controller.GetUsersOwningCharacterCardAsync(userId);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().Be(expected);
        }
    }
}
