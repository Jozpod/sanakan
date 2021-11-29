using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;

namespace DiscordBot.ModulesTests.LandsModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="LandsModule.AddPersonAsync(IGuildUser, string?)"/> method.
    /// </summary>
    [TestClass]
    public class AddPersonAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Tell_When_User_Does_Not_Own_Land()
        {
            await _module.AddPersonAsync(null, null);
        }
    }
}
