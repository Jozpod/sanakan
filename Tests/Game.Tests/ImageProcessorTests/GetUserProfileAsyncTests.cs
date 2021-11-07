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
using Sanakan.ShindenApi.Models;

namespace Sanakan.Game.Tests
{
    [TestClass]
    public class GetUserProfileAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Return_User_Profile_Image()
        {
            var shindenUser = new UserInfo
            {

            };
            var botUser = new User(1, DateTime.Now);
            var avatarUrl = "";
            var topPos = 0l;
            var nickname = "nickname";
            var color = Discord.Color.DarkerGrey;

            var image = await _imageProcessor.GetUserProfileAsync(shindenUser, botUser, avatarUrl, topPos, nickname, color);
            
        }
    }
}