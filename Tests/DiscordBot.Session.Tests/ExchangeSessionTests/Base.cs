﻿using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Cache;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.Game.Models;
using System;

namespace Sanakan.DiscordBot.Session.ExchangeSessionTests
{
    /// <summary>
    /// Defines tests for <see cref="ExchangeSession"/> class.
    /// </summary>
    [TestClass]
    public abstract class Base
    {
        protected readonly ExchangeSession _session;
        protected readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        protected readonly ServiceProvider _serviceProvider;
        protected readonly Mock<IUser> _userMock = new();
        protected readonly Mock<IUserMessage> _userMessageMock = new();
        protected readonly Mock<IMessageChannel> _messageChannelMock = new(MockBehavior.Strict);
        protected readonly PlayerInfo _sourcePlayer = new();
        protected readonly PlayerInfo _destinationPlayer = new();

        public Base()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IIconConfiguration>(new DefaultIconConfiguration());
            serviceCollection.AddSingleton(_userRepositoryMock.Object);
            serviceCollection.AddSingleton(_cacheManagerMock.Object);

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _session = new(
                1ul,
                2ul,
                DateTime.UtcNow,
                _userMessageMock.Object,
                _sourcePlayer,
                _destinationPlayer,
                string.Empty,
                string.Empty);
        }
    }
}
