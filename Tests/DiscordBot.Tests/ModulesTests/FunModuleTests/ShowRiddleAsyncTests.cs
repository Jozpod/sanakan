using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="FunModule.ShowRiddleAsync"/> method.
    /// </summary>
    [TestClass]
    public class ShowRiddleAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message()
        {
            await _module.ShowRiddleAsync();
            _messageChannelMock.Verify();
        }
    }
}
