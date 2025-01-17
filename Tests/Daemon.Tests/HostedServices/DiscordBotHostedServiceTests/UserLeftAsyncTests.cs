﻿using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.TaskQueue.Messages;
using Sanakan.Tests.Shared;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.DiscordBotHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="DiscordBotHostedService.UserLeftAsync"/> event handler.
    /// </summary>
    [TestClass]
    public class UserLeftAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Handle_User_Left()
        {
            await StartAsync();
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var guildChannelMock = new Mock<IGuildChannel>(MockBehavior.Strict);
            var messageChannelMock = guildChannelMock.As<IMessageChannel>();
            var dmChannelMock = new Mock<IDMChannel>(MockBehavior.Strict);
            var guildId = 1ul;
            var guildOptions = new GuildOptions(guildId, 50);
            guildOptions.GoodbyeMessage = "test";

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            guildUserMock
               .Setup(pr => pr.Guild)
               .Returns(guildMock.Object);

            guildUserMock
               .Setup(pr => pr.Mention)
               .Returns("mention");

            guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedById(guildId))
                .ReturnsAsync(guildOptions);

            guildMock
                .Setup(pr => pr.GetChannelAsync(guildOptions.GreetingChannelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildChannelMock.Object);

            messageChannelMock.SetupSendMessageAsync(null);

            guildUserMock
                .Setup(pr => pr.CreateDMChannelAsync(null))
                .ReturnsAsync(dmChannelMock.Object);

            dmChannelMock.SetupSendMessageAsync(null);

            dmChannelMock
                .Setup(pr => pr.CloseAsync(null))
                .Returns(Task.CompletedTask);

            _blockingPriorityQueueMock
                .Setup(pr => pr.TryEnqueue(It.IsAny<DeleteUserMessage>()))
                .Returns(true);

            _discordClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordClientAccessorMock.Raise(pr => pr.UserLeft += null, guildMock.Object, guildUserMock.Object);
        }
    }
}