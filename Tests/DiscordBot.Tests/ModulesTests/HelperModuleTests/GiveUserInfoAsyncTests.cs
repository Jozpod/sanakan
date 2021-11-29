using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="HelperModule.GetServerInfoAsync"/> method.
    /// </summary>
    [TestClass]
    public class GiveUserInfoAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Give_Bot_Info()
        {
            await _module.GiveUserInfoAsync();
        }
    }
}
