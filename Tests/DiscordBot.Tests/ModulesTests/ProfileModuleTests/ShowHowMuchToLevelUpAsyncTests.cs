using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ShowHowMuchToLevelUpAsync(IUser?)"/> method.
    /// </summary>
    [TestClass]
    public class ShowHowMuchToLevelUpAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_No_User()
        {
            _commandContextMock
               .Setup(pr => pr.User)
               .Returns<IUser>(null);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.ShowHowMuchToLevelUpAsync();
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_No_Database_User()
        {
            var userId = 1ul;

            _userMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetByDiscordIdAsync(userId))
                .ReturnsAsync(null as User);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.ShowHowMuchToLevelUpAsync();
        }

        [TestMethod]
        public async Task Should_Send_Message_Containing_Remaining_Experience()
        {
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
