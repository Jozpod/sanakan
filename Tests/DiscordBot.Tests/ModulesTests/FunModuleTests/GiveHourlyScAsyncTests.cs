using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="FunModule.GiveHourlyScAsync"/> method.
    /// </summary>
    [TestClass]
    public class GiveHourlyScAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message()
        {
            await _module.GiveHourlyScAsync();
            _messageChannelMock.Verify();
        }
    }
}
