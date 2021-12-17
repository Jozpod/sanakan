using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Daemon.HostedService;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using Sanakan.DiscordBot.Supervisor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.SupervisorHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="SupervisorHostedService.HandleMessageAsync(IMessage)"/> event handler.
    /// </summary>
    [TestClass]
    public class HandleMessageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Verify_Message_Mute()
        {
            var guildId = 1ul;
            var userId = 1ul;
            var content = "offensive message";
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var notifyChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var guildOptions = new GuildOptions(guildId, 50ul);
            guildOptions.SupervisionEnabled = true;
            var roleId = guildOptions.MuteRoleId = 1ul;
            var channelId = guildOptions.NotificationChannelId = 1ul;
            var userIds = Enumerable.Empty<ulong>();
            var roleIds = new List<ulong>();
            var penaltyInfo = new PenaltyInfo();

            userMessageMock
                .Setup(pr => pr.Author)
                .Returns(guildUserMock.Object)
                .Verifiable();

            userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object)
                .Verifiable();

            userMessageMock
                .Setup(pr => pr.Content)
                .Returns(content)
                .Verifiable();

            guildUserMock
               .Setup(pr => pr.Id)
               .Returns(userId)
               .Verifiable();

            guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds)
                .Verifiable();

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object)
                .Verifiable();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildOptions);

            _userMessageSupervisorMock
                .Setup(pr => pr.MakeDecisionAsync(guildId, userId, content, true))
                .ReturnsAsync(SupervisorAction.Mute)
                .Verifiable();

            guildMock
                .Setup(pr => pr.GetRole(roleId))
                .Returns<IRole?>(null)
                .Verifiable();

            guildMock
                .Setup(pr => pr.GetChannelAsync(channelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(notifyChannelMock.Object)
                .Verifiable();

            _moderatorServiceMock
                .Setup(pr => pr.MuteUserAsync(
                    guildUserMock.Object,
                    null,
                    null,
                    null,
                    TimeSpan.FromDays(1),
                    "spam/flood",
                    null))
                .ReturnsAsync(penaltyInfo);

            _moderatorServiceMock
                .Setup(pr => pr.NotifyAboutPenaltyAsync(
                    guildUserMock.Object,
                    notifyChannelMock.Object,
                    penaltyInfo,
                    DiscordBot.Constants.Automatic))
                .Returns(Task.CompletedTask);

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);

            _discordClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMessageMock.Object);

            _userMessageSupervisorMock.Verify();
            guildUserMock.Verify();
            userMessageMock.Verify();
        }

        [TestMethod]
        public async Task Should_Verify_Message_Ban()
        {
            var guildId = 1ul;
            var userId = 1ul;
            var content = "offensive message";
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var notifyChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var guildOptions = new GuildOptions(guildId, 50ul);
            guildOptions.SupervisionEnabled = true;
            var roleId = guildOptions.MuteRoleId = 1ul;
            var channelId = guildOptions.NotificationChannelId = 1ul;
            var userIds = Enumerable.Empty<ulong>();
            var roleIds = new List<ulong>();
            var penaltyInfo = new PenaltyInfo();

            userMessageMock
                .Setup(pr => pr.Author)
                .Returns(guildUserMock.Object)
                .Verifiable();

            userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object)
                .Verifiable();

            userMessageMock
                .Setup(pr => pr.Content)
                .Returns(content)
                .Verifiable();

            guildUserMock
               .Setup(pr => pr.Id)
               .Returns(userId)
               .Verifiable();

            guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds)
                .Verifiable();

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object)
                .Verifiable();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildOptions);

            _userMessageSupervisorMock
                .Setup(pr => pr.MakeDecisionAsync(guildId, userId, content, true))
                .ReturnsAsync(SupervisorAction.Ban)
                .Verifiable();

            guildMock
                .Setup(pr => pr.GetRole(roleId))
                .Returns<IRole?>(null)
                .Verifiable();

            guildMock
                .Setup(pr => pr.GetChannelAsync(channelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(notifyChannelMock.Object)
                .Verifiable();

            guildMock
               .Setup(pr => pr.AddBanAsync(guildUserMock.Object, 1, "Supervisor(ban) spam/flood", null))
               .Returns(Task.CompletedTask)
               .Verifiable();

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);

            _discordClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMessageMock.Object);

            guildMock.Verify();
            _userMessageSupervisorMock.Verify();
            guildUserMock.Verify();
            userMessageMock.Verify();
        }

    }
}
