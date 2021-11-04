using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Sanakan.Common;

namespace DiscordBot.ServicesTests.ModeratorServiceTests
{
    [TestClass]
    public abstract class Base
    {
        private readonly ModeratorService _moderatorService;
        private readonly Mock<DiscordSocketClient> _discordSocketClientMock = new(MockBehavior.Strict);
        private readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        private readonly Mock<ISystemClock> _ystemClockMock = new(MockBehavior.Strict);

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _moderatorService = new(
                NullLogger<IModeratorService>.Instance,
                _discordSocketClientMock.Object,
                _cacheManagerMock.Object,
                _ystemClockMock.Object,
                serviceScopeFactory);
        }
    }
}
