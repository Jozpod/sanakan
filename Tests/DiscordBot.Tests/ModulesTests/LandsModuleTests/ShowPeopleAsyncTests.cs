using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.LandsModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="LandsModule.ShowPeopleAsync(string?)"/> method.
    /// </summary>
    [TestClass]
    public class ShowPeopleAsync : Base
    {  
        [TestMethod]
        public async Task Should_Return_Users()
        {
            var guildOptions = new GuildOptions(1ul, 50);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var userLand = new UserLand
            {
                Name = "test land",
                UnderlingId = 1ul,
            };
            var roleIds = new List<ulong> { userLand.UnderlingId };
            var embeds = new List<Embed>() { new EmbedBuilder().Build() };

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            _landManagerMock
                .Setup(pr => pr.DetermineLand(
                    It.IsAny<IEnumerable<UserLand>>(),
                    It.IsAny<IEnumerable<ulong>>(),
                    It.IsAny<string?>()))
                .Returns(userLand);

            _landManagerMock
                .Setup(pr => pr.GetMembersList(It.IsAny<UserLand>(), _guildMock.Object))
                .ReturnsAsync(embeds);

            guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _guildUserMock
                .Setup(pr => pr.RoleIds)
                .Returns(roleIds);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildOptions.Id))
                .ReturnsAsync(guildOptions);

            _taskManagerMock
                .Setup(pr => pr.Delay(It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.ShowPeopleAsync();
        }
    }
}
