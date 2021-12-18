using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.GetExperienceToUpgrade(Card, Card)"/> method.
    /// </summary>
    [TestClass]
    public class GetExperienceToUpgradeTests : Base
    {
        [TestMethod]
        public void Should_Return_Experience()
        {
            var cardToUpgrade = new Card(1, "test 1", "test 1", 100, 50, Rarity.A, Dere.Bodere, DateTime.Now);
            var cardToSacrifice = new Card(2, "test 2", "test 2", 200, 150, Rarity.SSS, Dere.Bodere, DateTime.Now);

            var experience = _waifuService.GetExperienceToUpgrade(cardToUpgrade, cardToSacrifice);
            experience.Should().Be(20.25);
        }
    }
}
