using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Session;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenModule.SearchAnimeAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class SearchAnimeAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Anime_Info()
        {
            var title = "test";
            var userId = 1ul;

            var searchObject = new QuickSearchResult
            {
                Title = "Anime 1",
                TitleId = 1,
            };

            var searchResult = new ShindenResult<List<QuickSearchResult>>
            {
                Value = new List<QuickSearchResult>
                {
                    searchObject,
                },
            };

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
                        },
                    }
                }
            };

            _sessionManagerMock
                .Setup(pr => pr.Exists<SearchSession>(userId))
                .Returns(false);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            _shindenClientMock
                .Setup(pr => pr.QuickSearchAsync(title, QuickSearchType.Anime))
                .ReturnsAsync(searchResult);

            _shindenClientMock
                .Setup(pr => pr.GetAnimeMangaInfoAsync(searchObject.TitleId))
                .ReturnsAsync(animeMangaInfoResult);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Fields.Should().HaveCount(4);
            });

            await _module.SearchAnimeAsync(title);
        }

        [TestMethod]
        public async Task Should_Start_Session()
        {
            var title = "test";
            var userId = 1ul;

            var searchObject = new QuickSearchResult
            {
                Title = "Anime 1",
                TitleId = 1,
            };

            var searchResult = new ShindenResult<List<QuickSearchResult>>
            {
                Value = new List<QuickSearchResult>
                {
                    searchObject,
                    new QuickSearchResult
                    {
                        Title = "Anime 2",
                        TitleId = 2,
                    },
                },
            };

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
                        },
                    }
                }
            };

            _sessionManagerMock
                .Setup(pr => pr.Exists<SearchSession>(userId))
                .Returns(false);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            _shindenClientMock
                .Setup(pr => pr.QuickSearchAsync(title, QuickSearchType.Anime))
                .ReturnsAsync(searchResult);

            _shindenClientMock
                .Setup(pr => pr.GetAnimeMangaInfoAsync(searchObject.TitleId))
                .ReturnsAsync(animeMangaInfoResult);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            _sessionManagerMock
                .Setup(pr => pr.Add(It.IsAny<SearchSession>()));

            SetupSendMessage();

            await _module.SearchAnimeAsync(title);
        }
    }
}