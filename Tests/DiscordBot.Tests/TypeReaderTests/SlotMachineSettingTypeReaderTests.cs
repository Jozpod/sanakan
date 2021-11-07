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
    public class SlotMachineSettingTypeReaderTests
    {
        private readonly SlotMachineSettingTypeReader _typeReader = new();
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly IServiceProvider _serviceProvider;

        public SlotMachineSettingTypeReaderTests()
        {
            var serviceCollection = new ServiceCollection();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        [DataRow("info", SlotMachineSetting.Info)]
        [DataRow("informacje", SlotMachineSetting.Info)]
        [DataRow("beat", SlotMachineSetting.Beat)]
        [DataRow("stawka", SlotMachineSetting.Beat)]
        [DataRow("mnożnik", SlotMachineSetting.Multiplier)]
        [DataRow("mnoznik", SlotMachineSetting.Multiplier)]
        [DataRow("multiplier", SlotMachineSetting.Multiplier)]
        [DataRow("rows", SlotMachineSetting.Rows)]
        [DataRow("rzedy", SlotMachineSetting.Rows)]
        [DataRow("rzędy", SlotMachineSetting.Rows)]
        public async Task Should_Parse_Values(string input, SlotMachineSetting slotMachineSetting)
        {
            var result = await _typeReader.ReadAsync(_commandContextMock.Object, input, _serviceProvider);
            result.BestMatch.Should().Be(slotMachineSetting);
        }
    }
}
