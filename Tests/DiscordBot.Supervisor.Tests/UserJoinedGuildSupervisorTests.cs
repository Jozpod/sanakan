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
    public class UserJoinedGuildSupervisorTests
    {
        private readonly IUserJoinedGuildSupervisor _userJoinedGuildSupervisor;
        private readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);

        public UserJoinedGuildSupervisorTests()
        {
            _userJoinedGuildSupervisor = new UserJoinedGuildSupervisor(_systemClockMock.Object);
        }

        [TestMethod]
        public async Task Should_Return_Users()
        {
            var guildId = 1ul;
            var suspects = Enumerable.Range(1, 4).Select(pr => ("username", (ulong)pr));
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
