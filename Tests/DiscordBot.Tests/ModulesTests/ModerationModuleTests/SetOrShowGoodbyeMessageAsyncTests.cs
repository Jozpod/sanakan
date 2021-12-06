using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;
using Sanakan.DAL.Models.Configuration;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.SetOrShowGoodbyeMessageAsync(string?)"/> method.
    /// </summary>
    [TestClass]
    public class SetOrShowGoodbyeMessageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message()
        {
            var guildOptions = new GuildOptions(1ul, 50);

            _guildConfigRepositoryMock
              .Setup(pr => pr.GetGuildConfigOrCreateAsync(guildOptions.Id))
              .ReturnsAsync(guildOptions);

            await _module.SetOrShowGoodbyeMessageAsync();
        }
    }
}
