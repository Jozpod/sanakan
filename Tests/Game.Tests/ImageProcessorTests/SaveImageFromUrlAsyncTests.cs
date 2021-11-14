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

namespace Sanakan.Game.Tests.ImageProcessorTests
{
    [TestClass]
    public class SaveImageFromUrlAsyncTests : Base
    {
        [TestMethod]
        public void Should_Return_Waifu_Image()
        {
            var imageUrl = "TestData/card-image.png";
            var filePath = "TestData/card-image.png";

            using var image = _imageProcessor.SaveImageFromUrlAsync(imageUrl, filePath);
            image.Should().NotBeNull();
        }
    }
}
