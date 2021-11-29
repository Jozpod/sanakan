using Microsoft.VisualStudio.TestTools.UnitTesting;
using Discord;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.GenerateLevelUpBadgeAsync(IGuildUser?)"/> method.
    /// </summary>
    [TestClass]
    public class GenerateLevelUpBadgeAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Send_Message()
        {
            await _module.GenerateLevelUpBadgeAsync();
            _messageChannelMock.Verify();
        }
    }
}
