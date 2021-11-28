using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Daemon.HostedService;
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
using Sanakan.DAL.Models.Configuration;
using System.Linq;

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
            var guildId = 1ul;
            var userId = 1ul;
            var username = "username";
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var guildOptions = new GuildOptions(guildId, 50ul)
            {
                WaifuConfig = new WaifuConfiguration
                {
                    SpawnChannelId = 1ul,
                    TrashSpawnChannelId = 1ul,
                },
                MuteRoleId = 1ul,
            };
            var userIds = Enumerable.Empty<ulong>();

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(userId)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.Username)
                .Returns(username)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object)
                .Verifiable();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildOptions);

            _userJoinedGuildSupervisorMock
                .Setup(pr => pr.GetUsersToBanCauseRaid(guildId, username, userId))
                .Returns(userIds);

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);

            _discordClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordClientAccessorMock.Raise(pr => pr.UserJoined += null, guildUserMock.Object);
        }

    }
}
