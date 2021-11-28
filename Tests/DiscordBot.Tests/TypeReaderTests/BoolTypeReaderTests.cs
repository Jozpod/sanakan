using Discord.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.TypeReaders;
using System;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests
{
    /// <summary>
    /// Defines tests for <see cref="BoolTypeReader.ReadAsync(ICommandContext, string, IServiceProvider)"/> method.
    /// </summary>
    [TestClass]
    public class BoolTypeReaderTests
    {
        private readonly BoolTypeReader _typeReader = new();
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly IServiceProvider _serviceProvider;

        public BoolTypeReaderTests()
        {
            var serviceCollection = new ServiceCollection();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        [DataRow("1", true)]
        [DataRow("tak", true)]
        [DataRow("true", true)]
        [DataRow("prawda", true)]
        [DataRow("0", false)]
        [DataRow("nie", false)]
        [DataRow("false", false)]
        [DataRow("fałsz", false)]
        [DataRow("falsz", false)]
        public async Task Should_Parse_Values(string input, bool value)
        {
            var result = await _typeReader.ReadAsync(_commandContextMock.Object, input, _serviceProvider);
            result.BestMatch.Should().Be(value);
        }
    }
}
