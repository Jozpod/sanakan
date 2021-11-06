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

namespace Sanakan.Game.Tests
{
    [TestClass]
    public class GetWaifuCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Generate_Level_Up_Badge()
        {
            var card = new Card(1, "test", "test", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);

            var badge = await _imageProcessor.GetWaifuCardAsync(card);
            //badge.Save();
        }
    }
}
