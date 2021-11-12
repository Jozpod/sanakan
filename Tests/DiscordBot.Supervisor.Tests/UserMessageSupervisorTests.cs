using Discord.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Supervisor.Tests
{
    [TestClass]
    public class UserMessageSupervisorTests
    {
        private readonly IUserMessageSupervisor _userMessageSupervisor;
        private readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        private readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        private readonly Mock<IFileSystem> _fileSystemMock = new(MockBehavior.Strict);

        public UserMessageSupervisorTests()
        {
            _userMessageSupervisor = new UserMessageSupervisor(
                _discordConfigurationMock.Object,
                _systemClockMock.Object,
                _fileSystemMock.Object);
        }

        [TestMethod]
        public async Task Should_Return_Ban_Decision()
        {
            var guildId = 1ul;
            var userId = 1ul;
            var messages = Enumerable.Range(1, 12).Select(pr => $"Message {pr}");

            foreach (var message in messages)
            {
                var decision = _userMessageSupervisor.MakeDecision(guildId, userId, message, false);
                decision.Should().Be(SupervisorAction.None);
            }
        }
    }
}
