using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.RemoveCardTagAsync(string, ulong[])"/> method.
    /// </summary>
    [TestClass]
    public class RemoveCardTagAsyncTests : Base
    {    
        [TestMethod]
        public async Task Should_Remove_Card_Tag()
        {
            var tag = "test tag";
            await _module.RemoveCardTagAsync(tag);
        }
    }
}
