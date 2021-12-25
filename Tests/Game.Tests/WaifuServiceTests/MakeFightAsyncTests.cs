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
            var sourceUserId = 1ul;
            var enemyUserId = 2ul;
            var card1 = new Card(1ul, "Test Card 1", "Test Card 1", 200, 50, Rarity.S, Dere.Bodere, DateTime.UtcNow);
            card1.GameDeckId = sourceUserId;
            var card2 = new Card(2ul, "Test Card 2", "Test Card 2", 150, 150, Rarity.B, Dere.Deredere, DateTime.UtcNow);
            card2.GameDeckId = sourceUserId;
            var card3 = new Card(3ul, "Test Card 3", "Test Card 3", 150, 250, Rarity.SS, Dere.Dandere, DateTime.UtcNow);
            card3.GameDeckId = enemyUserId;
            var card4 = new Card(4ul, "Test Card 4", "Test Card 4", 250, 150, Rarity.A, Dere.Kuudere, DateTime.UtcNow);
            card4.GameDeckId = enemyUserId;
            var firstPlayer = new PlayerInfo
            {
                Cards = new List<Card>
                {
                    card1,
                    card2,
                }
            };
            var secondPlayer = new PlayerInfo
            {
                Cards = new List<Card>
                {
                    card3,
                    card4
                }
            };
            var players = new[] { firstPlayer, secondPlayer };

            _randomNumberGeneratorMock
                .Setup(pr => pr.Shuffle(It.IsAny<IEnumerable<CardWithHealth>>()))
                .Returns<IEnumerable<CardWithHealth>>(pr => pr);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<CardWithHealth>>()))
                .Returns<IEnumerable<CardWithHealth>>((value) => value.First());

            var fightResult = _waifuService.MakeFight(players, false);
            fightResult.Should().NotBeNull();
        }
    }
}
