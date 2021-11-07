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

namespace Sanakan.Game.Tests
{
    [TestClass]
    public class GetCatchThatWaifuImageTests : Base
    {
        [TestMethod]
        public void Should_Return_Waifu_Image()
        {
            var card = new Image<Rgba32>(100, 100);
            var pokeImg = "";
            var xPos = 0;
            var yPos = 0;


            var siteStatistics = _imageProcessor.GetCatchThatWaifuImage(card, pokeImg, xPos, yPos);
        }
    }
}
