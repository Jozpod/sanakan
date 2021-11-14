using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Diagnostics;
using System;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    [TestClass]
    public class ReportUserAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Give_Bot_Info()
        {
            await _module.ReportUserAsync();
        }
    }
}
