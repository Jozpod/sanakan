using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Diagnostics;
using System;
using Sanakan.DAL.Models;
using Moq;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    [TestClass]
    public class ShowExpeditionStatusAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message_Containing_Expedition_Status()
        {
  
            await _module.ShowExpeditionStatusAsync();
        }
    }
}
