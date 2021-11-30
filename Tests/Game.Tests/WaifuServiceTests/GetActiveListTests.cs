using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using Discord;
using Sanakan.Game.Services.Abstractions;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.GetActiveList(IEnumerable{Card})"/> method.
    /// </summary>
    [TestClass]
    public class GetActiveListTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Embed_With_Cards_Details()
        {
            var card1 = new Card(1, "test 1", "test 1", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);
            var card2 = new Card(1, "test 2", "test 2", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);
            var cards = new[]
            {
                card1,
                card2,
            };

            var embed = _waifuService.GetActiveList(cards);
            embed.Should().NotBeNull();
            embed.Description.Should().NotBeNullOrEmpty();
        }
    }
}
