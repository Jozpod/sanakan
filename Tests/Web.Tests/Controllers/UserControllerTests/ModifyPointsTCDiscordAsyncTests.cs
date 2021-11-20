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
    public class ModifyPointsTCDiscordAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Return_Not_Found()
        {

        }

        [TestMethod]
        public async Task Should_Return_Ok()
        {
            var discordUserId = 1ul;
            var amount = 100ul;
            var user = new User(discordUserId, DateTime.UtcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetByDiscordIdAsync(discordUserId))
                .ReturnsAsync(user)
                .Verifiable();

            var result = await _controller.ModifyPointsTCDiscordAsync(discordUserId, amount);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();

            _userRepositoryMock.Verify();
        }
    }
}
