using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using System;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests
{
    /// <summary>
    /// Defines tests for <see cref="Extensions.UserExtensions.GetViewValueForTop(User, Models.TopType)"/> class.
    /// </summary>
    [TestClass]
    public class UserExtensionsTests : Base
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
        [DataRow(TopType.PostsMonthly)]
        [DataRow(TopType.PostsMonthlyCharacter)]
        [DataRow(TopType.Pvp)]
        [DataRow(TopType.PvpSeason)]
        public void Should_Return_Text(TopType topType)
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.CharacterCountFromDate = 1;
            user.MessagesCount = 2;
            user.MessagesCountAtDate = 1;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            user.GameDeck.Cards.Add(card);

            var text = Extensions.UserExtensions.GetViewValueForTop(user, topType);
            text.Should().NotBeNullOrEmpty();
        }
    }
}
