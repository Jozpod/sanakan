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
    /// Defines tests for <see cref="ProfileModule.ShowWalletAsync(IUser?)"/> method.
    /// </summary>
    [TestClass]
    public class ShowWalletAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_Wallet_Info()
        {
            await _module.ShowWalletAsync();
        }
    }
}
