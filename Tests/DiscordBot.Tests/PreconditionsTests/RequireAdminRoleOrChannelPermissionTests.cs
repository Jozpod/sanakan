using Discord;
using Discord.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Sanakan.Preconditions;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.DAL.Repositories.Abstractions;
using FluentAssertions;
using Sanakan.DAL.Models.Configuration;
using System.Collections.Generic;

namespace Sanakan.DiscordBot.Tests.PreconditionsTests
{
    /// <summary>
    /// Defines tests for <see cref="RequireAdminRoleOrChannelPermission.CheckPermissionsAsync(ICommandContext, CommandInfo, System.IServiceProvider)"/> method.
    /// </summary>
    [TestClass]
    public class RequireAdminRoleOrChannelPermissionTests
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly RequireAdminRoleOrChannelPermission _preconditionAttribute;
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly Mock<IMessageChannel> _messageChannelMock = new(MockBehavior.Strict);
        private readonly Mock<IGuild> _guildMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);
        private readonly Mock<IRole> _roleMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        private Mock<IGuildChannel> _guildChannelMock;

        public RequireAdminRoleOrChannelPermissionTests()
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

            _guildChannelMock = _messageChannelMock.As<IGuildChannel>();

            _commandContextMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _preconditionAttribute = new(ChannelPermission.ManageMessages);
        }

        [TestMethod]
        public async Task Should_Return_Error_Only_Server_No_Guild_User()
        {
            _commandContextMock
                 .Setup(pr => pr.User)
                 .Returns(null as IUser);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public async Task Should_Return_Error_Only_Server_No_Guild_Channel()
        {
            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(_guildUserMock.Object);

            _commandContextMock
                .Setup(pr => pr.Channel)
                .Returns(null as IMessageChannel);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public async Task Should_Return_Error()
        {
            var guildConfig = new GuildOptions(1, 50);

            _commandContextMock
                 .Setup(pr => pr.User)
                 .Returns(_guildUserMock.Object);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildConfig.Id))
                .ReturnsAsync(guildConfig);

            _guildUserMock
                .Setup(pr => pr.GuildPermissions)
                .Returns(new GuildPermissions(administrator: false));

            _guildUserMock
                .Setup(pr => pr.GetPermissions(_guildChannelMock.Object))
                .Returns(ChannelPermissions.None);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public async Task Should_Return_Success_Administrator()
        {
            var guildConfig = new GuildOptions(1, 50);

            _commandContextMock
                 .Setup(pr => pr.User)
                 .Returns(_guildUserMock.Object);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildConfig.Id))
                .ReturnsAsync(guildConfig);

            _guildUserMock
                .Setup(pr => pr.GuildPermissions)
                .Returns(new GuildPermissions(administrator: true));

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Return_Success_Has_Permission()
        {
            var guildConfig = null as GuildOptions;

            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(_guildUserMock.Object);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(1ul))
                .ReturnsAsync(guildConfig);

            _guildUserMock
                .Setup(pr => pr.GuildPermissions)
                .Returns(new GuildPermissions(administrator: false));

            _guildUserMock
                .Setup(pr => pr.GetPermissions(_guildChannelMock.Object))
                .Returns(new ChannelPermissions(manageMessages: true));

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Return_Success_Has_Role()
        {
            var roleId = 1ul;
            var roleIds = new List<ulong> { roleId };
            var guildConfig = new GuildOptions(1, 50);
            guildConfig.AdminRoleId = roleId;

            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(_guildUserMock.Object);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildConfig.Id))
                .ReturnsAsync(guildConfig);

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
    }
}
