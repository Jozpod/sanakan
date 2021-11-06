using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.ShindenApi;
using Sanakan.Web.Controllers;
using System;

namespace Sanakan.Web.Tests.Controllers.UserControllerTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly UserController _controller;
        protected readonly Mock<IDiscordSocketClientAccessor> _discordSocketClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<SanakanConfiguration>> _sanakanConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ITransferAnalyticsRepository> _transferAnalyticsRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserContext> _userContextMock = new(MockBehavior.Strict);
        protected readonly Mock<IJwtBuilder> _jwtBuilderMock = new(MockBehavior.Strict);
        protected readonly Mock<IRequestBodyReader> _requestBodyReaderMock = new(MockBehavior.Strict);

        public Base()
        {
            _controller = new(
                _discordSocketClientAccessorMock.Object,
                _sanakanConfigurationMock.Object,
                _userRepositoryMock.Object,
                _transferAnalyticsRepositoryMock.Object,
                _shindenClientMock.Object,
                NullLogger<UserController>.Instance,
                _systemClockMock.Object,
                _cacheManagerMock.Object,
                _userContextMock.Object,
                _jwtBuilderMock.Object,
                _requestBodyReaderMock.Object);
        }
    }
}
