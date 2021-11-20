using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Web.HostedService;
using System.Threading.Tasks;
using System.Reflection;
using Discord.WebSocket;
using Sanakan.Tests.Shared;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Session;
using System;
using Discord.Commands;
using System.Threading;

namespace Sanakan.Web.Tests.HostedServices.SessionHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="SessionHostedService.HandleMessageAsync(IMessage)"/> event handler.
    /// </summary>
    [TestClass]
    public class HandleMessageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Run_Session()
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

            userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(socketMessageChannel.Object);

            var cancellationTokenSource = new CancellationTokenSource();
            await _service.StartAsync(cancellationTokenSource.Token);

            _discordClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMessageMock.Object);
        }

    }
}
