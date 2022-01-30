using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Daemon.HostedService;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.DiscordBotHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="DiscordBotHostedService.BotLeftGuildAsync(IGuild)"/> event handler.
    /// </summary>
    [TestClass]
    public class BotLeftGuildAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Handle_Bot_Leaving_Guild()
        {
            await StartAsync();

            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var guildId = 1ul;
            var guildOptions = new GuildOptions(guildId, 50);
            var timeStatuses = new List<TimeStatus>();
            var penaltyInfos = new List<PenaltyInfo>();

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId)
                .Verifiable();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetOrCreateAsync(guildId))
                .ReturnsAsync(guildOptions)
                .Verifiable();

            _guildConfigRepositoryMock
                .Setup(pr => pr.Remove(It.IsAny<GuildOptions>()))
                .Verifiable();

            _timeStatusRepositoryMock
                .Setup(pr => pr.GetByGuildIdAsync(guildId))
                .ReturnsAsync(timeStatuses)
                .Verifiable();

            _timeStatusRepositoryMock
                .Setup(pr => pr.RemoveRange(It.IsAny<IEnumerable<TimeStatus>>()))
                .Verifiable();

            _timeStatusRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.GetByGuildIdAsync(guildId))
                .ReturnsAsync(penaltyInfos)
                .Verifiable();

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.RemoveRange(It.IsAny<IEnumerable<PenaltyInfo>>()))
                .Verifiable();

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _discordClientAccessorMock.Raise(pr => pr.LeftGuild += null, guildMock.Object);

            _guildConfigRepositoryMock.Verify();
            _timeStatusRepositoryMock.Verify();
            _penaltyInfoRepositoryMock.Verify();
        }

    }
}
