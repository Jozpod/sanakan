using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Services.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests
{
    [TestClass]
    public class CommandHandlerTests
    {
        private readonly CommandHandler _commandHandler;
        private readonly Mock<IDiscordSocketClientAccessor> _discordSocketClientAccessorMock = new(MockBehavior.Strict);
        private readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        private readonly Mock<CommandService> _commandServiceMock = new(MockBehavior.Strict);
        private readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);

        public CommandHandlerTests()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            
        _commandHandler = new(
                _discordSocketClientAccessorMock.Object,
                _discordConfigurationMock.Object,
                NullLogger<CommandHandler>.Instance,
                _commandServiceMock.Object,
                _systemClockMock.Object,
                serviceProvider,
                serviceScopeFactory);
        }

        [TestMethod]
        public async Task Should_()
        {

        }
    }
}
