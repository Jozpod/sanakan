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
    public class MuteUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Success()
        {
            var userMock = new Mock<IGuildUser>();
            var muteRoleMock = new Mock<IRole>();
            var muteModRoleMock = new Mock<IRole>();
            var userRoleMock = new Mock<IRole>();
            var duration = TimeSpan.FromMinutes(1);
            var reason = "reason";
            var modRoles = new[]
            {
                new ModeratorRoles()
            };

            await _moderatorService.MuteUserAsync(
                userMock.Object,
                muteRoleMock.Object,
                muteModRoleMock.Object,
                userRoleMock.Object,
                duration,
                reason,
                modRoles);
        }
    }
}
