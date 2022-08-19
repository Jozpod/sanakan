using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot.Session.Abstractions;
using System;
using System.Linq;

namespace Sanakan.DiscordBot.Session.Tests
{
    /// <summary>
    /// Defines tests for <see cref="SessionManager"/> class.
    /// </summary>
    [TestClass]
    public class SessionManagerTests
    {
        private readonly SessionManager _sessionManager;

        public SessionManagerTests()
        {
            _sessionManager = new();
        }

        [TestMethod]
        public void Should_Check_If_Session_Exists()
        {
            var session = new CraftSession(1, DateTime.UtcNow, null, null, null, null, null);
            _sessionManager.Add(session);
            _sessionManager.Exists<CraftSession>(session.OwnerIds.First()).Should().BeTrue();
        }

        [TestMethod]
        public void Should_Add_And_Remove_Session()
        {
            var session = new CraftSession(1, DateTime.UtcNow, null, null, null, null, null);
            _sessionManager.Add(session);
            _sessionManager.GetByOwnerId(1, SessionExecuteCondition.AllEvents).Should().HaveCount(1);
            _sessionManager.Remove(session);
            _sessionManager.GetByOwnerId(1, SessionExecuteCondition.AllEvents).Should().HaveCount(0);
        }

        [TestMethod]
        public void Should_Remove_If_Exist()
        {
            var session = new CraftSession(1, DateTime.UtcNow, null, null, null, null, null);
            _sessionManager.Add(session);
            _sessionManager.RemoveIfExists<CraftSession>(1);
            var sessions = _sessionManager.GetByOwnerId(1, SessionExecuteCondition.AllEvents);
            sessions.Should().BeEmpty();
        }

        [TestMethod]
        public void Should_Get_Expired()
        {
            var utcNow = DateTime.UtcNow;
            var session = new CraftSession(1, utcNow, null, null, null, null, null);
            _sessionManager.Add(session);
            var sessions = _sessionManager.GetExpired(utcNow.AddHours(1));
            sessions.Should().NotBeEmpty();
        }
    }
}
