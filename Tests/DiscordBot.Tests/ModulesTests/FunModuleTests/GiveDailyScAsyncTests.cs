using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="FunModule.GiveDailyScAsync"/> method.
    /// </summary>
    [TestClass]
    public class GiveDailyScAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Confirming_Transaction()
        {
            var discordUserId = 1ul;
            var utcNow = DateTime.UtcNow;
            var user = new User(discordUserId, utcNow);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(discordUserId);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(discordUserId))
                .ReturnsAsync(user);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()))
                .Verifiable();

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.GiveDailyScAsync();
        }
    }
}
