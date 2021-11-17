using Discord;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sanakan.DiscordBot.Session.CraftSession;

namespace Sanakan.DiscordBot.Session.Tests.CraftSessionTests
{
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
            _payload.PlayerInfo.Items = new List<DAL.Models.Item>();
            _payload.Items = new List<DAL.Models.Item>();
            _payload.Items.Add(new DAL.Models.Item());
            _payload.Items.Add(new DAL.Models.Item());

            await _session.ExecuteAsync(context, _serviceProvider);
        }
    }
}
