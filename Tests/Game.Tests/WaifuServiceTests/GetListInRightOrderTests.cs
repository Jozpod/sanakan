using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.GetListInRightOrder(IEnumerable{Card}, Models.HaremType, string)"/> method.
    /// </summary>
    [TestClass]
    public class GetListInRightOrderTests : Base
    {
        [TestMethod]
        public void Should_Return_Embed()
        {
            var card1 = new Card(1, "test 1", "test 1", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);
            var card2 = new Card(1, "test 2", "test 2", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);
            var cards = new[]
            {
                card1,
                card2,
            };
            var haremType = HaremType.Affection;
            var tag = "test";

            var orderedCards = _waifuService.GetListInRightOrder(cards, haremType, tag);
            orderedCards.Should().NotBeNull();
        }
    }
}
