using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.SetUserRoleAsync(IRole)"/> method.
    /// </summary>
    [TestClass]
    public class SetUserRoleAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Set_User_User_And_Send_Confirm_Message()
        {
            _helperServiceMock
                .Setup(pr => pr.GivePrivateHelp(PrivateModules.Moderation))
                .Returns("test info");
            
            await _module.SetUserRoleAsync(null);
        }
    }
}
