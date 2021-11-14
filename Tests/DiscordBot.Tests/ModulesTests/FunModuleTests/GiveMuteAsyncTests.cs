using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Diagnostics;
using System;
using Moq;
using Sanakan.ShindenApi.Models;
using Sanakan.DAL.Models;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    [TestClass]
    public class GiveMuteAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message()
        {
            await _module.GiveMuteAsync();
            _messageChannelMock.Verify();
        }
    }
}
