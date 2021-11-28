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
    /// Defines tests for <see cref="RequireWaifuMarketChannel.CheckPermissionsAsync(ICommandContext, CommandInfo, System.IServiceProvider)"/> method.
    /// </summary>
    [TestClass]
    public class RequireWaifuMarketChannelTests
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly RequireWaifuMarketChannel _preconditionAttribute;
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly Mock<ITextChannel> _textChannelMock = new(MockBehavior.Strict);
        private readonly Mock<IGuild> _guildMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);

        public RequireWaifuMarketChannelTests()
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
        public async Task Should_Return_Error_No_Guild_User()
        {
            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(null as IUser)
                .Verifiable();

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public async Task Should_Return_Success_No_Guild()
        {
            SetupUserAndGuildConfig(null);

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
            guildConfig.WaifuConfig = new WaifuConfiguration
            {
                MarketChannelId = channelId,
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
            var channelId = 1ul;

            SetupUserAndGuildConfig(guildConfig);
            SetupTextChannel(2ul);

            _guildUserMock
                .Setup(pr => pr.GuildPermissions)
                .Returns(new GuildPermissions(administrator: true));

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();
        }
    }
}
