using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    [TestClass]
    public class ReportUserAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Give_Bot_Info()
        {
            var messageId = 1ul;
            var reason = "reason";
            await _module.ReportUserAsync(messageId, reason);
        }
    }
}
