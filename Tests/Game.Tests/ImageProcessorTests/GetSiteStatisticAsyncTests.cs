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
using Sanakan.ShindenApi.Models;

namespace Sanakan.Game.Tests
{
    [TestClass]
    public class GetSiteStatisticAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Generate_Level_Up_Badge()
        {
            var shindenInfo = new UserInfo
            {

            };
            var color = Discord.Color.DarkPurple;
            var lastRead = new List<LastWatchedRead>
            {

            };
            var lastWatch = new List<LastWatchedRead>
            {

            };
            
            var siteStatistics = await _imageProcessor.GetSiteStatisticAsync(shindenInfo, color, lastRead, lastWatch);
        }
    }
}
