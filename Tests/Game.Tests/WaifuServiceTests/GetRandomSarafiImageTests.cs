using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.GetRandomSarafiImage"/> method.
    /// </summary>
    [TestClass]
    public class GetRandomSarafiImageTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Safari_Image()
        {
            var safariImages = new[]
            {
                new SafariImage(),
            };

            _resourceManagerMock
                .Setup(pr => pr.ReadFromJsonAsync<IEnumerable<SafariImage>>(It.IsAny<string>()))
                .ReturnsAsync(safariImages);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<SafariImage>>()))
                .Returns<IEnumerable<SafariImage>>(items => items.First());

            var safariImage = await _waifuService.GetRandomSarafiImage();
            safariImage.Should().NotBeNull();
        }
    }
}
