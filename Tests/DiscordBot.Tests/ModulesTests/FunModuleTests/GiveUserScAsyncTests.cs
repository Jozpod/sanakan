using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Moq;
using Sanakan.DiscordBot.Modules;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="FunModule.GiveUserScAsync(IGuildUser, uint)"/> method.
    /// </summary>
    [TestClass]
    public class GiveUserScAsyncTests : Base
    {
        private readonly Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);

        [TestMethod]
        public async Task Should_Give_Sc_And_Send_Confirm_Message()
        {
            var value = 1000u;
            await _module.GiveUserScAsync(_guildUserMock.Object, value);
        }
    }
}
