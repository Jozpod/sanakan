using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.Abstractions;

namespace DiscordBot.ServicesTests.AuditServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly AuditService _auditService;
        protected readonly Mock<IDiscordClientAccessor> _discordClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);

        protected readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);

        public Base()
        {
            _discordConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new DiscordConfiguration
                {

                });

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _auditService = new (
                NullLogger<AuditService>.Instance,
                _discordClientAccessorMock.Object,
                _discordConfigurationMock.Object,
                serviceScopeFactory);
        }
    }
}
