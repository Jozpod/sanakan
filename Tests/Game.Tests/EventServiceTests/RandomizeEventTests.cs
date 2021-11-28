using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using FluentAssertions;
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
        [DataRow(EventType.IncAtk, 1000)]
        public void Should_Return_Event(EventType expected, int value)
        {
            var expedition = ExpeditionCardType.DarkExp;
            var duration = (1, 1);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(10000))
                .Returns(value);

            var eventType = _eventsService.RandomizeEvent(expedition, duration);
            eventType.Should().Be(expected);
        }
    }
}
