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
using Shinden.Models;

namespace Sanakan.Game.Tests
{
    [TestClass]
    public class GetCatchThatWaifuImageTests : Base
    {
        [TestMethod]
        public async Task Should_Generate_Level_Up_Badge()
        {
            var shindenInfo = new UserInfo
            {

            };
            var color = Discord.Color.DarkPurple;
            var lastRead = new List<ILastReaded>
            {

            };
            var lastWatch = new List<ILastWatched>
            {

            };
            
            var siteStatistics = await _imageProcessor.GetCatchThatWaifuImage(shindenInfo, color, lastRead, lastWatch);
        }
    }
}
