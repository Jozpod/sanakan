using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories;
using Sanakan.Game.Models;
using Sanakan.TaskQueue.Messages;
using Sanakan.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="WaifuController.GiveShindenUserAPacksAndOpenAsync(ulong, List{Game.Models.CardBoosterPack})"/> method.
    /// </summary>
    [TestClass]
    public class GiveShindenUserAPacksAndOpenAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Enqueue_Task_And_Return_Ok()
        {
            var shindenUserId = 1ul;
            var boosterPacks = new[]
            {
                new CardBoosterPack
                {
                    Count = 1,
                    Pool = new CardBoosterPackPool
                    {
                        TitleId = 1,
                        Type = CardsPoolType.Title,
                    }
                }
            };
            var cards = new[]
            {
               new Card(1ul, "title", "name", 100, 50, Rarity.B, Dere.Bodere, DateTime.UtcNow),
            };
            var user = new User(1ul, DateTime.UtcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetByShindenIdAsync(shindenUserId, It.IsAny<UserQueryOptions>()))
                .ReturnsAsync(user);

            _waifuServiceMock
                .Setup(pr => pr.OpenBoosterPackAsync(null, It.IsAny<BoosterPack>()))
                .ReturnsAsync(cards);

            _blockingPriorityQueueMock
                .Setup(pr => pr.TryEnqueue(It.IsAny<GiveBoosterPackMessage>()))
                .Returns(true);

            var result = await _controller.GiveShindenUserAPacksAndOpenAsync(shindenUserId, boosterPacks);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().BeEquivalentTo(cards);
        }
    }
}
