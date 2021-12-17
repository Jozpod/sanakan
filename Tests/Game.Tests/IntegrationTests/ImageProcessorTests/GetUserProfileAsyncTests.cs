using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common.Models;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.ShindenApi.Models;
using System;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.IntegrationTests.ImageProcessorTests
{
    /// <summary>
    /// Defines tests for <see cref="IImageProcessor.GetUserProfileAsync(UserInfo?, User, string, long, string, Discord.Color)"/> method.
    /// </summary>
#if DEBUG
    [TestClass]
#endif
    public class GetUserProfileAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Return_User_Profile_Image()
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
                ProfileType = ProfileType.Cards,
            };
            var card1 = new Card(1ul, "test card 1", "test card 1", 100, 50, Rarity.SSS, Dere.Dandere, DateTime.UtcNow);
            var card2 = new Card(2ul, "test card 2", "test card 2", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            var card3 = new Card(3ul, "test card 3", "test card 3", 100, 50, Rarity.E, Dere.Tsundere, DateTime.UtcNow);
            var card4 = new Card(4ul, "test card 4", "test card 4", 100, 50, Rarity.S, Dere.Yami, DateTime.UtcNow);
            databaseUser.GameDeck.FavouriteWaifuId = 1ul;
            databaseUser.GameDeck.Cards.Add(card1);
            databaseUser.GameDeck.Cards.Add(card2);
            databaseUser.GameDeck.Cards.Add(card3);
            databaseUser.GameDeck.Cards.Add(card4);
            var avatarUrl = "https://test.com/user-avatar";
            var topPosition = 5;
            var nickname = "test user";
            var color = Discord.Color.DarkerGrey;

            MockHttpGetImage("TestData/user-avatar.png");

            var userProfileImage = await _imageProcessor.GetUserProfileAsync(
                shindenUser,
                databaseUser,
                avatarUrl,
                topPosition,
                nickname,
                color);
            userProfileImage.Should().NotBeNull();

            await ShouldBeEqual("TestData/expected-user-profile.png", userProfileImage);
        }
    }
}