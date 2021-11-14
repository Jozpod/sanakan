using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Discord.Rest;
using System.IO;
using Sanakan.DiscordBot.Modules;

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
