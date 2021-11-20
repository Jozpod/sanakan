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
    [TestClass]
    public class HaremTypeReaderTests
    {
        private readonly HaremTypeReader _typeReader = new();
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly IServiceProvider _serviceProvider;

        public HaremTypeReaderTests()
        {
            var serviceCollection = new ServiceCollection();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        [DataRow("rarity", HaremType.Rarity)]
        [DataRow("jakość", HaremType.Rarity)]
        [DataRow("jakośc", HaremType.Rarity)]
        [DataRow("jakosc", HaremType.Rarity)]
        [DataRow("jakosć", HaremType.Rarity)]
        [DataRow("def", HaremType.Defence)]
        [DataRow("obrona", HaremType.Defence)]
        [DataRow("defence", HaremType.Defence)]
        [DataRow("atk", HaremType.Attack)]
        [DataRow("atak", HaremType.Attack)]
        [DataRow("attack", HaremType.Attack)]
        [DataRow("cage", HaremType.Cage)]
        [DataRow("klatka", HaremType.Cage)]
        [DataRow("relacja", HaremType.Affection)]
        [DataRow("affection", HaremType.Affection)]
        [DataRow("hp", HaremType.Health)]
        [DataRow("życie", HaremType.Health)]
        [DataRow("zycie", HaremType.Health)]
        [DataRow("health", HaremType.Health)]
        [DataRow("tag", HaremType.Tag)]
        [DataRow("tag+", HaremType.Tag)]
        [DataRow("tag-", HaremType.NoTag)]
        [DataRow("blocked", HaremType.Blocked)]
        [DataRow("inconvertible", HaremType.Blocked)]
        [DataRow("niewymienialne", HaremType.Blocked)]
        [DataRow("broken", HaremType.Broken)]
        [DataRow("uszkodzone", HaremType.Broken)]
        [DataRow("image", HaremType.Picture)]
        [DataRow("obrazek", HaremType.Picture)]
        [DataRow("picture", HaremType.Picture)]
        [DataRow("image-", HaremType.NoPicture)]
        [DataRow("obrazek-", HaremType.NoPicture)]
        [DataRow("picture-", HaremType.NoPicture)]
        [DataRow("imagec", HaremType.CustomPicture)]
        [DataRow("obrazekc", HaremType.CustomPicture)]
        [DataRow("picturec", HaremType.CustomPicture)]
        [DataRow("unikat", HaremType.Unique)]
        [DataRow("unique", HaremType.Unique)]
        public async Task Should_Parse_Values(string input, HaremType haremType)
        {
            var result = await _typeReader.ReadAsync(_commandContextMock.Object, input, _serviceProvider);
            result.BestMatch.Should().Be(haremType);
        }
    }
}
