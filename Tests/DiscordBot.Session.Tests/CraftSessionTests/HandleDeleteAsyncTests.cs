﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
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

            _payload.Message = _userMessageMock.Object;
            _payload.PlayerInfo = new Game.Models.PlayerInfo();
            var item = new DAL.Models.Item();
            _payload.PlayerInfo.Items.Add(item);
            _payload.Items.Add(item);

            await _session.ExecuteAsync(context, _serviceProvider);
        }
    }
}
