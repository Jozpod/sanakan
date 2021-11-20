using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using FluentAssertions;
using System;
using Sanakan.DAL.Models;

namespace DiscordBot.ServicesTests.ProfileServiceTests
{
    [TestClass]
    public class GetProfileImageAsync : Base
    {
        [TestMethod]
        public async Task Should_Return_Profile_Image()
        {
            var guildUserMock = new Mock<IGuildUser>();
            var databaseUser = new User(1ul, DateTime.UtcNow);
            var topPosition = 1;
            var result = await _profileService.GetProfileImageAsync(guildUserMock.Object, databaseUser, topPosition);
            result.Should().NotBeNull();
        }
    }
}
