using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sanakan.ShindenApi.Models;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.GenerateNewCard(ulong?, CharacterInfo)"/> method.
    /// </summary>
    [TestClass]
    public class GenerateNewCardTests : Base
    {
        [TestMethod]
        public void Should_Return_Card()
        {
            var utcNow = DateTime.UtcNow;
            var characterInfo = new CharacterInfo
            {
            };
            var discordUserId = 1ul;

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(1000))
                .Returns(500);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(15, 54))
                .Returns(30);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(20, 51))
                .Returns(40);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<Dere>>()))
                .Returns(Dere.Tsundere);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(40, 231))
                .Returns(100);

            var card = _waifuService.GenerateNewCard(discordUserId, characterInfo);
            card.Should().NotBeNull();
        }
    }
}
