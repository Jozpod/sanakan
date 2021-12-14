using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.ImageProcessorTests
{
    /// <summary>
    /// Defines tests for <see cref="IImageProcessor.GetWaifuCardImageAsync(Card)"/> method.
    /// </summary>
#if DEBUG
    [TestClass]
#endif
    public class GetWaifuCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Generate_Waifu_Card()
        {
            var card = new Card(1, "test", "test", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);

            MockHttpGetImage("TestData/character.png");

            var waifuCard = await _imageProcessor.GetWaifuCardImageAsync(card);
            waifuCard.Should().NotBeNull();

            await ShouldBeEqual("TestData/expected-waifu-card.png", waifuCard);
        }
    }
}
