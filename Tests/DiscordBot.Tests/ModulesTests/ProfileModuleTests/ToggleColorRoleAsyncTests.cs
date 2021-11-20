using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DAL.Models;
using DiscordBot.Services;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    [TestClass]
    public class ToggleColorRoleAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Toggle_Color_Role()
        {
            var color = FColor.AgainBlue;
            var currency = SCurrency.Tc;
            await _module.ToggleColorRoleAsync(color, currency);
        }
    }
}
