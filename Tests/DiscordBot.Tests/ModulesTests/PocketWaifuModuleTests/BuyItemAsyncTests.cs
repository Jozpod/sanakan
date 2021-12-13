using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;
using FluentAssertions;
using DiscordBot.Services.PocketWaifu;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.BuyItemAsync(int, string)"/> method.
    /// </summary>
    [TestClass]
    public class BuyItemAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Execute_Shop()
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
