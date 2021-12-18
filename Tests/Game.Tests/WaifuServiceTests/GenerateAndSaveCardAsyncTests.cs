using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.GenerateAndSaveCardAsync"/> method.
    /// </summary>
    [TestClass]
    public class GenerateAndSaveCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_ImageUrl()
        {
            var card = new Card(1, "test", "test", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);
            var cardImageType = CardImageType.Profile;
            var cardImage = new Image<Rgba32>(300, 300);

            _imageProcessorMock
                .Setup(pr => pr.GetWaifuCardImageAsync(card))
                .ReturnsAsync(cardImage);

            _imageProcessorMock
                .Setup(pr => pr.GetWaifuInProfileCardAsync(card))
                .ReturnsAsync(cardImage);

            _fileSystemMock
                .Setup(pr => pr.OpenWrite(It.IsAny<string>()))
                .Returns(() => new MemoryStream());

            var imageUrl = await _waifuService.GenerateAndSaveCardAsync(card, cardImageType);
            imageUrl.Should().NotBeNull();
        }
    }
}
