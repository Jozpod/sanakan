using Discord;
using DiscordBot.Services.PocketWaifu;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Session;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.BuyItemPvPAsync(int, string)"/> method.
    /// </summary>
    [TestClass]
    public class BuyItemPvPAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Buy_Item_And_Send_Confirm_Message()
        {
            _waifuServiceMock
              .Setup(pr => pr.ExecuteShopAsync(ShopType.Pvp, _userMock.Object, 0, "0"))
              .ReturnsAsync(new EmbedBuilder().Build());

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.BuyItemPvPAsync();
        }
    }
}
