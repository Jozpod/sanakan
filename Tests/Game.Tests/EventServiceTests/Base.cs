using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Game.Services.Abstractions;
using Sanakan.Game.Services;
using System.Threading.Tasks;
using Sanakan.Game.Models;
using FluentAssertions;

namespace Sanakan.Game.Tests.EventServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly IEventsService _eventsService;
        protected readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);

        public Base()
        {
            _eventsService = new EventsService(
                _randomNumberGeneratorMock.Object,
                _systemClockMock.Object);
        }

        [TestMethod]
        public async Task Should_Return_Value()
        {
            var eventType = EventType.MoreItems;

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(5);

            var items = _eventsService.GetMoreItems(eventType);

            items.Should().BeGreaterThan(2);
        }
    }
}
