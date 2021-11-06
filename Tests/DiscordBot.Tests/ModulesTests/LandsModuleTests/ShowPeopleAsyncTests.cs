using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.LandsModuleTests
{
    [TestClass]
    public class ShowPeopleAsync : Base
    {
       
        [TestMethod]
        public async Task Should_Return_Users()
        {
            await _module.ShowPeopleAsync();
        }
    }
}
