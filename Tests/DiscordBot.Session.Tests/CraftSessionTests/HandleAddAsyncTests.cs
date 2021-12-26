using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot.Session.Abstractions;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session.Tests.CraftSessionTests
{
    /// <summary>
    /// Defines tests for <see cref="CraftSession.HandleAddAsync"/> method.
    /// </summary>
    [TestClass]
    public class HandleAddAsyncTests : Base
    {
        [TestMethod]
        [DataRow("dodaj 1")]
        [DataRow("dodaj 1 1")]
        public async Task Should_Handle_Add_Command_Correctly(string message)
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
                .Returns(message);

            _ownedItems.Add(new DAL.Models.Item());
            _ownedItems.Add(new DAL.Models.Item());

            await _session.ExecuteAsync(context, _serviceProvider);
        }
    }
}
