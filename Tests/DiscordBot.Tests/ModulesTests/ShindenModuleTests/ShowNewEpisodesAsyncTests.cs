using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenModule.ShowNewEpisodesAsync"/> method.
    /// </summary>
    [TestClass]
    public class ShowNewEpisodesAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_New_Epsiodes()
        {
            await _module.ShowNewEpisodesAsync();
        }
    }
}
