using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot.Session.Abstractions;
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

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("dodaj ")]
        public async Task Should_React_And_Exit_When_Invalid_Message(string content)
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
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns(content);

            await _session.ExecuteAsync(context, _serviceProvider);
        }
    }
}
