using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.ChangeCardAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class ChangeCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Change_Card()
        {
            var waifuId = 1ul;
            await _module.ChangeCardAsync(waifuId);
        }
    }
}
