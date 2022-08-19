using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.IntegrationTests.ImageProcessorTests
{
    /// <summary>
    /// Defines integration tests for <see cref="IImageProcessor.GetWaifuCardImageAsync(Card)"/> method.
    /// </summary>
#if DEBUG
    [TestClass]
#endif
    public class GetWaifuCardImageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Waif_Card_Image()
        {
            var card = new Card(1, "test", "test", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);

            MockHttpGetImage("character.png");

            var cardImage = await _imageProcessor.GetWaifuCardImageAsync(card);
            cardImage.Should().NotBeNull();

            //await SaveImageAsync(cardImage, "actual-waifu-card.png");
            await ShouldBeEqual("expected-waifu-card.png", cardImage);
        }
    }
}
