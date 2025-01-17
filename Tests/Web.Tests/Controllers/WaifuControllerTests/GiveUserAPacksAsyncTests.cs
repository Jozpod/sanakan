﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using Sanakan.TaskQueue.Messages;
using Sanakan.Web.Controllers;
using System;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="WaifuController.GiveUserAPacksAsync(ulong, System.Collections.Generic.IEnumerable{CardBoosterPack}?)"/> method.
    /// </summary>
    [TestClass]
    public class GiveUserAPacksAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Ok()
        {
            var discordUserId = 1ul;
            var cardBoosterPacks = new[]
            {
                new CardBoosterPack()
                {
                    Pool = new CardBoosterPackPool
                    {
                        Type = CardsPoolType.Random,
                    }
                }
            };
            var user = new User(discordUserId, DateTime.UtcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetCachedAsync(discordUserId))
                .ReturnsAsync(user);

            _blockingPriorityQueueMock
                .Setup(pr => pr.TryEnqueue(It.IsAny<BaseMessage>()))
                .Returns(true);

            var result = await _controller.GiveUserAPacksAsync(discordUserId, cardBoosterPacks);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
