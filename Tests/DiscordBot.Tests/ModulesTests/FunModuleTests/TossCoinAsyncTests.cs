using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Services;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    [TestClass]
    public class TossCoinAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message()
        {
            var coinSide = CoinSide.Head;
            var amount = 1000;
            await _module.TossCoinAsync(coinSide, amount);
        }
    }
}
