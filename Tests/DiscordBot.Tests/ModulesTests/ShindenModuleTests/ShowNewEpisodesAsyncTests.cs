using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
    [TestClass]
    public class ShowNewEpisodesAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message_Containing_New_Epsiodes()
        {
            await _module.ShowNewEpisodesAsync();
        }
    }
}
