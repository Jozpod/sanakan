using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.ModeratorHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="ModeratorHostedService.OnTick(object, Common.TimerEventArgs)"/> event handler.
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
            var guildOption = new GuildOptions(guildId, 50);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var guildUser = null as IGuildUser;
            var banMock = new Mock<IBan>(MockBehavior.Strict);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.GetCachedFullPenalties())
                .ReturnsAsync(penalties);

            _discordClientMock
                .Setup(pr => pr.GetGuildAsync(guildId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildMock.Object);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildOption);

            guildMock
                .Setup(pr => pr.GetRole(It.IsAny<ulong>()))
                .Returns<IRole?>(null);

            guildMock
                .Setup(pr => pr.GetUserAsync(penalty.UserId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildUser);

            guildMock
                .Setup(pr => pr.GetBanAsync(penalty.UserId, null))
                .ReturnsAsync(banMock.Object);

            guildMock
               .Setup(pr => pr.RemoveBanAsync(penalty.UserId, null))
               .Returns(Task.CompletedTask);

            _penaltyInfoRepositoryMock
               .Setup(pr => pr.Remove(penalty));

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _timerMock.Raise(pr => pr.Tick += null, null, new TimerEventArgs(null)); ;
        }

        [TestMethod]
        public async Task Should_Mute_Mod_User()
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
            guildOption.ModMuteRoleId = 1ul;
            guildOption.ModeratorRoles.Add(new ModeratorRoles { RoleId = guildOption.ModMuteRoleId });
            penalty.Roles.Add(new OwnedRole { RoleId = guildOption.ModMuteRoleId });
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.GetCachedFullPenalties())
                .ReturnsAsync(penalties);

            _discordClientMock
                .Setup(pr => pr.GetGuildAsync(guildId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildMock.Object);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            guildMock
                .Setup(pr => pr.GetUserAsync(penalty.UserId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildUserMock.Object);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildOption);

            guildMock
                .Setup(pr => pr.GetRole(0ul))
                .Returns<IRole?>(null);

            guildMock
                .Setup(pr => pr.GetRole(guildOption.ModMuteRoleId))
                .Returns(roleMock.Object);

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(new List<ulong>());

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            roleMock
                .Setup(pr => pr.Id)
                .Returns(guildOption.ModMuteRoleId);

            guildUserMock
               .Setup(pr => pr.AddRoleAsync(roleMock.Object, null))
               .Returns(Task.CompletedTask);

            guildUserMock
                .Setup(pr => pr.RemoveRoleAsync(guildOption.ModMuteRoleId, null))
                .Returns(Task.CompletedTask);

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _timerMock.Raise(pr => pr.Tick += null, null, new TimerEventArgs(null));
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
                .Returns<IRole?>(null)
                .Verifiable();

            guildMock
                .Setup(pr => pr.GetRole(guildOption.MuteRoleId))
                .Returns(roleMock.Object)
                .Verifiable();

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _timerMock.Raise(pr => pr.Tick += null, null, new TimerEventArgs(null));
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
                .Returns(utcNow);

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.GetCachedFullPenalties())
                .ReturnsAsync(penalties);

            _discordClientMock
                .Setup(pr => pr.GetGuildAsync(guildId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildMock.Object);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            guildMock
                .Setup(pr => pr.GetUserAsync(penalty.UserId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildUserMock.Object);

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(userRoles);

            guildUserMock
                .Setup(pr => pr.RemoveRoleAsync(roleMock.Object, null))
                .Returns(Task.CompletedTask);

            _penaltyInfoRepositoryMock
               .Setup(pr => pr.Remove(penalty));

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildOption);

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            guildMock
                .Setup(pr => pr.GetRole(0ul))
                .Returns<IRole?>(null);

            guildMock
                .Setup(pr => pr.GetRole(guildOption.MuteRoleId))
                .Returns(roleMock.Object);

            roleMock
                .Setup(pr => pr.Id)
                .Returns(guildOption.MuteRoleId);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _timerMock.Raise(pr => pr.Tick += null, null, new TimerEventArgs(null));
        }

        [TestMethod]
        public async Task Should_Unmute_Mod_User()
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
            var ownerRoleId = 2ul;
            var guildOption = new GuildOptions(guildId, 50);
            guildOption.ModMuteRoleId = 1ul;
            penalty.Roles.Add(new OwnedRole { RoleId = ownerRoleId });
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var roleIds = new List<ulong>
            {
                guildOption.ModMuteRoleId,
            };

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.GetCachedFullPenalties())
                .ReturnsAsync(penalties);

            _discordClientMock
                .Setup(pr => pr.GetGuildAsync(guildId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildMock.Object);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            guildMock
                .Setup(pr => pr.GetUserAsync(penalty.UserId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildUserMock.Object);

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            guildUserMock
                .Setup(pr => pr.RemoveRoleAsync(roleMock.Object, null))
                .Returns(Task.CompletedTask);

            _penaltyInfoRepositoryMock
               .Setup(pr => pr.Remove(penalty));

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildOption);

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            guildMock
              .Setup(pr => pr.GetRole(0ul))
              .Returns<IRole?>(null);

            guildMock
                .Setup(pr => pr.GetRole(ownerRoleId))
                .Returns(roleMock.Object);

            roleMock
                .SetupSequence(pr => pr.Id)
                .Returns(guildOption.ModMuteRoleId)
                .Returns(ownerRoleId)
                .Returns(ownerRoleId);

            guildMock
                .Setup(pr => pr.GetRole(guildOption.ModMuteRoleId))
                .Returns(roleMock.Object);

            guildUserMock
               .Setup(pr => pr.AddRoleAsync(ownerRoleId, null))
               .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);
            _timerMock.Raise(pr => pr.Tick += null, null, new TimerEventArgs(null));
        }
    }
}
