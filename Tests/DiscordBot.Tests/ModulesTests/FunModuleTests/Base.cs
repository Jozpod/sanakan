using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Rest;
using System.IO;
using Sanakan.DiscordBot.Modules;
using Sanakan.Common;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.TaskQueue;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly FunModule _module;
        protected readonly Mock<IModeratorService> _moderatorServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<ISessionManager> _sessionManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _module = new(
                _moderatorServiceMock.Object,
                _sessionManagerMock.Object,
                _cacheManagerMock.Object,
                _systemClockMock.Object,
                serviceScopeFactory,
                _taskManagerMock.Object);
        }
    }
}