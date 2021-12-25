using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Daemon.HostedService;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.DiscordBotHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="DiscordBotHostedService.HandleMessageAsync"/> event handler.
    /// </summary>
    [TestClass]
    public class HandleMessageAsyncTests : Base
    {
        protected readonly Mock<IMessage> _messageMock = new(MockBehavior.Strict);

        public async Task SetupAsync(
            GuildOptions guildOptions,
            Mock<IGuild> guildMock = null,
            Mock<IGuildUser> guildUserMock = null,
            Mock<IMessageChannel> messageChannelMock = null)
        {
            await StartAsync();

            guildMock ??= new Mock<IGuild>(MockBehavior.Strict);
            guildUserMock ??= new Mock<IGuildUser>(MockBehavior.Strict);
            messageChannelMock ??= new Mock<IMessageChannel>(MockBehavior.Strict);

            var userId = 1ul;

            _messageMock
                .Setup(pr => pr.Author)
                .Returns(guildUserMock.Object)
                .Verifiable();

            _messageMock
                .Setup(pr => pr.Content)
                .Returns("test message");

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
               .Setup(pr => pr.Guild)
               .Returns(guildMock.Object)
               .Verifiable();

            guildMock
               .Setup(pr => pr.Id)
               .Returns(guildOptions.Id)
               .Verifiable();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildOptions.Id))
                .ReturnsAsync(guildOptions)
                .Verifiable();

            _messageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object);

            messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userRepositoryMock
               .Setup(pr => pr.ExistsByDiscordIdAsync(userId))
               .ReturnsAsync(false)
               .Verifiable();

            _userRepositoryMock
                .Setup(pr => pr.Add(It.IsAny<User>()))
                .Verifiable();

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            _systemClockMock
                .Setup(pr => pr.StartOfMonth)
                .Returns(DateTime.UtcNow);

            _blockingPriorityQueueMock
                .Setup(pr => pr.TryEnqueue(It.IsAny<AddExperienceMessage>()))
                .Returns(true);

            _messageMock
                .Setup(pr => pr.Tags)
                .Returns(new List<ITag>());
        }

        [TestMethod]
        public async Task Should_Handle_New_Message()
        {
            var guildOptions = new GuildOptions(1ul, 50);
            await SetupAsync(guildOptions);
            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, _messageMock.Object);

            _guildConfigRepositoryMock.Verify();
            _userRepositoryMock.Verify();
        }

        [TestMethod]
        public async Task Should_Quit_No_User_Role()
        {
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildOptions = new GuildOptions(1ul, 50);
            guildOptions.UserRoleId = 1ul;
            await SetupAsync(guildOptions, guildMock, guildUserMock);

            guildMock
               .Setup(pr => pr.GetRole(guildOptions.UserRoleId.Value))
               .Returns(roleMock.Object);

            roleMock
               .Setup(pr => pr.Id)
               .Returns(1ul);

            guildUserMock
               .Setup(pr => pr.RoleIds)
               .Returns(new List<ulong>());

            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, _messageMock.Object);
        }

        [TestMethod]
        public async Task Should_Not_Calculate_Experience()
        {
            var channelId = 1ul;
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var guildOptions = new GuildOptions(1ul, 50);
            guildOptions.ChannelsWithoutExperience.Add(new WithoutExpChannel { ChannelId = channelId });
            await SetupAsync(guildOptions, messageChannelMock: messageChannelMock);

            messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(channelId);

            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, _messageMock.Object);
        }

        [TestMethod]
        public async Task Should_Not_Count_Messages()
        {
            var channelId = 1ul;
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var guildOptions = new GuildOptions(1ul, 50);
            guildOptions.IgnoredChannels.Add(new WithoutMessageCountChannel { ChannelId = channelId });
            await SetupAsync(guildOptions, messageChannelMock: messageChannelMock);

            messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(channelId);

            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, _messageMock.Object);
        }

        [TestMethod]
        public async Task Should_Enqueue_Add_Experience_Task()
        {
            var guildOptions = new GuildOptions(1ul, 50);
            await SetupAsync(guildOptions);

            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, _messageMock.Object);
            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, _messageMock.Object);
        }
    }
}
