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
        [DataRow(HaremType.Attack)]
        [DataRow(HaremType.Blocked)]
        [DataRow(HaremType.Cage)]
        [DataRow(HaremType.CustomPicture)]
        [DataRow(HaremType.Defence)]
        [DataRow(HaremType.Broken)]
        [DataRow(HaremType.Health)]
        [DataRow(HaremType.NoPicture)]
        [DataRow(HaremType.NoTag)]
        [DataRow(HaremType.Blocked)]
        public void Should_Return_Embed(HaremType haremType)
        {
            var card1 = new Card(1, "test 1", "test 1", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);
            var card2 = new Card(1, "test 2", "test 2", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);
            var cards = new[]
            {
                card1,
                card2,
            };
            var tag = "test";

            var orderedCards = _waifuService.GetListInRightOrder(cards, haremType, tag);
            orderedCards.Should().NotBeNull();
        }
    }
}
