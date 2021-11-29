using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.AddGlobalEmotesAsync"/> method.
    /// </summary>
    [TestClass]
    public class AddGlobalEmotesAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Give_Bot_Info()
        {

            await _module.AddGlobalEmotesAsync();
        }
    }
}
