using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Web.Controllers;
using System;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.UserControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="UserController.GetUserByDiscordIdAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class GetUserByDiscordIdAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Ok_With_User_Payload()
        {
            var discordUserId = 0ul;
            var user = new User(discordUserId, DateTime.UtcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetCachedAsync(discordUserId))
                .ReturnsAsync(user);

            var result = await _controller.GetUserByDiscordIdAsync(discordUserId);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().BeEquivalentTo(user);
        }
    }
}
