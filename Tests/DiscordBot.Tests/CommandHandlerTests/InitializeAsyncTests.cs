using Discord;
using Discord.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Sanakan.Preconditions;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DAL.Models.Configuration;
using System.Collections.Generic;

namespace Sanakan.DiscordBot.Tests.CommandHandlerTests
{
    /// <summary>
    /// Defines tests for <see cref="ICommandHandler.InitializeAsync"/> method.
    /// </summary>
    [TestClass]
    public class InitializeAsyncTests : TestBase
    {
        [TestMethod]
        public async Task Should_Initialize_Correctly()
        {
            await _commandHandler.InitializeAsync();
        }

        [TestMethod]
        public async Task Should_Handle_Command_Not_User_Message()
        {
            var messageMock = new Mock<IMessage>(MockBehavior.Strict);

            await _commandHandler.InitializeAsync();
            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, messageMock.Object);
        }

        [TestMethod]
        public async Task Should_Handle_Command_User_Message_Bot()
        {
            var userMmessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);

            userMmessageMock
                .Setup(pr => pr.Author)
                .Returns(userMock.Object);

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(true);

            await _commandHandler.InitializeAsync();
            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMmessageMock.Object);
        }

        [TestMethod]
        public async Task Should_Handle_Command_User_Message_No_Guild_User()
        {
            var userMmessageMock = new Mock<IUserMessage>(MockBehavior.Strict);

            await _commandHandler.InitializeAsync();
            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMmessageMock.Object);
        }
    }
}
