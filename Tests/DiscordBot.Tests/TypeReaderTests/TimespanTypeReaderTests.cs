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
    /// Defines tests for <see cref="TimespanTypeReader.ReadAsync(ICommandContext, string, IServiceProvider)"/> method.
    /// </summary>
    [TestClass]
    public class TimespanTypeReaderTests
    {
        private readonly TimespanTypeReader _typeReader = new();
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly IServiceProvider _serviceProvider;

        public TimespanTypeReaderTests()
        {
            var serviceCollection = new ServiceCollection();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        [DataRow("10:10", true)]
        [DataRow("test", false)]
        public async Task Should_Parse_Values(string input, bool valid)
        {
            var result = await _typeReader.ReadAsync(_commandContextMock.Object, input, _serviceProvider);
            if (valid)
            {
                result.BestMatch.Should().NotBeNull();
            }
            else
            {
                result.ErrorReason.Should().NotBeNullOrEmpty();
            }
        }

        [TestMethod]
        public async Task Should_Return_Error()
        {
            var input = "test";
            var result = await _typeReader.ReadAsync(_commandContextMock.Object, input, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();
        }
    }
}
