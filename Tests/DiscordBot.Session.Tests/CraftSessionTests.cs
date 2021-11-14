using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session.Tests
{
    [TestClass]
    public class CraftSessionTests
    {
        private readonly CraftSession _session;

        public CraftSessionTests()
        {
            _session = new();
        }

        [TestMethod]
        public async Task Should_Check_If_Session_Exists()
        {
            await _session.ExecuteAsync();
        }
    }
}
