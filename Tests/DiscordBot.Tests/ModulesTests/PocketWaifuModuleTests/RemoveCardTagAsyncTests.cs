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
    public class RemoveCardTagAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Remove_Card_Tag()
        {
            var tag = "test tag";
            await _module.RemoveCardTagAsync(tag);
        }
    }
}
