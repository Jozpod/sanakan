using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FluentAssertions;
using Sanakan.DAL.Models;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Sanakan.DiscordBot.Services.Abstractions;

namespace DiscordBot.ServicesTests.ProfileServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IProfileService.GetColorList(SCurrency)"/> method.
    /// </summary>
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
