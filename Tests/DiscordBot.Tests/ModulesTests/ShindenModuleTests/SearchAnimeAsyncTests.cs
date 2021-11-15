using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Diagnostics;
using System;
using Sanakan.ShindenApi.Models;
using Moq;
using Sanakan.ShindenApi;
using System.Collections.Generic;
using Sanakan.ShindenApi.Models.Enums;
using System.Linq;
using Sanakan.DiscordBot.Session;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
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
                Title = "test",
                TitleId = 1,
            };

            var searchResult = new Result<List<QuickSearchResult>>
            {
                Value = new List<QuickSearchResult>
                {
                    searchObject,
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
                .ReturnsAsync(new Result<AnimeMangaInfo>
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
                });
            

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            await _module.SearchAnimeAsync(title);
        }
    }
}
