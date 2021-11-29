using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="FunModule.SlotMachineSettingsAsync"/> method.
    /// </summary>
    [TestClass]
    public class SlotMachineSettingsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message()
        {
            await _module.SlotMachineSettingsAsync();
            _messageChannelMock.Verify();
        }
    }
}
