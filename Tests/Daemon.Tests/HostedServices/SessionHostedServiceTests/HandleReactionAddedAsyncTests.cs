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

namespace Sanakan.Daemon.Tests.HostedServices.SessionHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="SessionHostedService.HandleReactionAddedAsync(Cacheable{IUserMessage, ulong}, IMessageChannel, IReaction)"/> event handler.
    /// </summary>
    [TestClass]
    public class HandleReactionAddedAsyncTests : Base
    {
        [TestMethod]
        public void Should_Run_Session()
        {
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var socketMessageChannel = new Mock<ISocketMessageChannel>(MockBehavior.Strict);
            var userId = 1ul;
            var sessionMock = new Mock<IInteractionSession>(MockBehavior.Strict);
            var sessions = new[]
            {
                sessionMock.Object
            };

            var cachedMessage = CacheableExtensions.CreateCacheable(userMessageMock.Object, 1ul);
            var reaction = ReactionExtensions.CreateReaction(
                socketMessageChannel.Object,
                1ul,
                Optional<SocketUserMessage>.Unspecified,
                userId,
                Optional<IUser>.Unspecified,
                Emotes.GreenChecked);

            var utcNow = DateTime.UtcNow;

            userMock
                 .Setup(pr => pr.Id)
                 .Returns(userId);

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            userMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            socketMessageChannel
                .Setup(pr => pr.GetUserAsync(
                    userId,
                    It.IsAny<CacheMode>(),
                    It.IsAny<RequestOptions>()))
                .ReturnsAsync(userMock.Object);

            userMessageMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _sessionManagerMock
                .Setup(pr => pr.GetByOwnerId(userId, SessionExecuteCondition.ReactionAdded))
                .Returns(sessions);

            userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(socketMessageChannel.Object);

            socketMessageChannel
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
                .Returns(Task.CompletedTask)
                .Verifiable();

            _sessionManagerMock
                .Setup(pr => pr.Remove(sessionMock.Object));

            sessionMock
                .Setup(pr => pr.DisposeAsync())
                .Returns(ValueTask.CompletedTask);

            _discordClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordClientAccessorMock.Raise(pr => pr.ReactionAdded += null, cachedMessage, socketMessageChannel.Object, reaction);

            sessionMock.Verify();
        }

    }
}
