using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    [TestClass]
    public class GetOneFromManyAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message()
        {
            await _module.GetOneFromManyAsync(new[] { "one", "two", "three" });
            _messageChannelMock.Verify();
        }
    }
}
