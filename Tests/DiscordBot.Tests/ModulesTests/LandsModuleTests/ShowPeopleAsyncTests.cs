using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.LandsModuleTests
{
    [TestClass]
    public class Base1 : Base
    {
       
        [TestMethod]
        public async Task Should_Tell_When_User_Does_Not_Own_Land()
        {
            await _module.ShowPeopleAsync();
        }
    }
}
