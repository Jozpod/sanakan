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
    public class UnmuteUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Success()
        {
            var userMock = new Mock<IGuildUser>();
            var muteRoleMock = new Mock<IRole>();
            var muteModRoleMock = new Mock<IRole>();

            await _moderatorService.UnmuteUserAsync(
                userMock.Object,
                muteRoleMock.Object,
                muteModRoleMock.Object);
        }
    }
}
