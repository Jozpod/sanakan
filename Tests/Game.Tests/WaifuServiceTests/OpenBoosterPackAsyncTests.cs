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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.OpenBoosterPackAsync(ulong?, BoosterPack)"/> method.
    /// </summary>
    [TestClass]
    public class OpenBoosterPackAsyncTests : Base
    {
        public OpenBoosterPackAsyncTests()
        {
            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(15, 54))
                .Returns(40);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(20, 51))
                .Returns(40);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<Dere>>()))
                .Returns<IEnumerable<Dere>>(items => items.First());

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(40, 221))
                .Returns(120);

            _randomNumberGeneratorMock
               .Setup(pr => pr.GetRandomValue(88, 97))
               .Returns(90);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(100, 131))
                .Returns(90);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(100, 121))
                .Returns(90);
        }

        [TestMethod]
        public async Task Should_Return_Cards_Title()
        {
            var discordUserId = 1ul;
            var boosterPack = new BoosterPack
            {
                TitleId = 1ul,
                CardCount = 10,
            };
            var charactersResult = new Result<TitleCharacters>
            {
                Value = new TitleCharacters
                {
                    Relations = new List<StaffInfoRelation>
                    {
                        new StaffInfoRelation
                        {
                            CharacterId = 1ul,
                        },
                        new StaffInfoRelation
                        {
                            CharacterId = 2ul,
                        }
                    }
                }
            };
            var characterInfoResult = new Result<CharacterInfo>
            {
                Value = new CharacterInfo
                {

                }
            };

            _shindenClientMock
                .Setup(pr => pr.GetCharactersAsync(boosterPack.TitleId.Value))
                .ReturnsAsync(charactersResult);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<StaffInfoRelation>> ()))
                .Returns<IEnumerable<StaffInfoRelation>>(items => items.First());

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(1ul))
                .ReturnsAsync(characterInfoResult);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(1000))
                .Returns(500);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            var cards = await _waifuService.OpenBoosterPackAsync(discordUserId, boosterPack);
            cards.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Should_Return_Cards_Characters()
        {
            var discordUserId = 1ul;
            var boosterPack = new BoosterPack
            {
                Characters = new Collection<BoosterPackCharacter>
                {
                    new BoosterPackCharacter(1ul),
                    new BoosterPackCharacter(2ul),
                },
                CardCount = 10,
            };
            var characterInfoResult = new Result<CharacterInfo>
            {
                Value = new CharacterInfo
                {

                }
            };

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<BoosterPackCharacter>>()))
                .Returns<IEnumerable<BoosterPackCharacter>>(items => items.First());

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(1ul))
                .ReturnsAsync(characterInfoResult);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(1000))
                .Returns(500);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            var cards = await _waifuService.OpenBoosterPackAsync(discordUserId, boosterPack);
            cards.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Should_Return_Cards_Random()
        {
            var discordUserId = 1ul;
            var boosterPack = new BoosterPack
            {
                CardCount = 10,
            };
            var charactersResult = new Result<List<ulong>>
            {
                Value = new List<ulong> { 1, 2, 3 }
            };
            var charactersFromAnimeResult = new Result<IEnumerable<ulong>>
            {
                Value = new[]
                {
                    1ul,
                    2ul,
                    3ul
                }
            };
            var characterInfoResult = new Result<CharacterInfo>
            {
                Value = new CharacterInfo
                {

                }
            };

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(1000))
                .Returns(500);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            _shindenClientMock
                .Setup(pr => pr.GetAllCharactersAsync())
                .ReturnsAsync(charactersResult);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(15, 54))
                .Returns(40);

            _shindenClientMock
                .Setup(pr => pr.GetAllCharactersFromAnimeAsync())
                .ReturnsAsync(charactersFromAnimeResult);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<ulong>>()))
                .Returns<IEnumerable<ulong>>(items => items.First());

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(1ul))
                .ReturnsAsync(characterInfoResult);

            var cards = await _waifuService.OpenBoosterPackAsync(discordUserId, boosterPack);
            cards.Should().NotBeNull();
        }
    }
}
