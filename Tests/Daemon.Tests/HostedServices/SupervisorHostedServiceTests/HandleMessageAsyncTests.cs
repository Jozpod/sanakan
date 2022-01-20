using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Daemon.HostedService;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using Sanakan.DiscordBot.Supervisor;
using Sanakan.Tests.Shared;
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
        public async Task Should_Quit_No_Guild()
        {
            var guildId = 1ul;
            var userId = 1ul;
            var content = "offensive message";
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var notifyChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);

            userMessageMock
                .Setup(pr => pr.Author)
                .Returns(guildUserMock.Object);

            userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object);

            userMessageMock
                .Setup(pr => pr.Content)
                .Returns(content);

            guildUserMock
               .Setup(pr => pr.Id)
               .Returns(userId);

            guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(null as GuildOptions);

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);

            _discordClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMessageMock.Object);
        }

        [TestMethod]
        public async Task Should_Quit_No_Supervision()
        {
            var guildId = 1ul;
            var userId = 1ul;
            var content = "offensive message";
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var notifyChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var guildOptions = new GuildOptions(guildId, 50ul);
            guildOptions.SupervisionEnabled = false;

            userMessageMock
                .Setup(pr => pr.Author)
                .Returns(guildUserMock.Object);

            userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object);

            userMessageMock
                .Setup(pr => pr.Content)
                .Returns(content);

            guildUserMock
               .Setup(pr => pr.Id)
               .Returns(userId);

            guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildOptions);

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);

            _discordClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMessageMock.Object);
        }

        [TestMethod]
        [DataRow(SupervisorAction.Ban)]
        [DataRow(SupervisorAction.Mute)]
        [DataRow(SupervisorAction.Warn)]
        [DataRow(SupervisorAction.None)]
        public async Task Should_Verify_Message_And_Perform_Action(SupervisorAction supervisorAction)
        {
            var guildId = 1ul;
            var userId = 1ul;
            var content = "offensive message";
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var notifyChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var guildOptions = new GuildOptions(guildId, 50ul);
            guildOptions.SupervisionEnabled = true;
            var roleId = guildOptions.MuteRoleId = 1ul;
            var channelId = guildOptions.NotificationChannelId = 1ul;
            var roleIds = new List<ulong>();
            var penaltyInfo = new PenaltyInfo();

            userMessageMock
                .Setup(pr => pr.Author)
                .Returns(guildUserMock.Object);

            userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object);

            userMessageMock
                .Setup(pr => pr.Content)
                .Returns(content);

            guildUserMock
               .Setup(pr => pr.Id)
               .Returns(userId);

            guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildOptions);

            _userMessageSupervisorMock
                .Setup(pr => pr.MakeDecisionAsync(guildId, userId, content, true))
                .ReturnsAsync(supervisorAction)
                .Verifiable();

            guildMock
                .Setup(pr => pr.GetRole(roleId))
                .Returns(roleMock.Object);

            guildMock
                .Setup(pr => pr.GetChannelAsync(channelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(notifyChannelMock.Object)
                .Verifiable();

            guildMock
               .Setup(pr => pr.AddBanAsync(guildUserMock.Object, 1, "Supervisor(ban) spam/flood", null))
               .Returns(Task.CompletedTask);


            messageChannelMock.SetupSendMessageAsync(null);

            _moderatorServiceMock
                .Setup(pr => pr.MuteUserAsync(
                    guildUserMock.Object,
                    roleMock.Object,
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
        }

    }
}
