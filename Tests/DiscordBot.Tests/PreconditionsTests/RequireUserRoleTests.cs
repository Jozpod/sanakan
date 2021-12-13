using Discord.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Sanakan.Preconditions;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DAL.Models.Configuration;
using System.Collections.Generic;
using FluentAssertions;

namespace Sanakan.DiscordBot.Tests.PreconditionsTests
{
    /// <summary>
    /// Defines tests for <see cref="RequireUserRole.CheckPermissionsAsync(ICommandContext, CommandInfo, System.IServiceProvider)"/> method.
    /// </summary>
    [TestClass]
    public class RequireUserRoleTests
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly RequireUserRole _preconditionAttribute;
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly Mock<IGuild> _guildMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<IRole> _roleMock = new(MockBehavior.Strict);

        public RequireUserRoleTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
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
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildConfig)
                .Verifiable();
        }

        [TestMethod]
        public async Task Should_Return_Success_Role_Exists()
        {
            var roleId = 1ul;
            var guildConfig = new GuildOptions(1, 50);
            guildConfig.UserRoleId = roleId;
            var roleIds = new List<ulong>() { roleId };
            SetupUserAndGuildConfig(guildConfig);

            _guildMock
                .Setup(pr => pr.GetRole(roleId))
                .Returns(_roleMock.Object)
                .Verifiable();

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            _roleMock
                .Setup(pr => pr.Id)
                .Returns(roleId);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();

            _commandContextMock.Verify();
            _guildConfigRepositoryMock.Verify();
        }

        [TestMethod]
        public async Task Should_Return_Success_No_Guild()
        {
            var guildConfig = null as GuildOptions;
            SetupUserAndGuildConfig(guildConfig);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();

            _commandContextMock.Verify();
            _guildConfigRepositoryMock.Verify();
        }

        [TestMethod]
        public async Task Should_Return_Success_Administrator()
        {
            var guildConfig = new GuildOptions(1, 50);
            var roleId = 1ul;
            guildConfig.UserRoleId = roleId;
            var roleIds = new List<ulong>();
            SetupUserAndGuildConfig(guildConfig);

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            _guildUserMock
                .Setup(pr => pr.GuildPermissions)
                .Returns(new GuildPermissions(administrator: true));

            _guildMock
                .Setup(pr => pr.GetRole(guildConfig.UserRoleId.Value))
                .Returns(_roleMock.Object)
                .Verifiable();

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();

            _commandContextMock.Verify();
            _guildConfigRepositoryMock.Verify();
        }

        [TestMethod]
        public async Task Should_Return_Success_No_Configured_Role()
        {
            var guildConfig = new GuildOptions(1, 50);

            SetupUserAndGuildConfig(guildConfig);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();

            _guildMock.Verify();
            _commandContextMock.Verify();
            _guildConfigRepositoryMock.Verify();
        }

        [TestMethod]
        public async Task Should_Return_Error_User_Has_No_Role()
        {
            var guildConfig = new GuildOptions(1, 50);
            guildConfig.UserRoleId = 1ul;
            var roleIds = new List<ulong>();

            SetupUserAndGuildConfig(guildConfig);

            _guildUserMock
               .Setup(pr => pr.RoleIds)
               .Returns(roleIds);

            _guildUserMock
                .Setup(pr => pr.GuildPermissions)
                .Returns(new GuildPermissions(administrator: false));

            _guildMock
                .Setup(pr => pr.GetRole(guildConfig.UserRoleId.Value))
                .Returns(_roleMock.Object)
                .Verifiable();

            _roleMock
                .Setup(pr => pr.Mention)
                .Returns("test role");

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();

            _guildMock.Verify();
            _commandContextMock.Verify();
            _guildConfigRepositoryMock.Verify();
        }

        [TestMethod]
        public async Task Should_Return_Success_No_Role()
        {
            var guildConfig = new GuildOptions(1, 50);
            guildConfig.UserRoleId = 1ul;

            SetupUserAndGuildConfig(guildConfig);

            _guildUserMock
                .Setup(pr => pr.GuildPermissions)
                .Returns(new GuildPermissions(administrator: false));

            _guildMock
                .Setup(pr => pr.GetRole(guildConfig.UserRoleId.Value))
                .Returns<IRole?>(null)
                .Verifiable();

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();

            _guildMock.Verify();
            _commandContextMock.Verify();
            _guildConfigRepositoryMock.Verify();
        }

        [TestMethod]
        public async Task Should_Return_Error_Only_Server()
        {
            _commandContextMock
                .Setup(pr => pr.User)
                .Returns<IUser?>(null)
                .Verifiable();

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();

            _commandContextMock.Verify();
        }
    }
}
