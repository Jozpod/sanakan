using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="HelperModule.ReportUserAsync(ulong, string)"/> method.
    /// </summary>
    [TestClass]
    public class ReportUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Report_User()
        {
            var messageId = 1ul;
            var reason = "reason";
            await _module.ReportUserAsync(messageId, reason);
        }
    }
}
