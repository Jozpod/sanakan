using Discord.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Modules;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using Sanakan.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Sanakan.DiscordBot.Session;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions.Configuration;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    [TestClass]
    public abstract class Base : TestBase
    {
        protected readonly HelperModule _module;
        protected readonly Mock<IDiscordClientAccessor> _discordClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IHelperService> _helperServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<ISessionManager> _sessionManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IOperatingSystem> _operatingSystemMock = new(MockBehavior.Strict);
        protected readonly Mock<ICommandContext> _socketCommandContextMock = new(MockBehavior.Strict);

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _discordConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new DiscordConfiguration
                {

                });

            _module = new(
                new DefaultIconConfiguration(),
                _discordClientAccessorMock.Object,
                _sessionManagerMock.Object,
                _helperServiceMock.Object,
                NullLogger<HelperModule>.Instance,
                _discordConfigurationMock.Object,
                _systemClockMock.Object,
                _operatingSystemMock.Object,
                serviceScopeFactory);
            Initialize(_module);
        }
    }
}
