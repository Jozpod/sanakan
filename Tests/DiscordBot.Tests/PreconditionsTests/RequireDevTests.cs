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
    /// Defines tests for <see cref="RequireDev.CheckPermissionsAsync(ICommandContext, CommandInfo, System.IServiceProvider)"/> method.
    /// </summary>
    [TestClass]
    public class RequireDevTests
    {
        private readonly RequireDev _preconditionAttribute;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly Mock<IUser> _userMock = new(MockBehavior.Strict);
        private readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);
        
        public RequireDevTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(_discordConfigurationMock.Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(_userMock.Object);

            _preconditionAttribute = new();
        }

        [TestMethod]
        public async Task Should_Return_Success()
        {
            var userId = 1ul;
            var configuration = new DiscordConfiguration();
            configuration.AllowedToDebug.Add(userId);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            _discordConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(configuration);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Return_Error_Not_Dev()
        {
            var configuration = new DiscordConfiguration();

            _userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _discordConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(configuration);

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();
        }
    }
}
