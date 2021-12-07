using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;
using Sanakan.DAL.Models.Configuration;
using FluentAssertions;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.SetOrShowGoodbyeMessageAsync(string?)"/> method.
    /// </summary>
    [TestClass]
    public class SetOrShowGoodbyeMessageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Set_Goodbye_Message()
        {
            var guildOptions = new GuildOptions(1ul, 50);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetGuildConfigOrCreateAsync(guildOptions.Id))
                .ReturnsAsync(guildOptions);

            _guildConfigRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.SetOrShowGoodbyeMessageAsync("test message");
        }
    }
}
