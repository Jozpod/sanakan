using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Services.Abstractions;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Rest;
using System.IO;
using Sanakan.DiscordBot.Modules;
using Sanakan.TaskQueue;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using Sanakan.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Sanakan.DiscordBot.Session;
using Sanakan.DiscordBot;

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
            var serviceProvider = serviceCollection.BuildServiceProvider();
            
            _module = new(
                _discordClientAccessorMock.Object,
                _sessionManagerMock.Object,
                _helperServiceMock.Object,
                NullLogger<HelperModule>.Instance,
                _discordConfigurationMock.Object,
                _guildConfigRepositoryMock.Object,
                _systemClockMock.Object,
                _operatingSystemMock.Object,
                serviceProvider);
            Initialize(_module);
        }
    }
}
