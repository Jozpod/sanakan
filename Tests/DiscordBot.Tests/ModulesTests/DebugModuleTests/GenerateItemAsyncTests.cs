using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;
using Sanakan.DAL.Models;
using Discord;
using System.Threading;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.GenerateItemAsync(Discord.IGuildUser,ItemType, uint, Quality)"/> method.
    /// </summary>
    [TestClass]
    public class GenerateItemAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Generate_Item_And_Send_Confirm_Message()
        {
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var itemType = ItemType.AffectionRecoveryBig;
            var amount = 1000u;
            var quality = Quality.Alpha;
            var user = new User(1ul, DateTime.UtcNow);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.GenerateItemAsync(guildUserMock.Object, itemType, amount, quality);
        }
    }
}
