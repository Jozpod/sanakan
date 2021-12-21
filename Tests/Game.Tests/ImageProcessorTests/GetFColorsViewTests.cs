using DiscordBot.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Moq.Protected;
using System.Net.Http;
using Moq;
using System.Threading;
using System.Net;

namespace Sanakan.Game.Tests.ImageProcessorTests
{
    /// <summary>
    /// Defines tests for <see cref="IImageProcessor.GetFColorsView(IEnumerable{(string, uint)})"/> method.
    /// </summary>
    [TestClass]
    public class GetFColorsViewTests : Base
    {
        [TestMethod]
        public void Should_Return_Color_View()
        {
            var currency = SCurrency.Sc;
            var colours = FColorExtensions.FColors;
            var coloursSummary = colours
                .Select(colour => ($"{colour} ({colour.Price(currency)} {currency.ToString().ToUpper()})", (uint)colour));

            using var image = _imageProcessor.GetFColorsView(coloursSummary);
            image.Should().NotBeNull();
        }
    }
}
