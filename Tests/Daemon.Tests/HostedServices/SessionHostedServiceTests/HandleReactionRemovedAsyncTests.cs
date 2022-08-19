using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Daemon.HostedService;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Session.Abstractions;
using Sanakan.Tests.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.SessionHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="SessionHostedService.HandleReactionRemovedAsync(Cacheable{IUserMessage, ulong}, IMessageChannel, IReaction)"/> event handler.
    /// </summary>
    [TestClass]
    public class HandleReactionRemovedAsyncTests : Base
    {
        [TestMethod]
        public void Should_Run_Session()
        {
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var userId = 1ul;
            var sessionMock = new Mock<IInteractionSession>(MockBehavior.Strict);
            var sessions = new[]
            {
                sessionMock.Object
            };
            var utcNow = DateTime.UtcNow;

            var cachedMessage = CacheableExtensions.CreateCacheable(userMessageMock.Object, 1ul);
            var cachedChannel = CacheableExtensions.CreateCacheable(messageChannelMock.Object, 1ul);
            var reaction = ReactionExtensions.CreateReaction(userId, Emotes.GreenChecked);

            userMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            userMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            messageChannelMock
                .Setup(pr => pr.GetUserAsync(
                    userId,
                    It.IsAny<CacheMode>(),
                    It.IsAny<RequestOptions>()))
                .ReturnsAsync(userMock.Object);

            userMessageMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _sessionManagerMock
                .Setup(pr => pr.GetByOwnerId(userId, SessionExecuteCondition.ReactionRemoved))
                .Returns(sessions);

            userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object);

            messageChannelMock
                .Setup(pr => pr.GetMessageAsync(1ul, CacheMode.AllowDownload, null))
                .ReturnsAsync(userMessageMock.Object);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            sessionMock
               .Setup(pr => pr.HasExpired(utcNow))
               .Returns(false)
               .Verifiable();

            sessionMock
                .Setup(pr => pr.RunMode)
                .Returns(RunMode.Async)
                .Verifiable();

            sessionMock
                .Setup(pr => pr.ExecuteAsync(
                    It.IsAny<SessionContext>(),
                    It.IsAny<IServiceProvider>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false)
                .Verifiable();

            _sessionManagerMock
                .Setup(pr => pr.Remove(sessionMock.Object));

            sessionMock
                .Setup(pr => pr.DisposeAsync())
                .Returns(ValueTask.CompletedTask);

            _discordClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordClientAccessorMock.Raise(pr => pr.ReactionRemoved += null, cachedMessage, cachedChannel, reaction);

            sessionMock.Verify();
        }
    }
}