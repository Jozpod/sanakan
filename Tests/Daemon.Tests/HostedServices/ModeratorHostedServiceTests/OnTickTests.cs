using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.ModeratorHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="ModeratorHostedServiceTests.OnTick(object, Common.TimerEventArgs)"/> event handler.
    /// </summary>
    [TestClass]
    public class OnTickTests : Base
    {
        [TestMethod]
        public async Task Should_Remove_Ban()
        {
            var utcNow = DateTime.UtcNow;
            var guildId = 1ul;
            var penalty = new PenaltyInfo
            {
                StartedOn = utcNow.AddMinutes(-10),
                Duration = TimeSpan.FromMinutes(5),
                Type = PenaltyType.Ban,
                UserId = 1ul,
                GuildId = guildId,
            };
            var penalties = new List<PenaltyInfo>
            {
                penalty,
            };

            var guildMock = new Mock<IGuild>();
            var guildUser = null as IGuildUser;
            var banMock = new Mock<IBan>();

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow)
                .Verifiable();

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.GetCachedFullPenalties())
                .ReturnsAsync(penalties)
                .Verifiable();

            _discordClientMock
                .Setup(pr => pr.GetGuildAsync(guildId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildMock.Object)
                .Verifiable();

            guildMock
                .Setup(pr => pr.GetUserAsync(penalty.UserId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildUser)
                .Verifiable();

            guildMock
                .Setup(pr => pr.GetBanAsync(penalty.UserId, null))
                .ReturnsAsync(banMock.Object)
                .Verifiable();

            guildMock
               .Setup(pr => pr.RemoveBanAsync(penalty.UserId, null))
               .Returns(Task.CompletedTask)
               .Verifiable();

            _penaltyInfoRepositoryMock
               .Setup(pr => pr.Remove(penalty))
               .Verifiable();

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string>()))
                .Verifiable();

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _fakeTimer.RaiseTickEvent();

            guildMock.Verify();
            _discordClientMock.Verify();
            _systemClockMock.Verify();
            _penaltyInfoRepositoryMock.Verify();
            _cacheManagerMock.Verify();
        }

        [TestMethod]
        public async Task Should_Mute_User()
        {
            var utcNow = DateTime.UtcNow;
            var guildId = 1ul;
            var penalty = new PenaltyInfo
            {
                StartedOn = utcNow.AddMinutes(-10),
                Duration = TimeSpan.FromMinutes(15),
                Type = PenaltyType.Mute,
                UserId = 1ul,
                GuildId = guildId,
            };
            var penalties = new List<PenaltyInfo>
            {
                penalty,
            };
            var guildOption = new GuildOptions(guildId, 50);
            guildOption.MuteRoleId = 1ul;

            var guildMock = new Mock<IGuild>();
            var roleMock = new Mock<IRole>();
            var guildUserMock = new Mock<IGuildUser>();

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow)
                .Verifiable();

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.GetCachedFullPenalties())
                .ReturnsAsync(penalties)
                .Verifiable();

            _discordClientMock
                .Setup(pr => pr.GetGuildAsync(guildId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildMock.Object)
                .Verifiable();

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId)
                .Verifiable();

            guildMock
                .Setup(pr => pr.GetUserAsync(penalty.UserId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildUserMock.Object)
                .Verifiable();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildOption)
                .Verifiable();

            guildMock
                .Setup(pr => pr.GetRole(0ul))
                .Returns(null as IRole)
                .Verifiable();

            guildMock
                .Setup(pr => pr.GetRole(guildOption.MuteRoleId))
                .Returns(roleMock.Object)
                .Verifiable();

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _fakeTimer.RaiseTickEvent();

            guildMock.Verify();
            _discordClientMock.Verify();
            _systemClockMock.Verify();
            _penaltyInfoRepositoryMock.Verify();
            _cacheManagerMock.Verify();
        }

        [TestMethod]
        public async Task Should_Unmute_User()
        {
            var utcNow = DateTime.UtcNow;
            var guildId = 1ul;
            var penalty = new PenaltyInfo
            {
                StartedOn = utcNow.AddMinutes(-10),
                Duration = TimeSpan.FromMinutes(5),
                Type = PenaltyType.Mute,
                UserId = 1ul,
                GuildId = guildId,
            };
            var penalties = new List<PenaltyInfo>
            {
                penalty,
            };
            var guildOption = new GuildOptions(guildId, 50);
            guildOption.MuteRoleId = 1ul;

            var guildMock = new Mock<IGuild>();
            var roleMock = new Mock<IRole>();
            var guildUserMock = new Mock<IGuildUser>();
            var userRoles = new List<ulong>
            {
                guildOption.MuteRoleId,
            };

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow)
                .Verifiable();

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.GetCachedFullPenalties())
                .ReturnsAsync(penalties)
                .Verifiable();

            _discordClientMock
                .Setup(pr => pr.GetGuildAsync(guildId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildMock.Object)
                .Verifiable();

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId)
                .Verifiable();

            guildMock
                .Setup(pr => pr.GetUserAsync(penalty.UserId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildUserMock.Object)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(userRoles)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.RemoveRoleAsync(roleMock.Object, null))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _penaltyInfoRepositoryMock
               .Setup(pr => pr.Remove(penalty))
               .Verifiable();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildOption)
                .Verifiable();

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            guildMock
                .Setup(pr => pr.GetRole(0ul))
                .Returns(null as IRole)
                .Verifiable();

            guildMock
                .Setup(pr => pr.GetRole(guildOption.MuteRoleId))
                .Returns(roleMock.Object)
                .Verifiable();

            roleMock
                .Setup(pr => pr.Id)
                .Returns(guildOption.MuteRoleId)
                .Verifiable();

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string>()))
                .Verifiable();

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _fakeTimer.RaiseTickEvent();

            roleMock.Verify();
            guildMock.Verify();
            guildUserMock.Verify();
            _discordClientMock.Verify();
            _systemClockMock.Verify();
            _penaltyInfoRepositoryMock.Verify();
            _cacheManagerMock.Verify();
        }
    }
}
