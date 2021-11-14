using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using System.Threading.Tasks;
using Discord.Rest;
using System.IO;
using Sanakan.Preconditions;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Sanakan.DAL.Models;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace DiscordBot.ServicesTests.ProfileServiceTests
{
    [TestClass]
    public class GetColorListTests : Base
    {
        [TestMethod]
        public void Should_Return_Color_List()
        {
            var currency = SCurrency.Sc;
            var expectedImage = new Image<Rgba32>(100, 500);

            _imageProcessorMock
                .Setup(pr => pr.GetFColorsView(It.IsAny<IEnumerable<(string, uint)>>()))
                .Returns(expectedImage);

            var result = _profileService.GetColorList(currency);
            result.Should().NotBeNull();
        }
    }
}
