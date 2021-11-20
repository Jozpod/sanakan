using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.TaskQueue.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.HostedServices.DiscordBotHostedServiceTests
{
    [TestClass]
    public class HandleMessageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Handle_New_Message()
        {
            await StartAsync();

            var messageMock = new Mock<IMessage>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);

            var guildId = 1ul;
            var userId = 1ul;
            var guildOptions = new GuildOptions(guildId, 50);

            var guildUserMock = userMock.As<IGuildUser>();

            messageMock
                .Setup(pr => pr.Author)
                .Returns(userMock.Object)
                .Verifiable();

            messageMock
                .Setup(pr => pr.Content)
                .Returns("test message");

            userMock
                .Setup(pr => pr.Id)
                .Returns(userId)
                .Verifiable();

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(false)
                .Verifiable();

            userMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false)
                .Verifiable();

            guildUserMock
               .Setup(pr => pr.Guild)
               .Returns(guildMock.Object)
               .Verifiable();

            guildMock
               .Setup(pr => pr.Id)
               .Returns(guildId)
               .Verifiable();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildOptions)
                .Verifiable();

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

            _blockingPriorityQueueMock
                .Setup(pr => pr.TryEnqueue(It.IsAny<BaseMessage>()))
                .Returns(true);

            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, messageMock.Object);

            _guildConfigRepositoryMock.Verify();
            _userRepositoryMock.Verify();
        }

    }
}
