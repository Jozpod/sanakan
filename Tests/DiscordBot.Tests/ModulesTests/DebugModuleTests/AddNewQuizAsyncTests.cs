using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    [TestClass]
    public class AddNewQuizAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Add_Quiz_And_Send_Message()
        {
            var json = "test";
            await _module.AddNewQuizAsync(json);
        }
    }
}
