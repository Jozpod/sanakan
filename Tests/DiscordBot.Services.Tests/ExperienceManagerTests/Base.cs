using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.TaskQueue;

namespace DiscordBot.ServicesTests.ExperienceManagerTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly ExperienceManager _experienceManager;
        protected readonly Mock<IBlockingPriorityQueue> _blockingPriorityQueueMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<ExperienceConfiguration>> _experienceConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClientAccessor> _discordClientAccessorMock = new(MockBehavior.Strict);

        protected readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);

        public Base()
        {
            _experienceConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new ExperienceConfiguration
                {
                    CharPerPacket = 12,
                    CharPerPoint = 5,
                    SaveThreshold = 5,
                });

            _discordConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new DiscordConfiguration());

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
            serviceCollection.AddSingleton(_userRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _experienceManager = new ExperienceManager(
                _blockingPriorityQueueMock.Object,
                serviceScopeFactory,
                _discordClientAccessorMock.Object,
                _discordConfigurationMock.Object,
                _experienceConfigurationMock.Object,
                _systemClockMock.Object);
        }
    }
}
