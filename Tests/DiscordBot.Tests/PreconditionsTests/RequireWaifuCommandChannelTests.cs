using Discord.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Sanakan.Preconditions;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Sanakan.DAL.Repositories.Abstractions;
using FluentAssertions;
using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DiscordBot.Tests.PreconditionsTests
{
    /// <summary>
    /// Defines tests for <see cref="RequireWaifuCommandChannel.CheckPermissionsAsync(ICommandContext, CommandInfo, System.IServiceProvider)"/> event handler.
    /// </summary>
    [TestClass]
    public class RequireWaifuCommandChannelTests
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly RequireWaifuCommandChannel _preconditionAttribute;
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly Mock<ITextChannel> _textChannelMock = new(MockBehavior.Strict);
        private readonly Mock<IGuild> _guildMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);


        public RequireWaifuCommandChannelTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_guildConfigRepositoryMock.Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();

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
        public async Task Should_Return_Success_No_Guild_User()
        {
            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(null as IUser)
                .Verifiable();

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();

            _commandContextMock.Verify();
        }

        [TestMethod]
        public async Task Should_Return_Success_No_Guild()
        {
            SetupUserAndGuildConfig(null);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();

            _commandContextMock.Verify();
        }

        [TestMethod]
        public async Task Should_Return_Success_No_Configured_Channels()
        {
            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(null as IUser)
                .Verifiable();

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();

            _commandContextMock.Verify();
        }

        [TestMethod]
        public async Task Should_Return_Success_Channel_Exists()
        {
            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(null as IUser)
                .Verifiable();

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();

            _commandContextMock.Verify();
        }

        [TestMethod]
        public async Task Should_Return_Success_Adiministrator()
        {
            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(null as IUser)
                .Verifiable();

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();

            _commandContextMock.Verify();
        }

        [TestMethod]
        public async Task Should_Return_Success_Channel_Exists_WaifuConfig()
        {
            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(null as IUser)
                .Verifiable();

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();

            _commandContextMock.Verify();
        }

        [TestMethod]
        public async Task Should_Return_Error()
        {
            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(null as IUser)
                .Verifiable();

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();

            _commandContextMock.Verify();
        }
    }
}
