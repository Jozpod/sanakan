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
using Sanakan.Game.Models;
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
    public class TopTypeReaderTests
    {
        private readonly TopTypeReader _typeReader = new();
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly IServiceProvider _serviceProvider;

        public TopTypeReaderTests()
        {
            var serviceCollection = new ServiceCollection();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        [DataRow("lvl", TopType.Level)]
        [DataRow("level", TopType.Level)]
        [DataRow("poziom", TopType.Level)]
        [DataRow("sc", TopType.ScCnt)]
        [DataRow("funds", TopType.ScCnt)]
        [DataRow("wallet", TopType.ScCnt)]
        [DataRow("wallet sc", TopType.ScCnt)]
        [DataRow("tc", TopType.TcCnt)]
        [DataRow("ac", TopType.AcCnt)]
        [DataRow("pc", TopType.PcCnt)]
        [DataRow("posty", TopType.Posts)]
        [DataRow("msg", TopType.Posts)]
        [DataRow("wiadomosci", TopType.Posts)]
        [DataRow("wiadomości", TopType.Posts)]
        [DataRow("messages", TopType.Posts)]
        [DataRow("postym", TopType.PostsMonthly)]
        [DataRow("msgm", TopType.PostsMonthly)]
        [DataRow("wiadomoscim", TopType.PostsMonthly)]
        [DataRow("wiadomościm", TopType.PostsMonthly)]
        [DataRow("messagesm", TopType.PostsMonthly)]
        [DataRow("postyms", TopType.PostsMonthlyCharacter)]
        [DataRow("msgmavg", TopType.PostsMonthlyCharacter)]
        [DataRow("wiadomoscims", TopType.PostsMonthlyCharacter)]
        [DataRow("wiadomościms", TopType.PostsMonthlyCharacter)]
        [DataRow("messagesmabg", TopType.PostsMonthlyCharacter)]
        [DataRow("command", TopType.Commands)]
        [DataRow("commands", TopType.Commands)]
        [DataRow("polecenia", TopType.Commands)]
        [DataRow("karta", TopType.Card)]
        [DataRow("card", TopType.Card)]
        [DataRow("karty", TopType.Cards)]
        [DataRow("cards", TopType.Cards)]
        [DataRow("kartym", TopType.CardsPower)]
        [DataRow("cardsp", TopType.CardsPower)]
        [DataRow("karma", TopType.Karma)]
        [DataRow("karma+", TopType.Karma)]
        [DataRow("+karma", TopType.PvpSeason)]
        [DataRow("karma-", TopType.KarmaNegative)]
        [DataRow("-karma", TopType.KarmaNegative)]
        [DataRow("pvp", TopType.Pvp)]
        [DataRow("pvps", TopType.PvpSeason)]
        public async Task Should_Parse_Values(string input, TopType topType)
        {
            var result = await _typeReader.ReadAsync(_commandContextMock.Object, input, _serviceProvider);
            result.BestMatch.Should().Be(topType);
        }
    }
}
