using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.TaskQueue.Messages;
using Sanakan.Web.Controllers;
using System;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.UserControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="UserController.ModifyPointsTCDiscordAsync(ulong, ulong)"/> method.
    /// </summary>
    [TestClass]
    public class ModifyPointsTCDiscordAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Not_Found()
        {
            var discordUserId = 1ul;
            var amount = 100ul;
            var user = new User(discordUserId, DateTime.UtcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetByDiscordIdAsync(discordUserId))
                .ReturnsAsync(null as User)
                .Verifiable();

            var result = await _controller.ModifyPointsTCDiscordAsync(discordUserId, amount);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            _userRepositoryMock.Verify();
        }

        [TestMethod]
        public async Task Should_Modify_Points_And_Return_Ok()
        {
            var discordUserId = 1ul;
            var amount = 100ul;
            var user = new User(discordUserId, DateTime.UtcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetByDiscordIdAsync(discordUserId))
                .ReturnsAsync(user);

            _blockingPriorityQueueMock
                .Setup(pr => pr.TryEnqueue(It.IsAny<BaseMessage>()))
                .Returns(true);

            var result = await _controller.ModifyPointsTCDiscordAsync(discordUserId, amount);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
