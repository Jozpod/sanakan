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
using Moq.Protected;
using System.Threading;
using System.Net;
using System.IO;
using FluentAssertions;
using System.Security.Cryptography;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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
            var avatarUrl = "https://test.com/avatar-url";
            var color = Discord.Color.Blue;

            MockHttpGetImage("TestData/card-image.png");

            var badgeImage = await _imageProcessor.GetLevelUpBadgeAsync(name, level, avatarUrl, color);
            badgeImage.Should().NotBeNull();

            await ShouldBeEqual("TestData/expected-badge.png", badgeImage);
        }
    }
}
