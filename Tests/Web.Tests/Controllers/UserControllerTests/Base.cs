using Discord;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.ShindenApi;
using Sanakan.TaskQueue;
using Sanakan.Web.Controllers;
using System;

namespace Sanakan.Web.Tests.Controllers.UserControllerTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly UserController _controller;
        protected readonly Mock<IDiscordClientAccessor> _discordSocketClientAccessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClient> _discordClientMock = new(MockBehavior.Strict);
        protected readonly Mock<IOptionsMonitor<SanakanConfiguration>> _sanakanConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IBlockingPriorityQueue> _blockingPriorityQueueMock = new(MockBehavior.Strict);
        protected readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserContext> _userContextMock = new(MockBehavior.Strict);
        protected readonly Mock<IJwtBuilder> _jwtBuilderMock = new(MockBehavior.Strict);
        protected readonly Mock<IRequestBodyReader> _requestBodyReaderMock = new(MockBehavior.Strict);
        protected readonly SanakanConfiguration _configuration;

        public Base()
        {
            _configuration = new SanakanConfiguration
            {
                Discord = new DiscordConfiguration
                {
                    MainGuild = 1ul,
                },
                SanakanApi = new ApiConfiguration
                {
                    Jwt = new JwtConfiguration
                    {
                        UserWithTokenExpiry = TimeSpan.FromMinutes(30),
                    }
                }
            };
            _sanakanConfigurationMock
               .Setup(pr => pr.CurrentValue)
               .Returns(_configuration);

            _discordSocketClientAccessorMock
                .Setup(pr => pr.Client)
                .Returns(_discordClientMock.Object);

            _controller = new(
                _discordSocketClientAccessorMock.Object,
                _sanakanConfigurationMock.Object,
                _userRepositoryMock.Object,
                _blockingPriorityQueueMock.Object,
                _shindenClientMock.Object,
                NullLogger<UserController>.Instance,
                _userContextMock.Object,
                _jwtBuilderMock.Object,
                _requestBodyReaderMock.Object);
        }
    }
}
