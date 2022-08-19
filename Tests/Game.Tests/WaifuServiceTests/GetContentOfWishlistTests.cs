using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Game.Services.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.GetContentOfWishlist(IEnumerable{ulong}, IEnumerable{ulong}, IEnumerable{ulong})"/> method.
    /// </summary>
    [TestClass]
    public class GetContentOfWishlistTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Embed_With_Cards_Details()
        {
            var cardId = 1ul;
            var characterId = 1ul;
            var titleId = 1ul;
            var cardsId = new[] { cardId };
            var charactersId = new[] { characterId };
            var titlesId = new[] { titleId };
            var animeMangaInfoResult = new ShindenResult<AnimeMangaInfo>
            {
                Value = new AnimeMangaInfo
                {
                    Title = new TitleEntry
                    {
                        Type = IllustrationType.Anime,
                        FinishDate = DateTime.UtcNow,
                        Title = "test",
                        Description = new AnimeMangaInfoDescription
                        {
                            OtherDescription = "test",
                        },
                        TitleOther = new List<TitleOther>(),
                        AnimeStatus = AnimeStatus.CurrentlyAiring,
                        Anime = new AnimeInfo
                        {
                            EpisodesCount = 10,
                            TitleId = 1ul,
                        },
                    }
                }
            };
            var characterResult = new ShindenResult<CharacterInfo>
            {
                Value = new CharacterInfo
                {
                    Relations = new List<StaffInfoRelation>
                    {
                        new StaffInfoRelation
                        {
                            FirstName = "Giga",
                            LastName = "Chad",
                            Title = "Giga Chad",
                        }
                    }
                }
            };

            _shindenClientMock
                .Setup(pr => pr.GetAnimeMangaInfoAsync(titleId))
                .ReturnsAsync(animeMangaInfoResult);

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(characterId))
                .ReturnsAsync(characterResult);

            var embed = await _waifuService.GetContentOfWishlist(cardsId, charactersId, titlesId);
            embed.Should().NotBeNull();
        }
    }
}
