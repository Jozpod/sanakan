using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using System.Threading.Tasks;
using FluentAssertions;
using DiscordBot.Services;
using System.Linq;

namespace Sanakan.Game.Tests.ImageProcessorTests
{
    [TestClass]
    public class GetFColorsViewTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Color_View()
        {
            var currency = SCurrency.Sc;
            var colours = FColorExtensions.FColors;
            var coloursSummary = colours
                .Select(colour => ($"{colour} ({colour.Price(currency)} {currency.ToString().ToUpper()})", (uint)colour));

            using var image = _imageProcessor.GetFColorsView(coloursSummary);
            image.Should().NotBeNull();
            await ShouldBeEqual("TestData/expected-f-colors.png", image);
        }
    }
}
