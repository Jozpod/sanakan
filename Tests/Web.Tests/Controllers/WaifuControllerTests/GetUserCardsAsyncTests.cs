using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Web.Controllers;
using System;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="WaifuController.GetUserCardsAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class GetUserCardsAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Return_NotFound()
        {
            var shindenUserId = 0ul;
            User? expected = null;

            _cardRepositoryMock
                .Setup(pr => pr.GetByShindenUserIdAsync(shindenUserId))
                .ReturnsAsync(expected);

            var result = await _controller.GetUserCardsAsync(shindenUserId);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Should_Return_Ok_With_Cards()
        {
            var discordUserId = 1ul;
            var shindenUserId = 1ul;
            var expected = new User(discordUserId, DateTime.UtcNow);
            expected.ShindenId = shindenUserId;

            _cardRepositoryMock
                .Setup(pr => pr.GetByShindenUserIdAsync(shindenUserId))
                .ReturnsAsync(expected);

            var result = await _controller.GetUserCardsAsync(shindenUserId);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
