using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    [TestClass]
    public class ShowUserAvatarAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Give_Bot_Info()
        {
            await _module.ShowUserAvatarAsync();
        }
    }
}
