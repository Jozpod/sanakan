using Discord;
using Discord.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Configuration;
using Sanakan.Preconditions;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests.PreconditionsTests
{
    /// <summary>
    /// Defines tests for <see cref="RequireGuildUser.CheckPermissionsAsync(ICommandContext, CommandInfo, System.IServiceProvider)"/> method.
    /// </summary>
    [TestClass]
    public class RequireGuildUserTests
    {
        private readonly RequireGuildUser _preconditionAttribute;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        
        public RequireGuildUserTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_discordConfigurationMock.Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            _preconditionAttribute = new();
        }

        [TestMethod]
        public async Task Should_Return_Success()
        {
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);

            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(guildUserMock.Object);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Return_Error()
        {
            var userMock = new Mock<IUser>(MockBehavior.Strict);

            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(userMock.Object);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();
        }
    }
}
