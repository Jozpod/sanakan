﻿using DiscordBot.Services.PocketWaifu.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.TaskQueue.Messages;
using Sanakan.Web;
using Sanakan.Web.Controllers;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    [TestClass]
    public class Base
    {
        protected readonly WaifuController _controller;
        protected readonly Mock<IShindenClient> _shindenClientMock = new();
        protected readonly Mock<IOptionsMonitor<ApiConfiguration>> _optionsMock = new();
        protected readonly Mock<IWaifuService> _waifuServiceMock = new();
        protected readonly Mock<IProducerConsumerCollection<BaseMessage>> _blockingPriorityQueueMock = new();
        protected readonly Mock<IFileSystem> _fileSystemMock = new();
        protected readonly Mock<IUserRepository> _userRepositoryMock = new();
        protected readonly Mock<ICardRepository> _cardRepositoryMock = new();
        protected readonly Mock<IUserContext> _userContextMock = new();
        protected readonly Mock<ICacheManager> _cacheManagerMock = new();
        protected readonly Mock<IJwtBuilder> _jwtBuilderMock = new();

        public Base()
        {
            _controller = new WaifuController(
                _shindenClientMock.Object,
                _blockingPriorityQueueMock.Object,
                _optionsMock.Object,
                _waifuServiceMock.Object,
                _fileSystemMock.Object,
                _userRepositoryMock.Object,
                _cardRepositoryMock.Object,
                _userContextMock.Object,
                _cacheManagerMock.Object,
                _jwtBuilderMock.Object);
        }
    }
}
