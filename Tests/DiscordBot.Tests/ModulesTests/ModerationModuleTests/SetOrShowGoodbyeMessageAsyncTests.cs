using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.SetOrShowGoodbyeMessageAsync(string?)"/> method.
    /// </summary>
    [TestClass]
    public class SetOrShowGoodbyeMessageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Current()
        {
            var guildOptions = new GuildOptions(1ul, 50);
            guildOptions.GoodbyeMessage = "test";

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetGuildConfigOrCreateAsync(guildOptions.Id))
                .ReturnsAsync(guildOptions);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.SetOrShowGoodbyeMessageAsync(null);
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_Too_Long()
        {
            var guildOptions = new GuildOptions(1ul, 50);
            var message = new string(Enumerable.Repeat('a', 200).ToArray());

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

            await _module.SetOrShowGoodbyeMessageAsync(message);
        }

        [TestMethod]
        public async Task Should_Set_Goodbye_Message_And_Send_Confirm_Message()
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
