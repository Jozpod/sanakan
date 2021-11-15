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
    public class ToggleWaifuViewInProfileAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Set_Waifu_In_Profile()
        {

            await _module.ToggleWaifuViewInProfileAsync();
        }
    }
}
