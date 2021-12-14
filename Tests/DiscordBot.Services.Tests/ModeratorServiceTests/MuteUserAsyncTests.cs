using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using Sanakan.DiscordBot.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ServicesTests.ModeratorServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IModeratorService.MuteUserAsync(IGuildUser, IRole?, IRole?, IRole?, TimeSpan, string, System.Collections.Generic.IEnumerable{ModeratorRoles}?)"/> method.
    /// </summary>
    [TestClass]
    public class MuteUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Mute_User_And_Return_Penalty_Details()
        {
            var userId = 1ul;
            var guildId = 1ul;
            var roleId = 1ul;
            var userMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var muteRoleMock = new Mock<IRole>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var duration = TimeSpan.FromMinutes(1);
            var reason = "reason";
            var modRoles = new[]
            {
                new ModeratorRoles()
            };
            var roleIds = new List<ulong>();

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            userMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            userMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            userMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            userMock
                .Setup(pr => pr.AddRoleAsync(muteRoleMock.Object, null))
                .Returns(Task.CompletedTask);

            muteRoleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.Add(It.IsAny<PenaltyInfo>()));

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string>()));

            var penalty = await _moderatorService.MuteUserAsync(
                userMock.Object,
                muteRoleMock.Object,
                null,
                null,
                duration,
                reason,
                modRoles);
            penalty.Should().NotBeNull();
        }
    }
}
