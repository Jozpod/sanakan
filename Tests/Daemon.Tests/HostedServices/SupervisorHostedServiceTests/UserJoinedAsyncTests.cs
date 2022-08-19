using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Daemon.HostedService;
using Sanakan.DAL.Models.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.SupervisorHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="SupervisorHostedService.UserJoinedAsync(IGuildUser)"/> method.
    /// </summary>
    [TestClass]
    public class UserJoinedAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Verify_User_Join_Ban()
        {
            var userId = 1ul;
            var username = "username";
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var guildOptions = new GuildOptions(1ul, 50ul)
            {
                WaifuConfig = new WaifuConfiguration
                {
                    SpawnChannelId = 1ul,
                    TrashSpawnChannelId = 1ul,
                },
                MuteRoleId = 1ul,
            };
            guildOptions.SupervisionEnabled = true;
            var userIds = new[] { userId };

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            guildMock
                .Setup(pr => pr.GetUserAsync(userId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildUserMock.Object);

            guildMock
                .Setup(pr => pr.AddBanAsync(guildUserMock.Object, 1, It.IsAny<string>(), null))
                .Returns(Task.CompletedTask);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.Username)
                .Returns(username);

            guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedById(guildOptions.Id))
                .ReturnsAsync(guildOptions);

            _userJoinedGuildSupervisorMock
                .Setup(pr => pr.GetUsersToBanCauseRaid(guildOptions.Id, username, userId))
                .Returns(userIds);

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);

            _discordClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordClientAccessorMock.Raise(pr => pr.UserJoined += null, guildUserMock.Object);
        }
    }
}
