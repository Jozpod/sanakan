using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.IntegrationTests.ImageProcessorTests
{
    /// <summary>
    /// Defines tests for <see cref="IImageProcessor.GetDuelCardImage(DuelInfo, DuelImage, Image{Rgba32}, Image{Rgba32})"/> method.
    /// </summary>
#if DEBUG
    [TestClass]
#endif
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

            await ShouldBeEqual("expected-duel-card.png", duelCardImage);
        }
    }
}
