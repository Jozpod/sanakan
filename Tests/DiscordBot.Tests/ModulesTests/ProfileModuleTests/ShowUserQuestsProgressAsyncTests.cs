using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DAL.Models;
using Moq;
using System.Collections.Generic;
using Sanakan.DiscordBot.Modules;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ShowUserQuestsProgressAsync(bool)"/> method.
    /// </summary>
    [TestClass]
    public class ShowUserQuestsProgressAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_Quest_Progress_And_Claim_Gifts()
        {
            var claimGifts = true;
            await _module.ShowUserQuestsProgressAsync(claimGifts);
        }
    }
}
