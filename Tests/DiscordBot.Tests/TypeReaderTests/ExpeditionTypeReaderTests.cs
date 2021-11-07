using Discord.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models;
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
    public class ExpeditionTypeReaderTests
    {
        private readonly ExpeditionTypeReader _typeReader = new();
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly IServiceProvider _serviceProvider;

        public ExpeditionTypeReaderTests()
        {
            var serviceCollection = new ServiceCollection();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        [DataRow("-", ExpeditionCardType.None)]
        [DataRow("normalna", ExpeditionCardType.NormalItemWithExp)]
        [DataRow("normal", ExpeditionCardType.NormalItemWithExp)]
        [DataRow("n", ExpeditionCardType.NormalItemWithExp)]
        [DataRow("trudna", ExpeditionCardType.ExtremeItemWithExp)]
        [DataRow("hard", ExpeditionCardType.ExtremeItemWithExp)]
        [DataRow("h", ExpeditionCardType.ExtremeItemWithExp)]
        [DataRow("mrok", ExpeditionCardType.DarkItemWithExp)]
        [DataRow("dark", ExpeditionCardType.DarkItemWithExp)]
        [DataRow("d", ExpeditionCardType.DarkItemWithExp)]
        [DataRow("mrok 1", ExpeditionCardType.DarkExp)]
        [DataRow("dark 1", ExpeditionCardType.DarkExp)]
        [DataRow("d1", ExpeditionCardType.DarkExp)]
        [DataRow("mrok 2", ExpeditionCardType.DarkItems)]
        [DataRow("dark 2", ExpeditionCardType.DarkItems)]
        [DataRow("d2", ExpeditionCardType.DarkItems)]
        [DataRow("światło", ExpeditionCardType.LightItemWithExp)]
        [DataRow("światlo", ExpeditionCardType.LightItemWithExp)]
        [DataRow("swiatło", ExpeditionCardType.LightItemWithExp)]
        [DataRow("swiatlo", ExpeditionCardType.LightItemWithExp)]
        [DataRow("light", ExpeditionCardType.LightItemWithExp)]
        [DataRow("l", ExpeditionCardType.LightItemWithExp)]
        [DataRow("światło 1", ExpeditionCardType.LightExp)]
        [DataRow("światlo 1", ExpeditionCardType.LightExp)]
        [DataRow("swiatło 1", ExpeditionCardType.LightExp)]
        [DataRow("swiatlo 1", ExpeditionCardType.LightExp)]
        [DataRow("light 1", ExpeditionCardType.LightExp)]
        [DataRow("l1", ExpeditionCardType.LightExp)]
        [DataRow("światło 2", ExpeditionCardType.LightItems)]
        [DataRow("światlo 2", ExpeditionCardType.LightItems)]
        [DataRow("swiatło 2", ExpeditionCardType.LightItems)]
        [DataRow("swiatlo 2", ExpeditionCardType.LightItems)]
        [DataRow("light 2", ExpeditionCardType.LightItems)]
        [DataRow("l2", ExpeditionCardType.LightItems)]
        [DataRow("ue", ExpeditionCardType.UltimateEasy)]
        [DataRow("um", ExpeditionCardType.UltimateMedium)]
        [DataRow("uh", ExpeditionCardType.UltimateHard)]
        [DataRow("uhh", ExpeditionCardType.UltimateHardcore)]
        public async Task Should_Parse_Values(string input, ExpeditionCardType expeditionCardType)
        {
            var result = await _typeReader.ReadAsync(_commandContextMock.Object, input, _serviceProvider);
            result.BestMatch.Should().Be(expeditionCardType);
        }
    }
}
