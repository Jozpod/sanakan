using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Diagnostics;
using System;
using Moq;
using Sanakan.ShindenApi.Models;
using Sanakan.DAL.Models;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    [TestClass]
    public class GiveUserScAsyncTests : Base
    {
        private readonly Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);

        [TestMethod]
        public async Task Should_Give_Sc_And_Send_Confirm_Message()
        {
            var value = 1000u;
            await _module.GiveUserScAsync(_guildUserMock.Object, value);
        }
    }
}
