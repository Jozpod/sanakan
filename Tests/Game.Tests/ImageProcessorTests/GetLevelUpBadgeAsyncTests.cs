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
    public class GetLevelUpBadgeAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Generate_Level_Up_Badge()
        {
            var name = "test-user";
            var level = 20ul;
            var avatarUrl = "avatar-url";
            var color = Discord.Color.Blue;

            var badge = await _imageProcessor.GetLevelUpBadgeAsync(name, level, avatarUrl, color);
            //badge.Save();
        }

        [TestMethod]
        public async Task Should_Generate_User_Profile()
        {
            var shindenUser = new UserInfo
            {

            };
            var databaseUser = new User(1, DateTime.UtcNow);
            var avatarUrl = "avatar-url";
            var color = Discord.Color.Blue;
            var topPosition = 10;
            var nickname = "nickname";

            var test = await _imageProcessor.GetUserProfileAsync(
                shindenUser,
                databaseUser,
                avatarUrl,
                topPosition,
                nickname,
                color);
        }
    }
}
