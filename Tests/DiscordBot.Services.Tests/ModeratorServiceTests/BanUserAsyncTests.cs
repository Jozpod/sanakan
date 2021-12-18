using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Management;
using Sanakan.DiscordBot.Services.Abstractions;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Services.Tests.ModeratorServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IModeratorService.BanUserAysnc(IGuildUser, TimeSpan, string)"/> method.
    /// </summary>
    [TestClass]
    public class BanUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Ban_User_And_Return_Penalty_Info()
        {
            var userId = 1ul;
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var duration = TimeSpan.FromMinutes(1);
            var utcNow = DateTime.UtcNow;

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            guildMock
                .Setup(pr => pr.AddBanAsync(guildUserMock.Object, 0, It.IsAny<string>(), null))
                .Returns(Task.CompletedTask);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.Add(It.IsAny<PenaltyInfo>()));

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string>()));

            var result = await _moderatorService.BanUserAysnc(guildUserMock.Object, duration);
            result.Should().NotBeNull();
            result.Duration.Should().Be(duration);
        }
    }
}
