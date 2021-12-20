using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories;
using Sanakan.Game.Models;
using Sanakan.TaskQueue.Messages;
using Sanakan.Web.Controllers;
using Sanakan.Web.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="WaifuController.GiveShindenUserAPacksAsync(ulong, List{Game.Models.CardBoosterPack})"/> method.
    /// </summary>
    [TestClass]
    public class GiveShindenUserAPacksAsyncTests : Base
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
            var tokenData = new TokenData
            {
                Expire = DateTime.UtcNow,
                Token = "test token",
            };

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserByShindenIdAsync(shindenUserId))
                .ReturnsAsync(user);

            _blockingPriorityQueueMock
                .Setup(pr => pr.TryEnqueue(It.IsAny<GiveCardsMessage>()))
                .Returns(true);

            _userContextMock
                .Setup(pr => pr.HasWebpageClaim())
                .Returns(true);

            _jwtBuilderMock
                .Setup(pr => pr.Build(It.IsAny<TimeSpan>(), It.IsAny<Claim[]>()))
                .Returns(tokenData);

            var result = await _controller.GiveShindenUserAPacksAsync(shindenUserId, boosterPacks);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().BeOfType<UserWithToken>();
        }
    }
}
