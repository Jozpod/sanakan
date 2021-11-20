using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.LandsModuleTests
{
    [TestClass]
    public class RemovePersonAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Remove_Person()
        {
          
            await _module.RemovePersonAsync(null);
        }
    }
}
