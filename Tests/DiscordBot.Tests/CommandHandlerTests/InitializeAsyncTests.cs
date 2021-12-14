using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

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
            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, messageMock.Object);
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
            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMmessageMock.Object);
        }

        [TestMethod]
        public async Task Should_Handle_Command_User_Message_No_Guild_User()
        {
            var userMmessageMock = new Mock<IUserMessage>(MockBehavior.Strict);

            await _commandHandler.InitializeAsync();
            _discordClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMmessageMock.Object);
        }
    }
}
