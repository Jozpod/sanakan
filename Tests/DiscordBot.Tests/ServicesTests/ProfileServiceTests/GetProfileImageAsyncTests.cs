using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using System.Threading.Tasks;
using Discord.Rest;
using System.IO;
using Sanakan.Preconditions;
using Microsoft.Extensions.DependencyInjection;
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
