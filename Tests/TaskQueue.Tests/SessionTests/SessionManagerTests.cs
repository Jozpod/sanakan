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
        public async Task Should_Process_Messages()
        {
            
        }
    }
}
