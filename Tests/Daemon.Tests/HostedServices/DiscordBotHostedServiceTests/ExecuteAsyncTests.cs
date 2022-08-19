using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Daemon.HostedService;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.DiscordBotHostedServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="DiscordBotHostedService.ExecuteAsync"/> method.
    /// </summary>
    [TestClass]
    public class ExecuteAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Register_Discord_Command_Modules_And_Start_Bot()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            _databaseFacadeMock
                .Setup(pr => pr.EnsureCreatedAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .Verifiable();

            var fakeDirectory = new System.IO.DirectoryInfo(Path.GetDirectoryName(typeof(ExecuteAsyncTests).Assembly.Location)!);

            _fileSystemMock
                .Setup(pr => pr.CreateDirectory(It.IsAny<string>()))
                .Returns(fakeDirectory)
                .Verifiable();

            _discordClientAccessorMock
                .Setup(pr => pr.LoginAsync(TokenType.Bot, It.IsAny<string>(), true))
                .Returns(Task.CompletedTask);

            _discordClientAccessorMock
                .Setup(pr => pr.SetGameAsync(It.IsAny<string>(), null, ActivityType.Playing))
                .Returns(Task.CompletedTask);

            _discordClientMock
                .Setup(pr => pr.StartAsync())
                .Returns(Task.CompletedTask);

            _commandHandlerMock
                .Setup(pr => pr.InitializeAsync())
                .Returns(Task.CompletedTask);

            _taskManagerMock
                .Setup(pr => pr.Delay(
                    It.IsAny<TimeSpan>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.StartAsync(cancellationTokenSource.Token);
        }
    }
}