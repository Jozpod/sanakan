using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Modules;
using Sanakan.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Rest;
using System.IO;
using Sanakan.DiscordBot.Modules;

namespace DiscordBot.ModulesTests.LandsModuleTests
{
    [TestClass]
    public class Base : Base
    {
        
        [TestMethod]
        public async Task Should_Tell_When_User_Does_Not_Own_Land()
        {
            await _module.AddPersonAsync(null, null);
        }
    }
}
