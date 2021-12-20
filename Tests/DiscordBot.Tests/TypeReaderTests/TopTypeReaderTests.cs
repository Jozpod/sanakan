using Discord.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Game.Models;
using Sanakan.TypeReaders;
using System;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests
{
    /// <summary>
    /// Defines tests for <see cref="TopTypeReader.ReadAsync(ICommandContext, string, IServiceProvider)"/> method.
    /// </summary>
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
        [DataRow("sc", TopType.ScCount)]
        [DataRow("funds", TopType.ScCount)]
        [DataRow("wallet", TopType.ScCount)]
        [DataRow("wallet sc", TopType.ScCount)]
        [DataRow("tc", TopType.TcCount)]
        [DataRow("ac", TopType.AcCount)]
        [DataRow("pc", TopType.PcCount)]
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
        [DataRow("+karma", TopType.Karma)]
        [DataRow("karma-", TopType.KarmaNegative)]
        [DataRow("-karma", TopType.KarmaNegative)]
        [DataRow("pvp", TopType.Pvp)]
        [DataRow("pvps", TopType.PvpSeason)]
        public async Task Should_Parse_Values(string input, TopType topType)
        {
            var result = await _typeReader.ReadAsync(_commandContextMock.Object, input, _serviceProvider);
            result.BestMatch.Should().Be(topType);
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
