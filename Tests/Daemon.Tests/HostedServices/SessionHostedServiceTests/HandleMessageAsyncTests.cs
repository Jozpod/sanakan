using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Daemon.HostedService;
using Sanakan.DiscordBot.Session.Abstractions;
using Sanakan.TaskQueue.Messages;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.SessionHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="SessionHostedService.HandleMessageAsync(IMessage)"/> event handler.
    /// </summary>
    [TestClass]
    public class HandleMessageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Quit_Not_User_Message()
        {
            var messageMock = new Mock<IMessage>(MockBehavior.Strict);
            var sessionMock = new Mock<IInteractionSession>(MockBehavior.Strict);
            var userId = 1ul;
            var sessions = Enumerable.Empty<IInteractionSession>();
            var utcNow = DateTime.UtcNow;

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            messageMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);

            _discordClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, messageMock.Object);
        }

        [TestMethod]
        public async Task Should_Quit_No_Sessions()
        {
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var userId = 1ul;
            var sessions = Enumerable.Empty<IInteractionSession>();
            var utcNow = DateTime.UtcNow;

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            userMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            userMessageMock
                .Setup(pr => pr.Author)
                .Returns(userMock.Object);

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            userMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            _sessionManagerMock
                .Setup(pr => pr.GetByOwnerId(userId, SessionExecuteCondition.Message))
                .Returns(sessions);

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);

            _discordClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMessageMock.Object);
        }

        [TestMethod]
        public async Task Should_Handle_Message_Expired_Session()
        {
            var socketMessageChannel = new Mock<ISocketMessageChannel>(MockBehavior.Strict);
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var sessionMock = new Mock<IInteractionSession>(MockBehavior.Strict);
            var userId = 1ul;
            var sessions = new[]
            {
                sessionMock.Object
            };
            var utcNow = DateTime.UtcNow;

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            userMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            userMessageMock
                .Setup(pr => pr.Author)
                .Returns(userMock.Object);

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            userMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            _sessionManagerMock
                .Setup(pr => pr.GetByOwnerId(userId, SessionExecuteCondition.Message))
                .Returns(sessions);

            userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(socketMessageChannel.Object);

            sessionMock
                .Setup(pr => pr.HasExpired(utcNow))
                .Returns(true);

            sessionMock
                .Setup(pr => pr.IsRunning)
                .Returns(false);

            sessionMock
                .SetupSet(pr => pr.ServiceProvider = It.IsAny<IServiceProvider>());

            _sessionManagerMock
                .Setup(pr => pr.Remove(sessionMock.Object));

            sessionMock
                .Setup(pr => pr.DisposeAsync())
                .Returns(ValueTask.CompletedTask);

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);

            _discordClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMessageMock.Object);
        }

        [TestMethod]
        public async Task Should_Run_Session_Async()
        {
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var sessionMock = new Mock<IInteractionSession>(MockBehavior.Strict);
            var socketMessageChannel = new Mock<ISocketMessageChannel>(MockBehavior.Strict);
            var userId = 1ul;
            var sessions = new[]
            {
                sessionMock.Object
            };
            var utcNow = DateTime.UtcNow;

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            userMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            userMessageMock
                .Setup(pr => pr.Author)
                .Returns(userMock.Object);

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            userMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            _sessionManagerMock
                .Setup(pr => pr.GetByOwnerId(userId, SessionExecuteCondition.Message))
                .Returns(sessions);

            sessionMock
                .Setup(pr => pr.HasExpired(utcNow))
                .Returns(false);

            sessionMock
                .Setup(pr => pr.RunMode)
                .Returns(RunMode.Async);

            sessionMock
                .Setup(pr => pr.ExecuteAsync(
                    It.IsAny<SessionContext>(),
                    It.IsAny<IServiceProvider>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _sessionManagerMock
                .Setup(pr => pr.Remove(sessionMock.Object));

            sessionMock
                .Setup(pr => pr.DisposeAsync())
                .Returns(ValueTask.CompletedTask);

            userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(socketMessageChannel.Object);

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);

            _discordClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMessageMock.Object);
        }

        [TestMethod]
        public async Task Should_Run_Session_Sync()
        {
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var sessionMock = new Mock<IInteractionSession>(MockBehavior.Strict);
            var socketMessageChannel = new Mock<ISocketMessageChannel>(MockBehavior.Strict);
            var userId = 1ul;
            var sessions = new[]
            {
                sessionMock.Object
            };
            var utcNow = DateTime.UtcNow;

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            userMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            userMessageMock
                .Setup(pr => pr.Author)
                .Returns(userMock.Object);

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            userMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            _sessionManagerMock
                .Setup(pr => pr.GetByOwnerId(userId, SessionExecuteCondition.Message))
                .Returns(sessions);

            sessionMock
                .Setup(pr => pr.HasExpired(utcNow))
                .Returns(false);

            sessionMock
                .Setup(pr => pr.RunMode)
                .Returns(RunMode.Sync);

            sessionMock
                .Setup(pr => pr.ExecuteAsync(
                    It.IsAny<SessionContext>(),
                    It.IsAny<IServiceProvider>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _sessionManagerMock
                .Setup(pr => pr.Remove(sessionMock.Object));

            sessionMock
                .Setup(pr => pr.DisposeAsync())
                .Returns(ValueTask.CompletedTask);

            userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(socketMessageChannel.Object);

            _blockingPriorityQueueMock
                .Setup(pr => pr.TryEnqueue(It.IsAny<SessionMessage>()))
                .Returns(true);

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);

            _discordClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMessageMock.Object);
        }

    }
}
