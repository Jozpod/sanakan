using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.EndExpedition(User, Card, bool)"/> method.
    /// </summary>
    [TestClass]
    public class EndExpeditionTests : Base
    {
        public static IEnumerable<object[]> EnumerateAllExpeditions
        {
            get
            {
                foreach (var expeditionCardType in Enum.GetValues<ExpeditionCardType>())
                {
                    foreach (var value in Enumerable.Range(0, 11).Select(pr => pr * 1000))
                    {
                        yield return new object[] { expeditionCardType, value };
                    }
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(EnumerateAllExpeditions))]
        public void Should_Return_Expedition_Result(ExpeditionCardType expeditionCardType, int value)
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            card.Expedition = expeditionCardType;
            card.ExpeditionDate = utcNow.AddDays(-1);
            user.GameDeck.Cards.Add(card);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _randomNumberGeneratorMock
                .Setup(pr => pr.TakeATry(It.IsAny<int>()))
                .Returns(true);

            _eventsServiceMock
                .Setup(pr => pr.RandomizeEvent(expeditionCardType, It.IsAny<(double, double)>()))
                .Returns(EventType.ChangeDere);

            _eventsServiceMock
                .Setup(pr => pr.ExecuteEvent(
                    It.IsAny<EventType>(),
                    It.IsAny<User>(),
                    It.IsAny<Card>(),
                    It.IsAny<string>()))
                .Returns((true, "test"));

            _eventsServiceMock
                .Setup(pr => pr.GetMoreItems(It.IsAny<EventType>()))
                .Returns(10);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(10000))
                .Returns(value);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(100000))
                .Returns(50000);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<Dere>>()))
                .Returns<IEnumerable<Dere>>(items => items.First());

            var result = _waifuService.EndExpedition(user, card, false);
            result.Should().NotBeNull();
        }
    }
}
