using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Services;
using Sanakan.ShindenApi;
using Sanakan.Game.Services;
using Shinden.API;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;

namespace Sanakan.Game.Tests.ImageProcessorTests
{
    [TestClass]
    public class GetWaifuCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Generate_Waifu_Card()
        {
            var card = new Card(1, "test", "test", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);

            MockHttpGetImage("TestData/character.png");

            var waifuCard = await _imageProcessor.GetWaifuCardImageAsync(card);
            waifuCard.Should().NotBeNull();

            await ShouldBeEqual("TestData/expected-waifu-card.png", waifuCard);
        }
    }
}
