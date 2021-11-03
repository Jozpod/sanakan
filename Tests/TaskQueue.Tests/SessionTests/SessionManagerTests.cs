using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Tests.SessionTests
{
    [TestClass]
    public class SessionManagerTests
    {
        private readonly SessionManager _sessionManager;

        public SessionManagerTests()
        {
            _sessionManager = new();
        }

        [TestMethod]
        public async Task Should_Add_And_Remove_Session()
        {
            var session = new CraftSession(1, DateTime.UtcNow, new CraftSession.CraftSessionPayload());
            _sessionManager.Add(session);
            _sessionManager.Sessions.Should().HaveCount(1);
            _sessionManager.Remove(session);
            _sessionManager.Sessions.Should().HaveCount(0);
        }
    }
}
