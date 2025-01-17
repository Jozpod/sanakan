﻿using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sanakan.DiscordBot.Supervisor.Tests
{
    /// <summary>
    /// Defines tests for <see cref="IUserJoinedGuildSupervisor"/> class.
    /// </summary>
    [TestClass]
    public class UserJoinedGuildSupervisorTests
    {
        private readonly UserJoinedGuildSupervisor _userJoinedGuildSupervisor;
        private readonly Mock<IOptionsMonitor<SupervisorConfiguration>> _supervisorConfigurationMock = new(MockBehavior.Strict);
        private readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);

        public UserJoinedGuildSupervisorTests()
        {
            _supervisorConfigurationMock
               .Setup(pr => pr.CurrentValue)
               .Returns(new SupervisorConfiguration
               {
                   MessagesLimit = 12,
                   MessageLimit = 6,
                   MessageCommandLimit = 2,
                   SameUsernameLimit = 3,
               });

            _userJoinedGuildSupervisor = new UserJoinedGuildSupervisor(
                _supervisorConfigurationMock.Object,
                _systemClockMock.Object);
        }

        [TestMethod]
        public void Should_Refresh()
        {
            var utcNow = DateTime.UtcNow;
            var guildId = 1ul;
            var suspects = Enumerable.Range(1, 4).Select(pr => ("username", (ulong)pr)).ToList();
            IEnumerable<ulong> usersToBan;

            _systemClockMock.Reset();
            _systemClockMock
                .SetupSequence(pr => pr.UtcNow)
                .Returns(utcNow)
                .Returns(utcNow)
                .Returns(utcNow)
                .Returns(utcNow.AddMinutes(6))
                .Returns(utcNow.AddMinutes(6));

            foreach (var (username, userId) in suspects.Take(3))
            {
                usersToBan = _userJoinedGuildSupervisor.GetUsersToBanCauseRaid(guildId, username, userId);
                usersToBan.Should().BeEmpty();
            }

            _userJoinedGuildSupervisor.Refresh();
            {
                var (username, userId) = suspects.Last();
                usersToBan = _userJoinedGuildSupervisor.GetUsersToBanCauseRaid(guildId, username, userId);
                usersToBan.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void Should_Return_Users()
        {
            var guildId = 1ul;
            var suspects = Enumerable.Range(1, 4).Select(pr => ("username", (ulong)pr)).ToList();
            IEnumerable<ulong> usersToBan;

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            foreach (var (username, userId) in suspects.Take(3))
            {
                usersToBan = _userJoinedGuildSupervisor.GetUsersToBanCauseRaid(guildId, username, userId);
                usersToBan.Should().BeEmpty();
            }

            {
                var (username, userId) = suspects.Last();
                usersToBan = _userJoinedGuildSupervisor.GetUsersToBanCauseRaid(guildId, username, userId);
                usersToBan.Should().BeEquivalentTo(suspects.Select(pr => pr.Item2));
            }
        }
    }
}
