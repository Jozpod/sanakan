using Discord;
using Discord.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Preconditions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests.PreconditionsTests
{
    /// <summary>
    /// Defines tests for <see cref="RequireAnyCommandChannelOrLevel.CheckPermissionsAsync(ICommandContext, CommandInfo, System.IServiceProvider)"/> method.
    /// </summary>
    [TestClass]
    public class RequireAnyCommandChannelOrLevelTests
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly RequireAnyCommandChannelOrLevel _preconditionAttribute;
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly Mock<ITextChannel> _textChannelMock = new(MockBehavior.Strict);
        private readonly Mock<IGuild> _guildMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);

        public RequireAnyCommandChannelOrLevelTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
            serviceCollection.AddSingleton(_userRepositoryMock.Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _commandContextMock
                .Setup(pr => pr.Guild)
                .Returns(_guildMock.Object);

            _preconditionAttribute = new();
        }

        public void SetupUserAndGuildConfig(GuildOptions? guildConfig)
        {
            var guildId = guildConfig?.Id ?? 1ul;

            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(_guildUserMock.Object)
                .Verifiable();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedById(guildId))
                .ReturnsAsync(guildConfig)
                .Verifiable();
        }

        public void SetupTextChannel(ulong channelId)
        {
            _commandContextMock
                .Setup(pr => pr.Channel)
                .Returns(_textChannelMock.Object)
                .Verifiable();

            _textChannelMock
                .Setup(pr => pr.Id)
                .Returns(channelId);

            _textChannelMock
                .Setup(pr => pr.Mention)
                .Returns("test channel");
        }

        [TestMethod]
        public async Task Should_Return_Success_No_Guild()
        {
            var guildConfig = null as GuildOptions;
            SetupUserAndGuildConfig(guildConfig);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Return_Success_Not_Guild_User()
        {
            _commandContextMock
                .Setup(pr => pr.User)
                .Returns<IUser?>(null)
                .Verifiable();

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Return_Success_No_Channels()
        {
            var guildConfig = new GuildOptions(1, 50);
            SetupUserAndGuildConfig(guildConfig);
            SetupTextChannel(1ul);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Return_Success_Channel_Exists()
        {
            var guildConfig = new GuildOptions(1, 50);
            var channelId = 1ul;
            guildConfig.CommandChannels = new List<CommandChannel>
            {
                new CommandChannel
                {
                    ChannelId = channelId,
                },
            };

            SetupUserAndGuildConfig(guildConfig);
            SetupTextChannel(channelId);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Return_Success_Administrator()
        {
            var guildConfig = new GuildOptions(1, 50);
            guildConfig.CommandChannels = new List<CommandChannel>();

            SetupUserAndGuildConfig(guildConfig);
            SetupTextChannel(2ul);

            _guildUserMock
                .Setup(pr => pr.GuildPermissions)
                .Returns(new GuildPermissions(administrator: true));

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Return_Success_User_Level()
        {
            var user = new User(1ul, DateTime.UtcNow);
            user.Level = 40;
            var guildConfig = new GuildOptions(1, 50);
            var channelId = 1ul;
            guildConfig.CommandChannels = new List<CommandChannel>()
            {
                new CommandChannel()
                {
                    ChannelId = channelId,
                }
            };

            SetupUserAndGuildConfig(guildConfig);
            SetupTextChannel(2ul);

            _guildUserMock
                .Setup(pr => pr.GuildPermissions)
                .Returns(new GuildPermissions(administrator: false));

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userRepositoryMock
                .Setup(pr => pr.GetBasicAsync(user.Id))
                .ReturnsAsync(user);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Return_Error()
        {
            var user = new User(1ul, DateTime.UtcNow);
            var channelId = 1ul;
            var guildConfig = new GuildOptions(1, 50);
            guildConfig.CommandChannels = new List<CommandChannel>()
            {
                new CommandChannel()
                {
                    ChannelId = channelId,
                }
            };

            SetupUserAndGuildConfig(guildConfig);
            SetupTextChannel(2ul);

            _guildUserMock
                .Setup(pr => pr.GuildPermissions)
                .Returns(new GuildPermissions(administrator: false));

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userRepositoryMock
                .Setup(pr => pr.GetBasicAsync(user.Id))
                .ReturnsAsync(user);

            _guildMock
                .Setup(pr => pr.GetTextChannelAsync(channelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_textChannelMock.Object)
                .Verifiable();

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();
        }
    }
}
