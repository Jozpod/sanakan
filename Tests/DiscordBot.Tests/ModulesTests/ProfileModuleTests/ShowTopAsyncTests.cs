using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Diagnostics;
using System;
using Sanakan.DAL.Models;
using Moq;
using DiscordBot.Services;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    [TestClass]
    public class ShowTopAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message_And_Start_Session()
        {
            await _module.ShowTopAsync();
        }
    }
}
