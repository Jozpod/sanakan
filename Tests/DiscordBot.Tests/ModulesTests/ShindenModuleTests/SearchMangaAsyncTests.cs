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
    /// Defines tests for <see cref="ShindenModule.SearchMangaAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class SearchMangaAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Mange_Info()
        {
            var title = "test";
            var userId = 1ul;

            var searchObject = new QuickSearchResult
            {
                Title = "test",
                TitleId = 1,
            };

            var searchResult = new ShindenResult<List<QuickSearchResult>>
            {
                Value = new List<QuickSearchResult>
                {
                    searchObject,
                }
            };

            var animeMangaInfoResult = new ShindenResult<AnimeMangaInfo>
            {
                Value = new AnimeMangaInfo
                {
                    Title = new TitleEntry
                    {
                        FinishDate = DateTime.UtcNow,
                        Title = "test",
                        Description = new AnimeMangaInfoDescription
                        {
                            OtherDescription = "test",
                        },
                        TitleOther = new List<TitleOther>
                        {

                        }
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
                .Setup(pr => pr.QuickSearchAsync(title, QuickSearchType.Manga))
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
                embed.Fields.Should().HaveCount(3);
            });

            await _module.SearchMangaAsync(title);
        }
    }
}
