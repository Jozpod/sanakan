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
using Sanakan.Common.Models;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    [TestClass]
    public class ChangeStyleAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Give_Bot_Info()
        {
            var profileType = ProfileType.Cards;
            await _module.ChangeStyleAsync(profileType);
        }
    }
}
