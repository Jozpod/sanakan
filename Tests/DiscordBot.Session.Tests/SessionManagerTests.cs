using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
            var session = new CraftSession(1, DateTime.UtcNow, new CraftSession.CraftSessionPayload());
            _sessionManager.Add(session);
            _sessionManager.Exists<CraftSession>(session.OwnerId).Should().BeTrue();
        }

        [TestMethod]
        public void Should_Add_And_Remove_Session()
        {
            var session = new CraftSession(1, DateTime.UtcNow, new CraftSession.CraftSessionPayload());
            _sessionManager.Add(session);
            _sessionManager.GetByOwnerId(1, SessionExecuteCondition.AllEvents).Should().HaveCount(1);
            _sessionManager.Remove(session);
            _sessionManager.GetByOwnerId(1, SessionExecuteCondition.AllEvents).Should().HaveCount(0);
        }
    }
}
