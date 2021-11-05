using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Modules;
using Sanakan.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Rest;
using System.IO;
using Sanakan.DiscordBot.Modules;
using Sanakan.TaskQueue;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using Sanakan.Common;

namespace DiscordBot.ModulesTests.HelperModuleTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly HelperModule _module;
        protected readonly Mock<IHelperService> _helperServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<ISessionManager> _sessionManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<IOperatingSystem> _operatingSystemMock = new(MockBehavior.Strict);

        public Base()
        {
            _module = new(
                _helperServiceMock.Object,
                _sessionManagerMock.Object,
                _discordConfigurationMock.Object,
                _guildConfigRepositoryMock.Object,
                _systemClockMock.Object,
                _operatingSystemMock.Object);
        }
    }
}
