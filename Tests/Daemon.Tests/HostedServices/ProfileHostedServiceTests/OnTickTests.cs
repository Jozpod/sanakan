using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Daemon.HostedService;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.ProfileHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileHostedService.OnTick(object, Common.TimerEventArgs)"/> event handler.
    /// </summary>
    [TestClass]
    public class OnTickTests : Base
    {
        [TestMethod]
        public async Task Should_Catch_Exception()
        {
            _timeStatusRepositoryMock
                .Setup(pr => pr.GetBySubTypeAsync())
                .ThrowsAsync(new Exception());

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _timerMock.Raise(pr => pr.Tick += null, null, new TimerEventArgs(null));
        }

        [TestMethod]
        public async Task Should_Catch_OperationCanceledException()
        {
            _timeStatusRepositoryMock
                .Setup(pr => pr.GetBySubTypeAsync())
                .ThrowsAsync(new OperationCanceledException());

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _timerMock.Raise(pr => pr.Tick += null, null, new TimerEventArgs(null));
        }

        [TestMethod]
        public async Task Should_Remove_Global_Role()
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
                .ReturnsAsync(timeStatuses);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedById(guildId))
                .ReturnsAsync(guildOption);

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
                .Returns(Task.CompletedTask);

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _timerMock.Raise(pr => pr.Tick += null, null, new TimerEventArgs(null));
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task Should_Remove_Color_Role(bool manyUsers)
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
            var roleIds = new List<ulong> { roleId };

            if (manyUsers)
            {
                var anotherUserMock = new Mock<IGuildUser>(MockBehavior.Strict);

                anotherUserMock
                   .Setup(pr => pr.RoleIds)
                   .Returns(roleIds);

                users.Add(anotherUserMock.Object);

                userMock
                    .Setup(pr => pr.RemoveRoleAsync(roleMock.Object, null))
                    .Returns(Task.CompletedTask);
            }

            roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            roleMock
                .Setup(pr => pr.Name)
                .Returns(roleId.ToString());

            roleMock
               .Setup(pr => pr.DeleteAsync(null))
               .Returns(Task.CompletedTask);

            _timeStatusRepositoryMock
                .Setup(pr => pr.GetBySubTypeAsync())
                .ReturnsAsync(timeStatuses);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedById(guildId))
                .ReturnsAsync(guildOption);

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
                .Returns(roleIds);

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _timerMock.Raise(pr => pr.Tick += null, null, new TimerEventArgs(null));
        }
    }
}
