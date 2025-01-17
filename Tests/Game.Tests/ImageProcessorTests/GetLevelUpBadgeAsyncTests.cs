﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Game.Services.Abstractions;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests
{
    /// <summary>
    /// Defines tests for <see cref="IImageProcessor.GetLevelUpBadgeAsync(string, ulong, string, Discord.Color)"/> method.
    /// </summary>
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

            var badgeImage = await _imageProcessor.GetLevelUpBadgeAsync(name, level, avatarUrl, color);
            badgeImage.Should().NotBeNull();
        }
    }
}
