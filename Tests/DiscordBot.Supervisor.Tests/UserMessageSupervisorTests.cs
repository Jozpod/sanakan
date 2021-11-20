using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using System;
using System.Linq;

namespace Sanakan.DiscordBot.Supervisor.Tests
{
    [TestClass]
    public class UserMessageSupervisorTests
    {
        private readonly IUserMessageSupervisor _userMessageSupervisor;
        private readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        private readonly Mock<IOptionsMonitor<SupervisorConfiguration>> _supervisorConfigurationMock = new(MockBehavior.Strict);
        private readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        private readonly Mock<IFileSystem> _fileSystemMock = new(MockBehavior.Strict);
        private readonly Mock<IFileSystemWatcherFactory> _fileSystemWatcherFactoryMock = new(MockBehavior.Strict);
        private readonly FakeFileSystemWatcher _fileSystemWatcher = new();
        
        public UserMessageSupervisorTests()
        {
            _supervisorConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new SupervisorConfiguration
                {
                    MessagesLimit = 12,
                    MessageLimit = 6,
                    MessageCommandLimit = 2,
                });

            _discordConfigurationMock
               .Setup(pr => pr.CurrentValue)
               .Returns(new DiscordConfiguration
               {
                   Prefix = ".",
               });

            _fileSystemMock
                .Setup(pr => pr.ReadAllLinesAsync("disallowed-urls.txt"))
                .ReturnsAsync(new string[] { "boostnltro.com" });

            _fileSystemWatcherFactoryMock
                .Setup(pr => pr.Create(It.IsAny<FileSystemWatcherOptions>()))
                .Returns(_fileSystemWatcher);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            _userMessageSupervisor = new UserMessageSupervisor(
                NullLogger<UserMessageSupervisor>.Instance,
                _discordConfigurationMock.Object,
                _supervisorConfigurationMock.Object,
                _systemClockMock.Object,
                _fileSystemMock.Object,
                _fileSystemWatcherFactoryMock.Object);
        }

        [TestMethod]
        public void Should_Return_Ban_Decision()
        {
            var guildId = 1ul;
            var userId = 1ul;
            var messages = Enumerable.Range(1, 11).Select(pr => $"Message {pr}").ToList();

            foreach (var message in messages.Take(10))
            {
                var decision = _userMessageSupervisor.MakeDecision(guildId, userId, message, false);
                decision.Should().Be(SupervisorAction.None);
            }
            {
                var message = messages.Last();
                var decision = _userMessageSupervisor.MakeDecision(guildId, userId, message, false);
                decision.Should().Be(SupervisorAction.Ban);
            }
        }

        [TestMethod]
        public void Should_Return_Mute_Decision()
        {
            var guildId = 1ul;
            var userId = 1ul;
            var messages = Enumerable.Range(1, 13).Select(pr => $"Message {pr}").ToList();

            foreach (var message in messages.Take(12))
            {
                var decision = _userMessageSupervisor.MakeDecision(guildId, userId, message, true);
            }
            {
                var message = messages.Last();
                var decision = _userMessageSupervisor.MakeDecision(guildId, userId, message, true);
                decision.Should().Be(SupervisorAction.Mute);
            }
        }


        [TestMethod]
        public void Should_Return_Warn_Decision()
        {
            var guildId = 1ul;
            var userId = 1ul;
            var messages = Enumerable.Range(1, 12).Select(pr => $"Message {pr}").ToList();

            foreach (var message in messages.Take(11))
            {
                var decision = _userMessageSupervisor.MakeDecision(guildId, userId, message, true);
                decision.Should().Be(SupervisorAction.None);
            }
            {
                var message = messages.Last();
                var decision = _userMessageSupervisor.MakeDecision(guildId, userId, message, true);
                decision.Should().Be(SupervisorAction.Warn);
            }
        }
    }
}
