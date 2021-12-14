using Discord;
using Discord.Commands;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Management;
using Sanakan.DiscordBot.Services.Abstractions;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ServicesTests.ModeratorServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IModeratorService.GetMutedListAsync(ICommandContext)"/> method.
    /// </summary>
    [TestClass]
    public class GetMutedListAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Embeds_Containing_Muted_Users()
        {
            var userId = 1ul;
            var guildId = 1ul;
            var commandContextMock = new Mock<ICommandContext>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var duration = TimeSpan.FromMinutes(1);
            var muted = new[]
            {
                new PenaltyInfo()
                {
                    StartedOn = DateTime.UtcNow,
                    UserId = userId,
                    GuildId = guildId,
                    Duration = TimeSpan.FromHours(1),
                }
            };

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            commandContextMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.GetMutedPenaltiesAsync(guildId))
                .ReturnsAsync(muted);

            guildMock
               .Setup(pr => pr.GetUserAsync(userId, CacheMode.AllowDownload, null))
               .ReturnsAsync(guildUserMock.Object);

            guildUserMock
               .Setup(pr => pr.Mention)
               .Returns("mention");

            var result = await _moderatorService.GetMutedListAsync(commandContextMock.Object);
            result.Should().NotBeNull();
            result.Description.Should().NotBeNullOrEmpty();
        }
    }
}
