using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.GetDeathLog(FightHistory, IEnumerable{PlayerInfo})"/> method.
    /// </summary>
    [TestClass]
    public class GetDeathLogTests : Base
    {
        [TestMethod]
        public void Should_Return_Text()
        {
            var card1 = new Card(1ul, "Test Card 1", "Test Card 2", 100, 100, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card1.Id = 1ul;
            var card2 = new Card(2ul, "Test Card 2", "Test Card 2", 140, 50, Rarity.B, Dere.Bodere, DateTime.UtcNow);
            card2.Id = 2ul;
            var firstPlayer = new PlayerInfo();
            firstPlayer.Cards.Add(card1);
            var secondPlayer = new PlayerInfo();
            secondPlayer.Cards.Add(card2);
            var players = new[] { firstPlayer, secondPlayer };
            var fightHistory = new FightHistory(firstPlayer)
            {
                Rounds = new List<RoundInfo>
                {
                    new RoundInfo
                    {
                        Cards = new List<HpSnapshot>
                        {
                            new HpSnapshot
                            {
                                CardId = 1ul,
                                Hp = -100,
                            }
                        }
                    },
                    new RoundInfo
                    {
                         Cards = new List<HpSnapshot>
                        {
                            new HpSnapshot
                            {
                                CardId = 2ul,
                                Hp = -100,
                            }
                        }
                    },
                }
            };

            var deathLog = _waifuService.GetDeathLog(fightHistory, players);
            deathLog.Should().NotBeNull();
        }
    }
}
