using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
    [TestClass]
    public class GetSiteStatisticAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Get_Site_Statistics()
        {
            var shindenUserId = 1ul;
            var result = await _module.GetSiteStatisticAsync(shindenUserId, null);
        }
    }
}
