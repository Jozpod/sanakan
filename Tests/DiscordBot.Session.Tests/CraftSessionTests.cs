using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sanakan.DiscordBot.Session.CraftSession;

namespace Sanakan.DiscordBot.Session.Tests
{
    [TestClass]
    public class CraftSessionTests
    {
        private readonly CraftSession _session;
        private readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        private readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<IWaifuService> _waifuServiceMock = new(MockBehavior.Strict);

        public CraftSessionTests()
        {
            var payload = new CraftSessionPayload
            {

            };
            _session = new(1ul, DateTime.UtcNow, payload);
        }

        [TestMethod]
        public async Task Should_Check_If_Session_Exists()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_cacheManagerMock.Object);
            serviceCollection.AddSingleton(_userRepositoryMock.Object);
            serviceCollection.AddSingleton(_waifuServiceMock.Object);
            var context = new SessionContext
            {

            };
            var serviceProvider = serviceCollection.BuildServiceProvider();

            await _session.ExecuteAsync(context, serviceProvider);
        }
    }
}
