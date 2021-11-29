using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DAL.Models;
using Moq;
using System.Collections.Generic;
using Sanakan.DiscordBot.Modules;
using Discord;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ToggleWaifuViewInProfileAsync"/> method.
    /// </summary>
    [TestClass]
    public class ToggleWaifuViewInProfileAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Set_Waifu_In_Profile()
        {

            await _module.ToggleWaifuViewInProfileAsync();
        }
    }
}
