using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.Common.Models;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;
using Sanakan.DAL.Models;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ChangeStyleAsync(ProfileType, string?, SCurrency)"/> method.
    /// </summary>
    [TestClass]
    public class ChangeStyleAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Change_Style()
        {
            var profileType = ProfileType.Cards;
            await _module.ChangeStyleAsync(profileType);
        }
    }
}
