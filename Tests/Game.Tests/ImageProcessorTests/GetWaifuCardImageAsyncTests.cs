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
    [TestClass]
    public class GetWaifuCardImageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Waif_Card_Image()
        {
            var card = new Card(1, "test", "test", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);

            var cardImage = await _imageProcessor.GetWaifuCardImageAsync(card);
            cardImage.Should().NotBeNull();
        }
    }
}
