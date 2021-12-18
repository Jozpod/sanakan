using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.GetRandomCharacterAsync"/> method.
    /// </summary>
    [TestClass]
    public class GetRandomCharacterAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Character()
        {
            var utcNow = DateTime.UtcNow;
            var characterId = 1ul;
            var characterResult = new Result<IEnumerable<ulong>>
            {
                Value = new List<ulong>
                {
                    characterId
                }
            };
            var characterInfoResult = new Result<CharacterInfo>
            {
                Value = new CharacterInfo
                {

                }
            };

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _shindenClientMock
                .Setup(pr => pr.GetAllCharactersFromAnimeAsync())
                .ReturnsAsync(characterResult);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<ulong>>()))
                .Returns<IEnumerable<ulong>>(items => items.First());

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(characterId))
                .ReturnsAsync(characterInfoResult);

            var characterInfo = await _waifuService.GetRandomCharacterAsync();
            characterInfo.Should().NotBeNull();
        }
    }
}
