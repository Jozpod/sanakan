using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.OpenPacketAsync(IGuildUser)"/> method.
    /// </summary>
    [TestClass]
    public class OpenPacketAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Open_Packet()
        {
       
            await _module.OpenPacketAsync();
        }
    }
}
