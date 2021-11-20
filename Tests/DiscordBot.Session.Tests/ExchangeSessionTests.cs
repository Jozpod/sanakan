using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Threading.Tasks;
using static Sanakan.DiscordBot.Session.ExchangeSession;

namespace Sanakan.DiscordBot.Session.Tests
{
    [TestClass]
    public class ExchangeSessionTests
    {
        private readonly ExchangeSession _session;
        private readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);

        public ExchangeSessionTests()
        {
            var payload = new ExchangeSessionPayload
            {

            };
            _session = new(1ul, DateTime.UtcNow, payload);
        }

        [TestMethod]
        public async Task Should_Check_If_Session_Exists()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_userRepositoryMock.Object);
            var context = new SessionContext
            {

            };
            var serviceProvider = serviceCollection.BuildServiceProvider();

            await _session.ExecuteAsync(context, serviceProvider);
        }
    }
}
