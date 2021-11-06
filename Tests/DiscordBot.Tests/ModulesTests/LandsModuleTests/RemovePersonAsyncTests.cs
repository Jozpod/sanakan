using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Discord.Rest;
using System.IO;
using Sanakan.DiscordBot.Modules;

namespace DiscordBot.ModulesTests.LandsModuleTests
{
    [TestClass]
    public class RemovePersonAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Remove_Person()
        {
          
            await _module.RemovePersonAsync(null);
        }
    }
}
