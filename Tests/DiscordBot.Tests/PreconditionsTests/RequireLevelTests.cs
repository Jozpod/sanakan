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
    /// Defines tests for <see cref="RequireLevel.CheckPermissionsAsync(ICommandContext, CommandInfo, System.IServiceProvider)"/> method.
    /// </summary>
    [TestClass]
    public class RequireLevelTests
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly RequireLevel _preconditionAttribute;
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly Mock<IGuild> _guildMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<IRole> _roleMock = new(MockBehavior.Strict);
        private readonly ulong _level = 10;

        public RequireLevelTests()
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

            _preconditionAttribute = new(_level);
        }

        public void SetupUserAndGuildConfig(GuildOptions? guildConfig)
        {
            var guildId = guildConfig?.Id ?? 1ul;

            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(_guildUserMock.Object);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildConfig);
        }

        [TestMethod]
        public async Task Should_Return_Success_Level_Met()
        {
            var guildConfig = new GuildOptions(1, 50);
            var roleIds = new List<ulong>();
            var user = new User(1ul, DateTime.UtcNow);
            user.Level = 10;
            SetupUserAndGuildConfig(guildConfig);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _guildUserMock
                .Setup(pr => pr.GuildPermissions)
                .Returns(new GuildPermissions(administrator: false));

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            _userRepositoryMock
                .Setup(pr => pr.GetBaseUserAndDontTrackAsync(user.Id))
                .ReturnsAsync(user);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Return_Success_Admin_Role()
        {
            var roleId = 1ul;
            var guildConfig = new GuildOptions(1, 50);
            guildConfig.AdminRoleId = roleId;
            var roleIds = new List<ulong>() { roleId };
            SetupUserAndGuildConfig(guildConfig);

            _guildUserMock
                .Setup(pr => pr.GuildPermissions)
                .Returns(new GuildPermissions(administrator: false));

            _guildMock
                .Setup(pr => pr.GetRole(roleId))
                .Returns(_roleMock.Object);

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            _roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Return_Success_Guild_Admin()
        {
            var roleId = 1ul;
            var guildConfig = new GuildOptions(1, 50);
            guildConfig.UserRoleId = roleId;
            var roleIds = new List<ulong>() { roleId };
            SetupUserAndGuildConfig(guildConfig);

            _guildUserMock
                .Setup(pr => pr.GuildPermissions)
                .Returns(new GuildPermissions(administrator: true));

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Return_Error_No_Database_User()
        {
            var guildConfig = new GuildOptions(1, 50);
            var roleIds = new List<ulong>();
            var userId = 1ul;
            SetupUserAndGuildConfig(guildConfig);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            _guildUserMock
                .Setup(pr => pr.GuildPermissions)
                .Returns(new GuildPermissions(administrator: false));

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            _userRepositoryMock
                .Setup(pr => pr.GetBaseUserAndDontTrackAsync(userId))
                .ReturnsAsync(null as User);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public async Task Should_Return_Error_Only_Server()
        {
            _commandContextMock
                .Setup(pr => pr.User)
                .Returns<IUser?>(null);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();
        }
    }
}
