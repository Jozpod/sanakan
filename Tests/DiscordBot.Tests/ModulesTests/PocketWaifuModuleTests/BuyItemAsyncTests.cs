using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using Sanakan.Game.Models;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.BuyItemAsync(int, string)"/> method.
    /// </summary>
    [TestClass]
    public class BuyItemAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Access_Shop()
        {
            _waifuServiceMock
                .Setup(pr => pr.ExecuteShopAsync(ShopType.Normal, _userMock.Object, 0, "0"))
                .ReturnsAsync(new EmbedBuilder().Build());

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.BuyItemAsync();
        }
    }
}
