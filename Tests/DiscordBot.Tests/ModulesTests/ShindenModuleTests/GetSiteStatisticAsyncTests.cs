using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenModule.GetSiteStatisticAsync(ulong, IGuildUser)"/> method.
    /// </summary>
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
