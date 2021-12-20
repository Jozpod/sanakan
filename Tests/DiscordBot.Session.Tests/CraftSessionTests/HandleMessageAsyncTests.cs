using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session.Tests.CraftSessionTests
{
    /// <summary>
    /// Defines tests for <see cref="CraftSession.HandleMessageAsync"/> method.
    /// </summary>
    [TestClass]
    public class HandleMessageAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Exit_When_No_PlayerInfo()
        {
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
            };

            _userMessageMock
                .SetupSequence(pr => pr.Id)
                .Returns(1ul)
                .Returns(1ul);

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Exit_When_Same_Message()
        {
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
            };

            _userMessageMock
                .SetupSequence(pr => pr.Id)
                .Returns(1ul)
                .Returns(1ul);

            _payload.Message = _userMessageMock.Object;
            _payload.PlayerInfo = new Game.Models.PlayerInfo();

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Exit_When_Wrong_Channel()
        {
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
            };

            _userMessageMock
                .SetupSequence(pr => pr.Id)
                .Returns(1ul)
                .Returns(2ul);

            _messageChannelMock
                .SetupSequence(pr => pr.Id)
                .Returns(1ul)
                .Returns(1ul);

            _payload.Message = _userMessageMock.Object;
            _payload.PlayerInfo = new Game.Models.PlayerInfo();

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Exit_When_Empty_Message()
        {
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
            };

            _userMessageMock
                .SetupSequence(pr => pr.Id)
                .Returns(1ul)
                .Returns(2ul);

            _messageChannelMock
                .SetupSequence(pr => pr.Id)
                .Returns(1ul)
                .Returns(2ul);

            _payload.Message = _userMessageMock.Object;
            _payload.PlayerInfo = new Game.Models.PlayerInfo();

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Exit_When_Invalid_Message()
        {
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
            };

            _userMessageMock
                .SetupSequence(pr => pr.Id)
                .Returns(1ul)
                .Returns(2ul);

            _messageChannelMock
                .SetupSequence(pr => pr.Id)
                .Returns(1ul)
                .Returns(2ul);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns(" ");

            _payload.Message = _userMessageMock.Object;
            _payload.PlayerInfo = new Game.Models.PlayerInfo();

            await _session.ExecuteAsync(context, _serviceProvider);
        }
    }
}
