using FluentAssertions;
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
    /// Defines tests for <see cref="WaifuController.OpenAPackAsync(int)"/> method.
    /// </summary>
    [TestClass]
    public class OpenAPackAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Ok()
        {
            var packNumber = 1;
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
                .Setup(pr => pr.GetCachedAsync(discordUserId))
                .ReturnsAsync(user);

            _waifuServiceMock
                .Setup(pr => pr.OpenBoosterPackAsync(null, It.IsAny<BoosterPack>()))
                .ReturnsAsync(cards);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(discordUserId))
                .ReturnsAsync(user);

            _blockingPriorityQueueMock
                .Setup(pr => pr.TryEnqueue(It.IsAny<BaseMessage>()))
                .Returns(true);

            var result = await _controller.OpenAPackAsync(packNumber);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
