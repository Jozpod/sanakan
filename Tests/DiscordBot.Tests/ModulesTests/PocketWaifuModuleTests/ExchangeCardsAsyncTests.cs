using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.ExchangeCardsAsync(IGuildUser)"/> method.
    /// </summary>
    [TestClass]
    public class ExchangeCardsAsyncTests : Base
    {
        private readonly Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);

        [TestMethod]
        public async Task Should_Add_To_Wish_List()
        {
            await _module.ExchangeCardsAsync(_guildUserMock.Object);
        }
    }
}
