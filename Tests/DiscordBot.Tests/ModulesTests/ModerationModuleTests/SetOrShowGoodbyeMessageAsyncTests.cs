using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Diagnostics;
using System;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    [TestClass]
    public class SetOrShowGoodbyeMessageAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message()
        {
            _helperServiceMock
                .Setup(pr => pr.GivePrivateHelp("Moderacja"))
                .Returns("test info");
            
            await _module.SetOrShowGoodbyeMessageAsync();
        }
    }
}
