using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Diagnostics;
using System;
using Sanakan.ShindenApi.Models;
using Sanakan.Session;
using Moq;
using Sanakan.ShindenApi;
using System.Collections.Generic;
using Sanakan.ShindenApi.Models.Enums;
using System.Linq;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
    [TestClass]
    public class ShowNewEpisodesAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message_Containing_New_Epsiodes()
        {
            await _module.ShowNewEpisodesAsync();
        }
    }
}
