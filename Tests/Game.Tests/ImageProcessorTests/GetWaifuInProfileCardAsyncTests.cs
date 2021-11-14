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
using System.Collections.Generic;
using FluentAssertions;

namespace Sanakan.Game.Tests.ImageProcessorTests
{
    [TestClass]
    public class GetWaifuInProfileCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Generate_Waifu_In_Profile()
        {
            var card = new Card(1, "test", "test", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);

            MockHttpGetImage("TestData/character.png");

            var waifuInProfile = await _imageProcessor.GetWaifuInProfileCardAsync(card);
            waifuInProfile.Should().NotBeNull();

            await ShouldBeEqual("TestData/expected-profile-card.png", waifuInProfile);
        }
    }
}
