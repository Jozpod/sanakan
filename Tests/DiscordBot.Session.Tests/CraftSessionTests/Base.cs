using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;

namespace Sanakan.DiscordBot.Session.Tests.CraftSessionTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly CraftSession _session;
        protected readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IWaifuService> _waifuServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserMessage> _userMessageMock = new();
        protected readonly Mock<IMessageChannel> _messageChannelMock = new(MockBehavior.Strict);
        protected readonly List<Item> _ownedItems = new();
        protected readonly PlayerInfo _playerInfo = new();
        protected readonly ServiceProvider _serviceProvider;

        public Base()
        {
            _session = new(
                1ul,
                DateTime.UtcNow,
                _userMessageMock.Object,
                _ownedItems,
                _playerInfo,
                "⚒ **Tworzenie:**",
                "Polecenia: `dodaj/usuń [nr przedmiotu] [liczba]`.");
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_cacheManagerMock.Object);
            serviceCollection.AddSingleton(_userRepositoryMock.Object);
            serviceCollection.AddSingleton(_waifuServiceMock.Object);
            serviceCollection.AddSingleton<IIconConfiguration>(new DefaultIconConfiguration());
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