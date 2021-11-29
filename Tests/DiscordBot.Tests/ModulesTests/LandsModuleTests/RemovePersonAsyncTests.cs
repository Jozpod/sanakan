using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;

namespace DiscordBot.ModulesTests.LandsModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="LandsModule.RemovePersonAsync(IGuildUser, string?)"/> method.
    /// </summary>
    [TestClass]
    public class RemovePersonAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Remove_Person()
        {
            await _module.RemovePersonAsync(null);
        }
    }
}
