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
    public class SetRaportChannelAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Set_Report_Channel_And_Send_Confirm_Message()
        {
           
            await _module.SetRaportChannelAsync();
        }
    }
}
