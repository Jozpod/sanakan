using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Moq;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    [TestClass]
    public class ShowRolesAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Return_Roles()
        {
            
            await _module.ShowRolesAsync();
        }
    }
}
