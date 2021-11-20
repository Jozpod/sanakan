using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
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
