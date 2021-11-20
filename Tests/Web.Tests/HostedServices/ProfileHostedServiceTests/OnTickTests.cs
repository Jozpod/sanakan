using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Web.HostedService;
using System.Threading.Tasks;
using System.Reflection;
using Discord.WebSocket;
using Sanakan.Tests.Shared;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Session;
using System;
using Discord.Commands;
using System.Threading;
using Sanakan.Common;
using Sanakan.DAL.Models;
using System.Collections.Generic;
using Sanakan.DAL.Models.Configuration;
using System.Linq;

namespace Sanakan.Web.Tests.HostedServices.ProfileHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileHostedService.OnTick(object, Common.TimerEventArgs)"/> event handler.
    /// </summary>
    [TestClass]
    public class OnTickTests : Base
    {
        [TestMethod]
        public async Task Should_Remove_Role()
        {
            var guildId = 1ul;
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var userMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var timeStatus = new TimeStatus(StatusType.Globals, guildId);
            var userId = timeStatus.UserId = 1ul;
            var timeStatuses = new List<TimeStatus>
            {
                timeStatus
            };
            var guildOption = new GuildOptions(guildId, 50);
            guildOption.GlobalEmotesRoleId = 1ul;

            _timeStatusRepositoryMock
                .Setup(pr => pr.GetBySubTypeAsync())
                .ReturnsAsync(timeStatuses)
                .Verifiable();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildOption)
                .Verifiable();

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            _discordClientMock
                .Setup(pr => pr.GetGuildAsync(guildId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildMock.Object);

            guildMock
                .Setup(pr => pr.GetRole(guildOption.GlobalEmotesRoleId))
                .Returns(roleMock.Object);

            guildMock
                .Setup(pr => pr.GetUserAsync(userId, CacheMode.AllowDownload, null))
                .ReturnsAsync(userMock.Object);

            userMock
                .Setup(pr => pr.RemoveRoleAsync(roleMock.Object, null))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _timerMock.Raise(pr => pr.Tick += null, null, new TimerEventArgs(null));

            _timeStatusRepositoryMock.Verify();
            _guildConfigRepositoryMock.Verify();
            userMock.Verify();
        }


        [TestMethod]
        public async Task Should_Remove_Color_Role()
        {
            var guildId = 1ul;
            var roleId = 32767ul;
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var userMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var timeStatus = new TimeStatus(StatusType.Color, guildId);
            var userId = timeStatus.UserId = 1ul;
            var timeStatuses = new List<TimeStatus>
            {
                timeStatus
            };
            var guildOption = new GuildOptions(guildId, 50);
            guildOption.GlobalEmotesRoleId = roleId;
            var roles = new List<IRole>
            {
                roleMock.Object
            };
            var users = new List<IGuildUser>()
            {
                userMock.Object,
            };

            roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            roleMock
                .Setup(pr => pr.Name)
                .Returns(roleId.ToString())
                .Verifiable();

            roleMock
               .Setup(pr => pr.DeleteAsync(null))
               .Returns(Task.CompletedTask)
               .Verifiable();

            _timeStatusRepositoryMock
                .Setup(pr => pr.GetBySubTypeAsync())
                .ReturnsAsync(timeStatuses)
                .Verifiable();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildOption)
                .Verifiable();

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            _discordClientMock
                .Setup(pr => pr.GetGuildAsync(guildId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildMock.Object);

            guildMock
                .Setup(pr => pr.GetUserAsync(userId, CacheMode.AllowDownload, null))
                .ReturnsAsync(userMock.Object);

            guildMock
                .Setup(pr => pr.GetUsersAsync(CacheMode.AllowDownload, null))
                .ReturnsAsync(users);

            guildMock
                .Setup(pr => pr.Roles)
                .Returns(roles);

            userMock
                .Setup(pr => pr.RoleIds)
                .Returns(roles.Select(pr => pr.Id).ToList())
                .Verifiable();

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _timerMock.Raise(pr => pr.Tick += null, null, new TimerEventArgs(null));

            roleMock.Verify();
        }
    }
}
