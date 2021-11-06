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
    public class AddPersonAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Tell_When_User_Does_Not_Own_Land()
        {
            await _module.AddPersonAsync(null, null);
        }
    }
}
