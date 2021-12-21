using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Sanakan.Common.Models;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.ShindenApi.Models;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.ImageProcessorTests
{
    /// <summary>
    /// Defines tests for <see cref="IImageProcessor.GetUserProfileAsync(UserInfo?, User, string, long, string, Discord.Color)"/> method.
    /// </summary>
    [TestClass]
    public class GetUserProfileAsyncTests : Base
    {

        public static IEnumerable<object[]> EnumerateAllProfileTypes
        {
            get
            {
                foreach (var showWaifuInProfile in new[] { false, true })
                {
                    foreach (var profileType in Enum.GetValues<ProfileType>())
                    {
                        yield return new object[] { profileType, showWaifuInProfile };
                    }
                }
            }
        }

        [DynamicData(nameof(EnumerateAllProfileTypes))]
        [DataTestMethod]
        public async Task Should_Return_User_Profile_Image(ProfileType profileType, bool showWaifuInProfile)
        {
            var shindenUser = new UserInfo
            {
                Rank = "test ranga",
                ReadedStatus = new ReadWatchStatuses
                {
                    Total = 50,
                    Completed = 10,
                    Dropped = 20,
                    Hold = 30,
                    InProgress = 40,
                    Plan = 0,
                    Skip = 60,
                },
                ReadTime = new Time
                {
                    Days = 1,
                    Hours = 2,
                    Months = 3,
                    Minutes = 4,
                    Years = 5,
                },
                WatchedStatus = new ReadWatchStatuses
                {
                    Completed = 1,
                    Dropped = 2,
                    Hold = 3,
                    InProgress = 4,
                    Plan = 5,
                    Skip = 6,
                    Total = 21,
                },
                MeanMangaScore = new MeanScore
                {
                    Rating = 4.20,
                    ScoreCount = 10,
                },
                MeanAnimeScore = new MeanScore
                {
                    Rating = 4.20,
                    ScoreCount = 10,
                },
            };
            var databaseUser = new User(1, DateTime.Now)
            {
                Level = 420,
                TcCount = 69,
                ExperienceCount = 1443001,
                MessagesCount = 1488,
                ProfileType = profileType,
                ShowWaifuInProfile = showWaifuInProfile,
            };
            var card1 = new Card(1ul, "test card 1", "test card 1", 100, 50, Rarity.SSS, Dere.Dandere, DateTime.UtcNow);
            card1.Quality = Quality.Zeta;
            var card2 = new Card(2ul, "test card 2", "test card 2", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card2.Quality = Quality.Omega;
            var card3 = new Card(3ul, "test card 3", "test card 3", 100, 50, Rarity.E, Dere.Tsundere, DateTime.UtcNow);
            card3.Quality = Quality.Gamma;
            var card4 = new Card(4ul, "test card 4", "test card 4", 100, 50, Rarity.S, Dere.Yami, DateTime.UtcNow);
            card4.Quality = Quality.Alpha;
            databaseUser.GameDeck.FavouriteWaifuId = 1ul;
            databaseUser.GameDeck.Cards.Add(card1);
            databaseUser.GameDeck.Cards.Add(card2);
            databaseUser.GameDeck.Cards.Add(card3);
            databaseUser.GameDeck.Cards.Add(card4);
            var avatarUrl = "https://test.com/user-avatar";
            var topPosition = 5;
            var nickname = "test user";
            var color = Discord.Color.DarkerGrey;

            _httpClientHandlerMock
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(pr => pr.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StreamContent(CreateFakeImage()),
                    };
                });

            _fileSystemMock
                .Setup(pr => pr.Exists(It.IsAny<string>()))
                .Returns(true);

            _fileSystemMock
                .Setup(pr => pr.OpenRead(It.IsAny<string>()))
                .Returns(CreateFakeImage);

            var userProfileImage = await _imageProcessor.GetUserProfileAsync(
                shindenUser,
                databaseUser,
                avatarUrl,
                topPosition,
                nickname,
                color);
            userProfileImage.Should().NotBeNull();
        }
    }
}