using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Sanakan.DAL.Models;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.SimulateExpeditionAsync(ulong, ExpeditionCardType, int)"/> method.
    /// </summary>
    [TestClass]
    public class SimulateExpeditionAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Simulate_Expedition_And_Send_Outcome_Message()
        {
            var waifuId = 1ul;
            await _module.SimulateExpeditionAsync(waifuId);
        }
    }
}
