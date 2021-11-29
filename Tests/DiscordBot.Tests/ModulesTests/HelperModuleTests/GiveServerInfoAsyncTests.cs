using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="HelperModule.GetServerInfoAsync"/> method.
    /// </summary>
    [TestClass]
    public class GiveServerInfoAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Give_Bot_Info()
        {
            _helperServiceMock
                .Setup(pr => pr.GetInfoAboutServerAsync(_guildMock.Object));

            await _module.GetServerInfoAsync();
        }
    }
}
