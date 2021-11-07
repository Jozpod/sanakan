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
    public class GiveBotInfoAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Give_Bot_Info()
        {
            var process = Process.GetCurrentProcess();

            _operatingSystemMock
                .Setup(pr => pr.GetCurrentProcess())
                .Returns(process);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.Now);

            await _module.GiveBotInfoAsync();
        }
    }
}
