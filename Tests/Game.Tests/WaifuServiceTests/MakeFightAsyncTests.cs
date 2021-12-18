using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.MakeFight(IEnumerable{Models.PlayerInfo}, bool)"/> method.
    /// </summary>
    [TestClass]
    public class MakeFightAsyncTests : Base
    {
        [TestMethod]
        public void Should_Return_Fight_Result()
        {
            var firstPlayer = new PlayerInfo
            {

            };
            var secondPlayer = new PlayerInfo
            {

            };
            var players = new[] { firstPlayer, secondPlayer };

            _randomNumberGeneratorMock
                .Setup(pr => pr.Shuffle(It.IsAny<IEnumerable<CardWithHealth>>()))
                .Returns<IEnumerable<CardWithHealth>>(pr => pr);

            var fightResult = _waifuService.MakeFight(players, false);
            fightResult.Should().NotBeNull();
        }
    }
}
