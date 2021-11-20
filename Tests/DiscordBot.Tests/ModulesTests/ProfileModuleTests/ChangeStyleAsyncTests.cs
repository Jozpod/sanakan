using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.Common.Models;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    [TestClass]
    public class ChangeStyleAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Give_Bot_Info()
        {
            var profileType = ProfileType.Cards;
            await _module.ChangeStyleAsync(profileType);
        }
    }
}
