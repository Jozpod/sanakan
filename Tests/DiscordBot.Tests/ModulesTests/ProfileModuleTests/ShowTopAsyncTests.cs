using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;
using Sanakan.Game.Models;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ShowTopAsync(TopType)"/> method.
    /// </summary>
    [TestClass]
    public class ShowTopAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_And_Start_Session()
        {
            await _module.ShowTopAsync();
        }
    }
}
