using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Services;
using Sanakan.ShindenApi;
using Sanakan.Game.Services;
using Shinden.API;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using FluentAssertions;

namespace Sanakan.Game.Tests.ImageProcessorTests
{
    [TestClass]
    public class GetCatchThatWaifuImageTests : Base
    {
        [TestMethod]
        public void Should_Return_Waifu_Image()
        {
            var card = new Image<Rgba32>(100, 100);
            var imageUrl = "TestData/card-image.png";
            var xPos = 0;
            var yPos = 0;

            using var image = _imageProcessor.GetCatchThatWaifuImage(card, imageUrl, xPos, yPos);
            image.Should().NotBeNull();
        }
    }
}
