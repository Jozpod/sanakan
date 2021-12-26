using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.ImageProcessorTests
{
    /// <summary>
    /// Defines tests for <see cref="IImageProcessor.GetWaifuCardImageAsync(Card)"/> method.
    /// </summary>
    [TestClass]
    public class GetWaifuCardImageAsyncTests : Base
    {
        public static IEnumerable<object?[]> EnumerateAllCardQualities
        {
            get
            {
                foreach (var fromFigure in new[] { false, true })
                {
                    foreach (var quality in Enum.GetValues<Quality>())
                    {
                        yield return new object?[] { quality, fromFigure };
                    }
                }
            }
        }

        [DynamicData(nameof(EnumerateAllCardQualities))]
        [DataTestMethod]
        public async Task Should_Return_Waif_Card_Image(Quality quality, bool fromFigure)
        {
            var card = new Card(1, "test", "test", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);
            card.Quality = quality;
            card.FromFigure = fromFigure;

            _fileSystemMock
                .Setup(pr => pr.Exists(It.IsAny<string>()))
                .Returns(false);

            _fileSystemMock
                .Setup(pr => pr.OpenRead(It.IsAny<string>()))
                .Returns(() => Utils.CreateFakeImage(600, 600));

            var cardImage = await _imageProcessor.GetWaifuCardImageAsync(card);
            cardImage.Should().NotBeNull();
        }
    }
}
