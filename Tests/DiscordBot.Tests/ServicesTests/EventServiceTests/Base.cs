using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Services.PocketWaifu;
using Sanakan.Common;
using Sanakan.ShindenApi;

namespace DiscordBot.ServicesTests.EventServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly EventsService _eventsService;
        protected readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);
        protected readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);

        public Base()
        {
            _eventsService = new(
                _shindenClientMock.Object,
                _randomNumberGeneratorMock.Object,
                _systemClockMock.Object);
        }

    }
}
