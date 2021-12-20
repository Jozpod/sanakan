using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.TaskQueue.Messages;
using Sanakan.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="WaifuController.ToggleCardStatusAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class ToggleCardStatusAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_NotFound()
        {
            var cardId = 1ul;
            var discordUserId = 1ul;
            var user = null as User;

            _userContextMock
                .Setup(pr => pr.DiscordId)
                .Returns(discordUserId);

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(discordUserId))
                .ReturnsAsync(user);

            var result = await _controller.ToggleCardStatusAsync(cardId);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [TestMethod]
        public async Task Should_Enqueue_Task_And_Return_Ok()
        {
            var cardId = 1ul;
            var discordUserId = 1ul;
            var user = new User(discordUserId, DateTime.UtcNow);
            var cards = new List<Card>
            {
                new Card(1ul, "title", "name", 100, 50, Rarity.B, Dere.Bodere, DateTime.UtcNow),
            };

            _userContextMock
                .Setup(pr => pr.DiscordId)
                .Returns(discordUserId);

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(discordUserId))
                .ReturnsAsync(user);

            _blockingPriorityQueueMock
                .Setup(pr => pr.TryEnqueue(It.IsAny<ToggleCardMessage>()))
                .Returns(true);

            var result = await _controller.ToggleCardStatusAsync(cardId);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
