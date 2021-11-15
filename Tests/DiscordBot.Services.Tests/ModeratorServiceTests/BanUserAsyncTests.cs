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

namespace DiscordBot.ServicesTests.ModeratorServiceTests
{
    [TestClass]
    public class BanUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Ban_User_And_Return_Penalty_Info()
        {
            var guildMock = new Mock<IGuildUser>();
            var duration = TimeSpan.FromMinutes(1);

            var result = await _moderatorService.BanUserAysnc(guildMock.Object, duration);
            result.Should().NotBeNull();
        }
    }
}
