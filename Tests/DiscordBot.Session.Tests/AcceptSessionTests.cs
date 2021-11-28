﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using static Sanakan.DiscordBot.Session.AcceptSession;

namespace Sanakan.DiscordBot.Session.Tests
{
    /// <summary>
    /// Defines tests for <see cref="AcceptSession"/> class.
    /// </summary>
    [TestClass]
    public class AcceptSessionTests
    {
        private readonly AcceptSession _session;

        public AcceptSessionTests()
        {
            var payload = new AcceptSessionPayload
            {

            };
            _session = new(1ul, DateTime.UtcNow, payload);
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
