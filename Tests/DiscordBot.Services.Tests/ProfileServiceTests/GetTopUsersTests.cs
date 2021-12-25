using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.ServicesTests.ProfileServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IProfileService.GetTopUsers(IEnumerable{User}, Sanakan.Game.Models.TopType, DateTime)"/> method.
    /// </summary>
    [TestClass]
    public class GetTopUsersTests : Base
    {
        [TestMethod]
        [DataRow(TopType.AcCount)]
        [DataRow(TopType.Card)]
        [DataRow(TopType.Cards)]
        [DataRow(TopType.CardsPower)]
        [DataRow(TopType.Commands)]
        [DataRow(TopType.PcCount)]
        [DataRow(TopType.ScCount)]
        [DataRow(TopType.TcCount)]
        [DataRow(TopType.Karma)]
        [DataRow(TopType.KarmaNegative)]
        [DataRow(TopType.Level)]
        [DataRow(TopType.Pvp)]
        [DataRow(TopType.PvpSeason)]
        [DataRow(TopType.PostsMonthly)]
        [DataRow(TopType.PostsMonthlyCharacter)]
        public void Should_Return_Ordered_Users(TopType topType)
        {
            var utcNow = DateTime.UtcNow;
            var users = Enumerable.Range(0, 5)
                .Select((pr, index) =>
                {
                    var user = new User((ulong)index, utcNow.AddHours(1));
                    user.AcCount = 0;
                    user.GameDeck.Karma = index * 1000;
                    user.GameDeck.CTCount = index * 1000;
                    user.TcCount = index * 1000;
                    user.GameDeck.PVPCoins = index * 1000;
                    user.MessagesCount = (ulong)index * 1000;
                    user.MeasuredOn = utcNow.Date;
                    user.GameDeck.PVPSeasonBeginDate = user.MeasuredOn;
                    user.MessagesCountAtDate = (ulong)index * 10 + 1;
                    user.GameDeck.GlobalPVPRank = index + 1;
                    user.GameDeck.SeasonalPVPRank = index + 1;
                    return user;
                });

            var result = _profileService.GetTopUsers(users, topType, utcNow);
            result.Should().HaveCount(5);
        }

    }
}
