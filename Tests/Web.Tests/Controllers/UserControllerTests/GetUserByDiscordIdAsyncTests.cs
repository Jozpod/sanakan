using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using System;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.UserControllerTests
{
    [TestClass]
    public class GetUserByDiscordIdAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Return_Ok()
        {
            var discordUserId = 0ul;
            var user = new User(discordUserId, DateTime.UtcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(discordUserId))
                .ReturnsAsync(user);

            var result = await _controller.GetUserByDiscordIdAsync(discordUserId);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().BeEquivalentTo(user);
        }
    }
}
