using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="HelperModule.GetPingAsync(string?)"/> method.
    /// </summary>
    [TestClass]
    public class GivePingAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Give_Bot_Info()
        {
            await _module.GetPingAsync();
        }
    }
}
