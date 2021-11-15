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
    public class NotifyUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Notify_User()
        {
            var userMock = new Mock<IUser>();
            var duration = TimeSpan.FromMinutes(1);
            var reason = "reason";

            await _moderatorService.NotifyUserAsync(userMock.Object, reason);
        }
    }
}
