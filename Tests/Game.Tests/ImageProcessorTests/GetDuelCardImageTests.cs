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
using Sanakan.Services.PocketWaifu;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Sanakan.Game.Tests
{
    [TestClass]
    public class GetDuelCardImageTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Duel_Card_Image()
        {
            var duelInfo = new DuelInfo();
            var duelImage = new DuelImage();
            var win = new Image<Rgba32>(400, 400);
            var los = new Image<Rgba32>(400, 400);

            var duelCardImage = _imageProcessor.GetDuelCardImage(duelInfo, duelImage, win, los);
            //badge.Save();
        }
    }
}
