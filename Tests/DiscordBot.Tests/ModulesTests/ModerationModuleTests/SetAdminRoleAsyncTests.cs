using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.SetAdminRoleAsync(IRole)"/> method.
    /// </summary>
    [TestClass]
    public class SetAdminRoleAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Roles()
        {
            
            await _module.SetAdminRoleAsync(null);
        }
    }
}
