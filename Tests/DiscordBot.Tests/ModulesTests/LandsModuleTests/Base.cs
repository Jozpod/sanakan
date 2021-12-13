using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Modules;
using Sanakan.Common;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.ModulesTests.LandsModuleTests
{
    [TestClass]
    public abstract class Base : TestBase
    {
        protected readonly LandsModule _module;
        protected readonly Mock<ILandManager> _landManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _module = new(
                _landManagerMock.Object,
                _taskManagerMock.Object,
                serviceScopeFactory);
            Initialize(_module);
        }
    }
}
