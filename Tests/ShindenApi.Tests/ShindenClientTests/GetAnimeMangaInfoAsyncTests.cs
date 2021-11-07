using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using Shinden.API;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class GetAnimeMangaInfoAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Manga_Info()
        {
            var expected = new AnimeMangaInfo
            {
                Title = new TitleEntry
                {
                    MpaaRating = Models.Enums.MpaaRating.R,
                    RatingStorySum = string.Empty,
                    Description = new AnimeMangaInfoDescription
                    {
                        DescriptionId = 1,
                        TitleId = 1,
                        LangCode = Language.English,
                    },
                }
            };

            MockHttpOk("manga-info-result.json", HttpMethod.Get);

            var titleId = 1ul;
            var result = await _shindenClient.GetAnimeMangaInfoAsync(titleId);
            result.Value.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task Should_Return_Anime_Info()
        {
            var expected = new AnimeMangaInfo
            {
                Title = new TitleEntry
                {

                }
            };

            MockHttpOk("anime-info-result.json", HttpMethod.Get);

            var titleId = 1ul;
            var result = await _shindenClient.GetAnimeMangaInfoAsync(titleId);
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
