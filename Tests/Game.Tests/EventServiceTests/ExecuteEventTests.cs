using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sanakan.Game.Tests.EventServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IEventsService.ExecuteEvent(EventType, User, Card, string)"/> method.
    /// </summary>
    [TestClass]
    public class ExecuteEventTests : Base
    {
        [TestMethod]
        [DataRow(EventType.MoreItems, true)]
        [DataRow(EventType.MoreExperience, true)]
        [DataRow(EventType.IncreaseAttack, true)]
        [DataRow(EventType.IncreaseDefence, true)]
        [DataRow(EventType.AddReset, true)]
        [DataRow(EventType.NewCard, true)]
        [DataRow(EventType.None, true)]
        [DataRow(EventType.ChangeDere, true)]
        [DataRow(EventType.DecreaseAttack, true)]
        [DataRow(EventType.DecreaseDefence, true)]
        [DataRow(EventType.DecreaseAffection, true)]
        [DataRow(EventType.LoseCard, false)]
        [DataRow(EventType.Fight, true)]
        public void Should_Execute_Event(EventType eventType, bool boolValue)
        {
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.B, Dere.Bodere, DateTime.UtcNow);
            var message = "test message";

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(1);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(1000))
                .Returns(500);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<ulong>>()))
                .Returns<IEnumerable<ulong>>(items => items.First());

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<Dere>>()))
                .Returns<IEnumerable<Dere>>(items => items.First());

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            var result = _eventsService.ExecuteEvent(eventType, user, card, message);
            result.Item1.Should().Be(boolValue);

            switch (eventType)
            {
                case EventType.Fight:
                    result.Item2.Should().Be($"{message}Wydarzenie: Walka, wynik: zwycięstwo!\n");
                    break;
                case EventType.LoseCard:
                    result.Item2.Should().Be($"{message}Wydarzenie: Utrata karty.\n");
                    break;
                default:
                    result.Item2.Should().Be(message);
                    break;
            }
        }
    }
}
