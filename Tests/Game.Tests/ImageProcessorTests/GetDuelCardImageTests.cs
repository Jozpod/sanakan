using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using System;
using System.Threading.Tasks;
using Sanakan.Services.PocketWaifu;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using FluentAssertions;

namespace Sanakan.Game.Tests.ImageProcessorTests
{
    [TestClass]
    public class GetDuelCardImageTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Duel_Card_Image()
        {
            var duelInfo = new DuelInfo()
            {
                Winner = new Card(1ul, "test 1", "test 1", 10, 10, Rarity.E, Dere.Bodere, DateTime.UtcNow),
                Loser = new Card(2ul, "test 2", "test 2", 10, 10, Rarity.E, Dere.Bodere, DateTime.UtcNow),
            };
            var duelImage = new DuelImage()
            {
                Color = "#aaaaaa",
            };
            var win = new Image<Rgba32>(400, 400);
            var los = new Image<Rgba32>(400, 400);

            var duelCardImage = _imageProcessor.GetDuelCardImage(duelInfo, duelImage, win, los);
            duelCardImage.Should().NotBeNull();

            await ShouldBeEqual("TestData/expected-duel-card.png", duelCardImage);
        }
    }
}
