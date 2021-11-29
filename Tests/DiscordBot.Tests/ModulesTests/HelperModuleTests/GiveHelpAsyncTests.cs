using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="HelperModule.GiveHelpAsync(string?)"/> method.
    /// </summary>
    [TestClass]
    public class GiveHelpAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Help_Command_Message_Public()
        {
            await _module.GiveHelpAsync();
        }

        [TestMethod]
        public async Task Should_Send_Help_Command_Message_Specific()
        {
            await _module.GiveHelpAsync();
        }
    }
}
