using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using System;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session.Tests
{
    [TestClass]
    public class ListSessionTests
    {
        private readonly ListSession<Card> _session;

        public ListSessionTests()
        {
            var payload = new ListSession<Card>.ListSessionPayload
            {

            };
            _session = new(1ul, DateTime.UtcNow, payload);
        }

        [TestMethod]
        public async Task Should_()
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
