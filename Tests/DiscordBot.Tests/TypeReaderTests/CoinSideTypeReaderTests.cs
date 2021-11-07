using Discord.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DiscordBot.Services;
using Sanakan.Services.Commands;
using Sanakan.TypeReaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests
{
    [TestClass]
    public class CoinSideTypeReaderTests
    {
        private readonly CoinSideTypeReader _typeReader = new();
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly IServiceProvider _serviceProvider;

        public CoinSideTypeReaderTests()
        {
            var serviceCollection = new ServiceCollection();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        [DataRow("orzel", CoinSide.Head)]
        [DataRow("head", CoinSide.Head)]
        [DataRow("tail", CoinSide.Tail)]
        [DataRow("reszka", CoinSide.Tail)]
        public async Task Should_Parse_Values(string input, CoinSide coinSide)
        {
            var result = await _typeReader.ReadAsync(_commandContextMock.Object, input, _serviceProvider);
            result.BestMatch.Should().Be(coinSide);
        }
    }
}
