using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot.Session.Abstractions;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session.Tests.CraftSessionTests
{
    /// <summary>
    /// Defines tests for <see cref="CraftSession.HandleDeleteAsync"/> method.
    /// </summary>
    [TestClass]
    public class HandleDeleteAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Handle_Remove_Command_Correctly()
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
                .Returns("usun 1");

            var item = new DAL.Models.Item();
            _playerInfo.Items.Add(item);
            _ownedItems.Add(item);

            await _session.ExecuteAsync(context, _serviceProvider);
        }
    }
}
