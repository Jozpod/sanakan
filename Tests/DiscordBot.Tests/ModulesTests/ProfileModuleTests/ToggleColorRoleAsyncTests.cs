using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DAL.Models;
using DiscordBot.Services;
using Sanakan.DAL.Models;
using Moq;
using System.Collections.Generic;
using Sanakan.DiscordBot.Modules;
using Discord;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ToggleColorRoleAsync(FColor, SCurrency)"/> method.
    /// </summary>
    [TestClass]
    public class ToggleColorRoleAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Toggle_Color_Role()
        {
            var color = FColor.AgainBlue;
            var currency = SCurrency.Tc;
            await _module.ToggleColorRoleAsync(color, currency);
        }
    }
}
