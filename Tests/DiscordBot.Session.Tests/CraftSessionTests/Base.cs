using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Game.Services.Abstractions;
using System;
using static Sanakan.DiscordBot.Session.CraftSession;

namespace Sanakan.DiscordBot.Session.Tests.CraftSessionTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly CraftSessionPayload _payload;
        protected readonly CraftSession _session;
        protected readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IWaifuService> _waifuServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserMessage> _userMessageMock = new();
        protected readonly Mock<IMessageChannel> _messageChannelMock = new(MockBehavior.Strict);
        protected readonly ServiceProvider _serviceProvider;

        public Base()
        {
            _payload = new CraftSessionPayload();
            _session = new(1ul, DateTime.UtcNow, _payload);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_cacheManagerMock.Object);
            serviceCollection.AddSingleton(_userRepositoryMock.Object);
            serviceCollection.AddSingleton(_waifuServiceMock.Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(It.IsAny<ulong>(), CacheMode.AllowDownload, null))
                .ReturnsAsync(_userMessageMock.Object);
            
        }
    }
}
