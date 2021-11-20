using Discord.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Services;
using Sanakan.TypeReaders;
using System;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests
{
    [TestClass]
    public class ConfigTypeReaderTests
    {
        private readonly ConfigTypeReader _typeReader = new();
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly IServiceProvider _serviceProvider;

        public ConfigTypeReaderTests()
        {
            var serviceCollection = new ServiceCollection();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        [DataRow("wfight", ConfigType.WaifuFightChannels)]
        [DataRow("waifufight", ConfigType.WaifuFightChannels)]
        [DataRow("waifu fight", ConfigType.WaifuFightChannels)]
        [DataRow("wcmd", ConfigType.WaifuCmdChannels)]
        [DataRow("waifucmd", ConfigType.WaifuCmdChannels)]
        [DataRow("waifu cmd", ConfigType.WaifuCmdChannels)]
        [DataRow("mods", ConfigType.ModeratorRoles)]
        [DataRow("wexp", ConfigType.NonExpChannels)]
        [DataRow("nonexp", ConfigType.NonExpChannels)]
        [DataRow("nonexpchannel", ConfigType.NonExpChannels)]
        [DataRow("nonexpchannels", ConfigType.NonExpChannels)]
        [DataRow("ignch", ConfigType.IgnoredChannels)]
        [DataRow("nomsgcnt", ConfigType.IgnoredChannels)]
        [DataRow("ignoredchannel", ConfigType.IgnoredChannels)]
        [DataRow("ignoredchannels", ConfigType.IgnoredChannels)]
        [DataRow("wsup", ConfigType.NonSupChannels)]
        [DataRow("nosup", ConfigType.NonSupChannels)]
        [DataRow("nonsupchannel", ConfigType.NonSupChannels)]
        [DataRow("nonsupchannels", ConfigType.NonSupChannels)]
        [DataRow("cmd", ConfigType.CommandChannels)]
        [DataRow("cmdchannel", ConfigType.CommandChannels)]
        [DataRow("cmdchannels", ConfigType.CommandChannels)]
        [DataRow("role", ConfigType.LevelRoles)]
        [DataRow("roles", ConfigType.LevelRoles)]
        [DataRow("levelrole", ConfigType.LevelRoles)]
        [DataRow("levelroles", ConfigType.LevelRoles)]
        [DataRow("land", ConfigType.Lands)]
        [DataRow("lands", ConfigType.Lands)]
        [DataRow("selfrole", ConfigType.SelfRoles)]
        [DataRow("selfroles", ConfigType.SelfRoles)]
        [DataRow("rmessage", ConfigType.RichMessages)]
        [DataRow("rmessages", ConfigType.RichMessages)]
        [DataRow("richmessage", ConfigType.RichMessages)]
        [DataRow("richmessages", ConfigType.RichMessages)]
        [DataRow("all", ConfigType.Global)]
        [DataRow("full", ConfigType.Global)]
        [DataRow("global", ConfigType.Global)]
        public async Task Should_Parse_Values(string input, ConfigType configType)
        {
            var result = await _typeReader.ReadAsync(_commandContextMock.Object, input, _serviceProvider);
            result.BestMatch.Should().Be(configType);
        }
    }
}
