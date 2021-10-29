using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Services;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.API.Common;
using Shinden.API;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests
{
    [TestClass]
    public class ImageProcessorTests
    {
        private readonly ImageProcessor _imageProcessor;
        private readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);
        private readonly Mock<IFileSystem> _fileSystemMock = new(MockBehavior.Strict);
        public ImageProcessorTests()
        {
            var httpClient = new HttpClient();

            _imageProcessor = new ImageProcessor(
                _shindenClientMock.Object,
                _fileSystemMock.Object,
                httpClient);
        }

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
