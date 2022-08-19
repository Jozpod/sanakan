using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ServicesTests.ExperienceManagerTests
{
    /// <summary>
    /// Defines tests for <see cref="ExperienceManager.HandleMessageAsync"/> event handler.
    /// </summary>
    [TestClass]
    public class HandleMessageAsyncTests : Base
    {
        protected readonly Mock<IMessage> _messageMock = new(MockBehavior.Strict);
        protected readonly Mock<IMessageChannel> _messageChannelMock = new(MockBehavior.Strict);
        protected readonly Mock<IGuild> _guildMock = new(MockBehavior.Strict);
        protected readonly Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);

        public HandleMessageAsyncTests()
        {
            _messageMock
                .Setup(pr => pr.Content)
                .Returns("message content");

            _messageMock
                .Setup(pr => pr.Tags)
                .Returns(new List<ITag>());

            _messageMock
                .Setup(pr => pr.Author)
                .Returns(_guildUserMock.Object);

            _messageMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _messageChannelMock
               .Setup(pr => pr.Id)
               .Returns(1ul);

            _guildUserMock
               .Setup(pr => pr.Guild)
               .Returns(_guildMock.Object);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(1ul);
        }

        [TestMethod]
        public void Should_Handle_New_Message_Create_User()
        {
            var guildOptions = new GuildOptions(1ul, 50);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            _guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedById(1ul))
                .ReturnsAsync(guildOptions);

            _userRepositoryMock
                .Setup(pr => pr.ExistsByDiscordIdAsync(1ul))
                .ReturnsAsync(false);

            _systemClockMock
                .Setup(pr => pr.StartOfMonth)
                .Returns(DateTime.UtcNow.Date);

            _userRepositoryMock
                .Setup(pr => pr.Add(It.IsAny<User>()));

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, _messageMock.Object);
        }

        [TestMethod]
        public void Should_Quit_No_User_Role()
        {
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var roleMock = new Mock<IRole>(MockBehavior.Strict);
            var guildOptions = new GuildOptions(1ul, 50);
            guildOptions.UserRoleId = 1ul;

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            _guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            guildMock
               .Setup(pr => pr.GetRole(guildOptions.UserRoleId.Value))
               .Returns(roleMock.Object);

            roleMock
               .Setup(pr => pr.Id)
               .Returns(1ul);

            _guildUserMock
               .Setup(pr => pr.RoleIds)
               .Returns(new List<ulong>());

            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, _messageMock.Object);
        }

        [TestMethod]
        public void Should_Not_Calculate_Experience()
        {
            var channelId = 1ul;
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var guildOptions = new GuildOptions(1ul, 50);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            guildOptions.ChannelsWithoutExperience.Add(new WithoutExpChannel { ChannelId = channelId });

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(channelId);

            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, _messageMock.Object);
        }

        [TestMethod]
        public void Should_Not_Count_Messages()
        {
            var channelId = 1ul;
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var guildOptions = new GuildOptions(1ul, 50);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            guildOptions.IgnoredChannels.Add(new WithoutMessageCountChannel { ChannelId = channelId });

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(channelId);

            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, _messageMock.Object);
        }

        [TestMethod]
        public void Should_Enqueue_Add_Experience_Task()
        {
            var guildOptions = new GuildOptions(1ul, 50);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, _messageMock.Object);
            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, _messageMock.Object);
        }
    }
}
