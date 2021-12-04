using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;
using Sanakan.Game.Models;
using System;
using Sanakan.DAL.Models;
using FluentAssertions;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ShowHowMuchToLevelUpAsync(IUser?)"/> method.
    /// </summary>
    [TestClass]
    public class ShowHowMuchToLevelUpAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_Remaining_Experience()
        {
            var amount = 100ul;
            var user = new User(1ul, DateTime.UtcNow);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetByDiscordIdAsync(user.Id))
                .ReturnsAsync(user);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.ShowHowMuchToLevelUpAsync();
        }
    }
}
