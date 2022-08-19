using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;

namespace Sanakan.Game.Tests.EventServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IEventsService.RandomizeEvent(ExpeditionCardType, (double, double))"/> method.
    /// </summary>
    [TestClass]
    public class RandomizeEventTests : Base
    {
        [TestMethod]
        [DataRow(ExpeditionCardType.DarkExp, EventType.IncreaseAttack, 1000)]
        [DataRow(ExpeditionCardType.DarkExp, EventType.ChangeDere, 5000)]
        [DataRow(ExpeditionCardType.DarkItems, EventType.IncreaseAttack, 1000)]
        [DataRow(ExpeditionCardType.DarkItems, EventType.ChangeDere, 5000)]
        [DataRow(ExpeditionCardType.DarkItemWithExp, EventType.MoreExperience, 1000)]
        [DataRow(ExpeditionCardType.ExtremeItemWithExp, EventType.MoreExperience, 1000)]
        [DataRow(ExpeditionCardType.ExtremeItemWithExp, EventType.LoseCard, 9000)]
        [DataRow(ExpeditionCardType.LightExp, EventType.IncreaseAttack, 1000)]
        [DataRow(ExpeditionCardType.LightExp, EventType.IncreaseDefence, 3000)]
        [DataRow(ExpeditionCardType.LightItems, EventType.IncreaseAttack, 1000)]
        [DataRow(ExpeditionCardType.LightItemWithExp, EventType.MoreExperience, 1000)]
        [DataRow(ExpeditionCardType.LightItemWithExp, EventType.IncreaseAttack, 4000)]
        [DataRow(ExpeditionCardType.NormalItemWithExp, EventType.MoreItems, 1000)]
        [DataRow(ExpeditionCardType.NormalItemWithExp, EventType.MoreExperience, 2000)]
        public void Should_Return_Event(ExpeditionCardType expeditionCardType, EventType expected, int value)
        {
            var duration = (1, 1);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(10000))
                .Returns(value);

            var eventType = _eventsService.RandomizeEvent(expeditionCardType, duration);
            eventType.Should().Be(expected);
        }
    }
}
