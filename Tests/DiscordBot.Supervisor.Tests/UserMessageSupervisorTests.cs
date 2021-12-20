using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Supervisor.Tests
{
    /// <summary>
    /// Defines tests for <see cref="IUserMessageSupervisor"/> class.
    /// </summary>
    [TestClass]
    public class UserMessageSupervisorTests
    {
        private readonly IUserMessageSupervisor _userMessageSupervisor;
        private readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        private readonly Mock<IOptionsMonitor<SupervisorConfiguration>> _supervisorConfigurationMock = new(MockBehavior.Strict);
        private readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        private readonly Mock<IFileSystem> _fileSystemMock = new(MockBehavior.Strict);
        private readonly Mock<IFileSystemWatcherFactory> _fileSystemWatcherFactoryMock = new(MockBehavior.Strict);
        private readonly Mock<IFileSystemWatcher>  _fileSystemWatcherMock = new(MockBehavior.Strict);
        private readonly Mock<IHostEnvironment> _hostEnvironmentMock = new(MockBehavior.Strict);

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

            _hostEnvironmentMock
                .Setup(pr => pr.ContentRootPath)
                .Returns("path");

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
                .Returns(_fileSystemWatcherMock.Object);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            _userMessageSupervisor = new UserMessageSupervisor(
                NullLogger<UserMessageSupervisor>.Instance,
                _discordConfigurationMock.Object,
                _supervisorConfigurationMock.Object,
                _systemClockMock.Object,
                _fileSystemMock.Object,
                _fileSystemWatcherFactoryMock.Object,
                _hostEnvironmentMock.Object);
        }

        [TestMethod]
        public void Should_Replace_Urls()
        {
            var eventArgs = new FileSystemEventArgs(WatcherChangeTypes.Changed, "directory", "url.txt");
            var invalidUrls = new[]
            {
                "https://invalid.url",
            };

            _fileSystemMock
                .Setup(pr => pr.ReadAllLinesAsync(eventArgs.Name!))
                .ReturnsAsync(invalidUrls);

            _fileSystemWatcherMock.Raise(pr => pr.Changed += null, null, eventArgs);
        }

        [TestMethod]
        public async Task Should_Refresh()
        {
            var guildId = 1ul;
            var userId = 1ul;
            var message = "test";
            var utcNow = DateTime.UtcNow;

            _systemClockMock.Reset();
            _systemClockMock
                .SetupSequence(pr => pr.UtcNow)
                .Returns(utcNow)
                .Returns(utcNow.AddMinutes(6));

            var decision = await _userMessageSupervisor.MakeDecisionAsync(guildId, userId, message, false);
            _userMessageSupervisor.Refresh();
        }

        [TestMethod]
        public async Task Should_Reset()
        {
            var guildId = 1ul;
            var userId = 1ul;
            var messages = Enumerable.Range(1, 11).Select(pr => $"Message {pr}").ToList();
            var utcNow = DateTime.UtcNow;

            _systemClockMock.Reset();
            var setup = _systemClockMock
               .SetupSequence(pr => pr.UtcNow);

            foreach (var item in messages.Take(10))
            {
                setup.Returns(utcNow);
            }
            
            setup.Returns(utcNow.AddMinutes(6));

            foreach (var message in messages.Take(10))
            {
                var decision = await _userMessageSupervisor.MakeDecisionAsync(guildId, userId, message, false);
                decision.Should().Be(SupervisorAction.None);
            }

            {
                var message = messages.Last();
                var decision = await _userMessageSupervisor.MakeDecisionAsync(guildId, userId, message, false);
                decision.Should().Be(SupervisorAction.None);
            }
            
        }

        [TestMethod]
        public async Task Should_Return_Ban_Decision()
        {
            var guildId = 1ul;
            var userId = 1ul;
            var messages = Enumerable.Range(1, 11).Select(pr => $"Message {pr}").ToList();

            foreach (var message in messages.Take(10))
            {
                var decision = await _userMessageSupervisor.MakeDecisionAsync(guildId, userId, message, false);
                decision.Should().Be(SupervisorAction.None);
            }
            {
                var message = messages.Last();
                var decision = await _userMessageSupervisor.MakeDecisionAsync(guildId, userId, message, false);
                decision.Should().Be(SupervisorAction.Ban);
            }
        }

        [TestMethod]
        public async Task Should_Return_Mute_Decision()
        {
            var guildId = 1ul;
            var userId = 1ul;
            var messages = Enumerable.Range(1, 13).Select(pr => $"Message {pr}").ToList();

            foreach (var message in messages.Take(12))
            {
                var decision = await _userMessageSupervisor.MakeDecisionAsync(guildId, userId, message, true);
            }
            {
                var message = messages.Last();
                var decision = await _userMessageSupervisor.MakeDecisionAsync(guildId, userId, message, true);
                decision.Should().Be(SupervisorAction.Mute);
            }
        }


        [TestMethod]
        public async Task Should_Return_Warn_Decision()
        {
            var guildId = 1ul;
            var userId = 1ul;
            var messages = Enumerable.Range(1, 12).Select(pr => $"Message {pr}").ToList();

            foreach (var message in messages.Take(11))
            {
                var decision = await _userMessageSupervisor.MakeDecisionAsync(guildId, userId, message, true);
                decision.Should().Be(SupervisorAction.None);
            }
            {
                var message = messages.Last();
                var decision = await _userMessageSupervisor.MakeDecisionAsync(guildId, userId, message, true);
                decision.Should().Be(SupervisorAction.Warn);
            }
        }
    }
}
