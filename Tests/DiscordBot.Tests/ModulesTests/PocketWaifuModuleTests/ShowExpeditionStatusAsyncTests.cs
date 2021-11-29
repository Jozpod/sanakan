using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.ShowExpeditionStatusAsync"/> method.
    /// </summary>
    [TestClass]
    public class ShowExpeditionStatusAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_Expedition_Status()
        {
  
            await _module.ShowExpeditionStatusAsync();
        }
    }
}
