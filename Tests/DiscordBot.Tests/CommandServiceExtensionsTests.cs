using Discord;
using Discord.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Extensions;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests
{
    [TestClass]
    public class CommandServiceExtensionsTests
    {
        private readonly Mock<ICommandService> _commandServiceMock = new(MockBehavior.Strict);
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly Mock<IUserMessage> _userMessageMock = new(MockBehavior.Strict);

        public CommandServiceExtensionsTests()
        {
            _commandContextMock
                .Setup(pr => pr.Message)
                .Returns(_userMessageMock.Object);
        }

        [TestMethod]
        public async Task Should_GetExecutableCommandAsync_Handle_Error()
        {
            var argPos = 0;
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var commandError = CommandError.BadArgCount;
            var searchResult = Discord.Commands.SearchResult.FromError(commandError, "test");

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns("test");

            _commandServiceMock
                .Setup(pr => pr.Search(It.IsAny<string>()))
                .Returns(searchResult);

            var result = await _commandServiceMock.Object
                .GetExecutableCommandAsync(_commandContextMock.Object, argPos, serviceProvider);
            result.Should().NotBeNull();
            result.Result.Error.Should().Be(commandError);
        }

        //[TestMethod]
        //public async Task Should_GetExecutableCommandAsync()
        //{
        //    var argPos = 0;
        //    var serviceCollection = new ServiceCollection();
        //    var serviceProvider = serviceCollection.BuildServiceProvider();

        //    var moduleBuilder = new ModuleBuilder(); // TO-DO ctor internal
        //    var commandBuilder = new CommandBuilder(); // TO-DO ctor internal
        //    var module = new ModuleInfo(); // TO-DO ctor internal
        //    var service = new CommandService();

        //    var commandInfo = new CommandInfo(); // TO-DO ctor internal
        //    var commandMatch = new CommandMatch();
        //    var commandMatches = new List<CommandMatch>
        //    {
        //        commandMatch,
        //    };
        //    var searchResult = SearchResult.FromSuccess("command", commandMatches.AsReadOnly());

        //    _userMessageMock
        //        .Setup(pr => pr.Content)
        //        .Returns("test");

        //    _commandServiceMock
        //        .Setup(pr => pr.Search(It.IsAny<string>()))
        //        .Returns(searchResult);

        //    var result = await _commandServiceMock.Object
        //        .GetExecutableCommandAsync(_commandContextMock.Object, argPos, serviceProvider);
        //    result.Should().NotBeNull();
        //}
    }
}
