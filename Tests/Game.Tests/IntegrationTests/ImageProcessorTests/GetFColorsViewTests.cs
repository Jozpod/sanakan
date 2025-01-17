﻿using DiscordBot.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.IntegrationTests.ImageProcessorTests
{
    /// <summary>
    /// Defines integration tests for <see cref="IImageProcessor.GetFColorsView(System.Collections.Generic.IEnumerable{(string, uint)})"/> method.
    /// </summary>
#if DEBUG
    [TestClass]
#endif
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

            //await SaveImageAsync(image, "actual-f-colors.png");
            await ShouldBeEqual("expected-f-colors.png", image);
        }
    }
}
