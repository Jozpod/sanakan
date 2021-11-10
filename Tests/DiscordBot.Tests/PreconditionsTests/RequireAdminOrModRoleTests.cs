using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Discord.Rest;
using System.IO;
using Sanakan.Preconditions;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DAL.Models.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sanakan.DiscordBot.Tests.PreconditionsTests
{
    [TestClass]
    public class RequireAdminOrModRoleTests
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly RequireAdminOrModRole _preconditionAttribute;
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly Mock<IMessageChannel> _messageChannelMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);
        private readonly Mock<IGuild> _guildMock = new(MockBehavior.Strict);
        private readonly Mock<IRole> _roleMock = new(MockBehavior.Strict);

        public RequireAdminOrModRoleTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            _preconditionAttribute = new();
        }

        [TestMethod]
        public async Task Should_Return_Success_Moderator_Role()
        {
            var roleId = 1ul;
            var userId = 1ul;
            var guildConfig = new GuildOptions(1, 50)
            {
                ModeratorRoles = new List<ModeratorRoles>
                {
                    new ModeratorRoles
                    {
                        RoleId = roleId,
                    }
                }
            };

            var roleIds = new List<ulong> { roleId }.AsReadOnly();

            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(_guildUserMock.Object);

            _commandContextMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _commandContextMock
                .Setup(pr => pr.Guild)
                .Returns(_guildMock.Object);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildConfig.Id);

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildConfig.Id))
                .ReturnsAsync(guildConfig);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Return_Error_Only_Server()
        {
            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
        }

        [TestMethod]
        public async Task Should_Return_Error_No_Permission()
        {
            var roleId = 1ul;
            var userId = 1ul;
            var guildConfig = new GuildOptions(1, 50)
            {
                AdminRoleId = roleId,
            };

            var roleIds = new List<ulong> { roleId }.AsReadOnly();

            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(_guildUserMock.Object);

            _commandContextMock
                .Setup(pr => pr.Guild)
                .Returns(_guildMock.Object);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildConfig.Id);

            _guildMock
                .Setup(pr => pr.GetRole(roleId))
                .Returns(_roleMock.Object);

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildConfig.Id))
                .ReturnsAsync(guildConfig);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
        }
    }
}
