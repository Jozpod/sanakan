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
    public class ChangeCardAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Change_Card()
        {
            var waifuId = 1ul;
            await _module.ChangeCardAsync(waifuId);
        }
    }
}
