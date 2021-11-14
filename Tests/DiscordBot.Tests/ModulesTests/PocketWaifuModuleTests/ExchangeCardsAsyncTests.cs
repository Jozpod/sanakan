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
    public class ExchangeCardsAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Add_To_Wish_List()
        {
       
            await _module.ExchangeCardsAsync();
        }
    }
}
