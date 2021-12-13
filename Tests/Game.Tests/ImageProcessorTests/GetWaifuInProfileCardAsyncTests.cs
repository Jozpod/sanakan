using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using Sanakan.Game.Services.Abstractions;

namespace Sanakan.Game.Tests.ImageProcessorTests
{
    /// <summary>
    /// Defines tests for <see cref="IImageProcessor.GetWaifuInProfileCardAsync(Card)"/> method.
    /// </summary>
#if DEBUG
    [TestClass]
#endif
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
