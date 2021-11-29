using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="FunModule.PlayOnSlotMachineAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class PlayOnSlotMachineAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message()
        {
            await _module.PlayOnSlotMachineAsync();
            _messageChannelMock.Verify();
        }
    }
}
