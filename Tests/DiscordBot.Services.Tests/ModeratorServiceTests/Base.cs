using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Sanakan.Common;
using Sanakan.DiscordBot.Services;
using Sanakan.Common.Cache;
using Sanakan.DAL.Repositories.Abstractions;

namespace DiscordBot.ServicesTests.ModeratorServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly IModeratorService _moderatorService;
        protected readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IPenaltyInfoRepository> _penaltyInfoRepositoryMock = new(MockBehavior.Strict);

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_penaltyInfoRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _moderatorService = new ModeratorService(
                NullLogger<IModeratorService>.Instance,
                _cacheManagerMock.Object,
                _systemClockMock.Object,
                serviceScopeFactory);
        }
    }
}
