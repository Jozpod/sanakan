using Discord.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Models;
using Sanakan.TypeReaders;
using System;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileTypeReader.ReadAsync(ICommandContext, string, IServiceProvider)"/> method.
    /// </summary>
    [TestClass]
    public class ProfileTypeReaderTests
    {
        private readonly ProfileTypeReader _typeReader = new();
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly IServiceProvider _serviceProvider;

        public ProfileTypeReaderTests()
        {
            var serviceCollection = new ServiceCollection();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        [DataRow("0", ProfileType.Statistics)]
        [DataRow("stats", ProfileType.Statistics)]
        [DataRow("statystyki", ProfileType.Statistics)]
        [DataRow("1", ProfileType.Image)]
        [DataRow("image", ProfileType.Image)]
        [DataRow("obrazek", ProfileType.Image)]
        [DataRow("2", ProfileType.StatisticsWithImage)]
        [DataRow("ugly", ProfileType.StatisticsWithImage)]
        [DataRow("brzydkie", ProfileType.StatisticsWithImage)]
        [DataRow("3", ProfileType.Cards)]
        [DataRow("cards", ProfileType.Cards)]
        [DataRow("karcianka", ProfileType.Cards)]
        public async Task Should_Parse_Values(string input, ProfileType profileType)
        {
            var result = await _typeReader.ReadAsync(_commandContextMock.Object, input, _serviceProvider);
            result.BestMatch.Should().Be(profileType);
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
