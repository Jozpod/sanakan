using Discord.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Sanakan.Preconditions;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.DAL.Repositories.Abstractions;
using Discord;
using Sanakan.DAL.Models.Configuration;
using FluentAssertions;

namespace Sanakan.DiscordBot.Tests.PreconditionsTests
{
    /// <summary>
    /// Defines tests for <see cref="RequireWaifuMarketChannel.CheckPermissionsAsync(ICommandContext, CommandInfo, System.IServiceProvider)"/> event handler.
    /// </summary>
    [TestClass]
    public class RequireWaifuMarketChannelTests
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly RequireWaifuMarketChannel _preconditionAttribute;
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly Mock<IGuild> _guildMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);

        public RequireWaifuMarketChannelTests()
        {
            _preconditionAttribute = new();
        }

        [TestMethod]
        public async Task Should_Return_Success_No_Channel()
        {
            var guildConfig = new GuildOptions(1, 50);

            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(_guildUserMock.Object)
                .Verifiable();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildConfig.Id))
                .ReturnsAsync(guildConfig)
                .Verifiable();

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();

            _guildConfigRepositoryMock.Verify();
        }

        [TestMethod]
        public async Task Should_Return_Error_Only_Server()
        {
            
            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
        }
    }
}
