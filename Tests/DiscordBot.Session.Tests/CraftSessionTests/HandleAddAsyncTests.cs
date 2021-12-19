﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
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
        public async Task Should_Handle_Add_Command_Correctly()
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
                .Returns("dodaj 1");

            _payload.Message = _userMessageMock.Object;
            _payload.PlayerInfo = new Game.Models.PlayerInfo();
            _payload.PlayerInfo.Items = new List<DAL.Models.Item>();
            _payload.Items = new List<DAL.Models.Item>();
            _payload.Items.Add(new DAL.Models.Item());
            _payload.Items.Add(new DAL.Models.Item());

            await _session.ExecuteAsync(context, _serviceProvider);
        }
    }
}