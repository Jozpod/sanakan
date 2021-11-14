using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session.Tests
{
    [TestClass]
    public class ListSessionTests
    {
        private readonly ListSession<Card> _session;

        public ListSessionTests()
        {
            _session = new();
        }

        [TestMethod]
        public async Task Should_()
        {
            await _session.ExecuteAsync();
        }
    }
}
