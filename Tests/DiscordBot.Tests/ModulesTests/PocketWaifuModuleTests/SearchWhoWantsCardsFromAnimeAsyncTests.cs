using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.SearchWhoWantsCardsFromAnimeAsync(ulong, bool)"/> method.
    /// </summary>
    [TestClass]
    public class SearchWhoWantsCardsFromAnimeAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Message()
        {
            var animeId = 1ul;
            var user = new User(1ul, DateTime.UtcNow);
            user.GameDeck.Id = user.Id;
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
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var gameDecks = new List<GameDeck>
            {
                user.GameDeck,
            };

            _shindenClientMock
                .Setup(pr => pr.GetAnimeMangaInfoAsync(animeId))
                .ReturnsAsync(animeMangaInfoResult);

            _gameDeckRepositoryMock
                .Setup(pr => pr.GetByAnimeIdAsync(animeId))
                .ReturnsAsync(gameDecks);

            _discordClientMock
                .Setup(pr => pr.GetUserAsync(user.Id, CacheMode.AllowDownload, null))
                .ReturnsAsync(userMock.Object);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.SearchWhoWantsCardsFromAnimeAsync(animeId);
        }
    }
}
