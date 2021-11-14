using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session.Tests
{
    [TestClass]
    public class AcceptSessionTests
    {
        private readonly AcceptSession _session;

        public AcceptSessionTests()
        {
            _session = new();
        }

        [TestMethod]
        public async Task Should_Check_If_Session_Exists()
        {
            var serviceCollection = new ServiceCollection();
            var context = new SessionContext
            {

            };
            var serviceProvider = serviceCollection.BuildServiceProvider();

            await _session.ExecuteAsync(context, serviceProvider);
        }
    }
}
