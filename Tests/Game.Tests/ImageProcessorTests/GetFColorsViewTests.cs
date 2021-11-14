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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using FluentAssertions;
using DiscordBot.Services;
using System.Linq;

namespace Sanakan.Game.Tests.ImageProcessorTests
{
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
            SaveImage(image);
        }
    }
}
