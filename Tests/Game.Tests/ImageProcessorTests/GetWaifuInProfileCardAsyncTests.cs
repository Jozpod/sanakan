using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.ImageProcessorTests
{
    /// <summary>
    /// Defines tests for <see cref="IImageProcessor.GetWaifuInProfileCardAsync(Card)"/> method.
    /// </summary>
    [TestClass]
    public class GetWaifuInProfileCardAsyncTests : Base
    {
        public static IEnumerable<object?[]> EnumerateAllCardTypes
        {
            get
            {
                foreach (var customBorderUrl in new[] { new Uri("https://test.com/image.png"), null })
                {
                    foreach (var fromFigure in new[] { false, true })
                    {
                        foreach (var quality in Enum.GetValues<Quality>())
                        {
                            yield return new object?[] { quality, fromFigure, customBorderUrl };
                        }
                    }
                }
            }
        }

        [DynamicData(nameof(EnumerateAllCardTypes))]
        [DataTestMethod]
        public async Task Should_Generate_Waifu_In_Profile(Quality quality, bool fromFigure, Uri? customBorderUrl)
        {
            var card = new Card(1, "test", "test", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);
            card.CustomBorderUrl = customBorderUrl;
            card.Quality = quality;
            card.FromFigure = fromFigure;

            _fileSystemMock
                .Setup(pr => pr.OpenRead(It.IsAny<string>()))
                .Returns(Utils.CreateFakeImage);

            _fileSystemMock
                .Setup(pr => pr.Exists(It.IsAny<string>()))
                .Returns(true);

            var waifuInProfile = await _imageProcessor.GetWaifuInProfileCardAsync(card);
            waifuInProfile.Should().NotBeNull();
        }
    }
}
