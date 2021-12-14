using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Management;
using Sanakan.DiscordBot.Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ServicesTests.ModeratorServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IModeratorService.UnmuteUserAsync(IGuildUser, IRole, IRole)"/> method.
    /// </summary>
    [TestClass]
    public class UnmuteUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Unmute_User()
        {
            var userId = 1ul;
            var guildId = 1ul;
            var roleId = 1ul;
            var userMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var muteRoleMock = new Mock<IRole>(MockBehavior.Strict);
            var roleIds = new List<ulong>
            {
                roleId,
            };
            var penaltyInfo = new PenaltyInfo();

            userMock
               .Setup(pr => pr.Id)
               .Returns(userId);

            userMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            userMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            userMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            muteRoleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            userMock
                .Setup(pr => pr.RemoveRoleAsync(muteRoleMock.Object, null))
                .Returns(Task.CompletedTask);

            _penaltyInfoRepositoryMock
                 .Setup(pr => pr.GetPenaltyAsync(userId, guildId, PenaltyType.Mute))
                 .ReturnsAsync(penaltyInfo);

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.Remove(It.IsAny<PenaltyInfo>()));

            _penaltyInfoRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                  .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string>()));

            await _moderatorService.UnmuteUserAsync(
                userMock.Object,
                muteRoleMock.Object,
                null);
        }
    }
}
