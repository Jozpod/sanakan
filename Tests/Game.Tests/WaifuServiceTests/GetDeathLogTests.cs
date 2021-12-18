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
            var firstPlayer = new PlayerInfo
            {

            };
            var secondPlayer = new PlayerInfo
            {

            };
            var players = new[] { firstPlayer, secondPlayer };
            var fightHistory = new FightHistory(firstPlayer)
            {

            };


            var deathLog = _waifuService.GetDeathLog(fightHistory, players);
            deathLog.Should().NotBeNull();
        }
    }
}
