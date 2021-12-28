using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Models;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ChangeStyleAsync(ProfileType, string?, SCurrency)"/> method.
    /// </summary>
    [TestClass]
    public class ChangeStyleAsyncTests : Base
    {
        [TestMethod]
        [DataRow(SCurrency.Sc)]
        [DataRow(SCurrency.Tc)]
        public async Task Should_Return_Error_Message_No_Coins(SCurrency currency)
        {
            var user = new User(1ul, DateTime.UtcNow);
            var imageUrl = "https://test.com/image.png";

            _userMock
               .Setup(pr => pr.Id)
               .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.ChangeStyleAsync(ProfileType.Cards, imageUrl, currency);
        }

        [TestMethod]
        [DataRow(ProfileType.Cards)]
        [DataRow(ProfileType.Image)]
        [DataRow(ProfileType.Statistics)]
        [DataRow(ProfileType.StatisticsWithImage)]
        public async Task Should_Change_Style_And_Return_Confirm_Message(ProfileType profileType)
        {
            var user = new User(1ul, DateTime.UtcNow);
            user.ScCount = 3000;
            user.TcCount = 1000;
            var imageUrl = "https://test.com/image.png";

            _userMock
               .Setup(pr => pr.Id)
               .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _profileServiceMock
                .Setup(pr => pr.SaveProfileImageAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(SaveResult.Success);

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

            await _module.ChangeStyleAsync(profileType, imageUrl);
        }
    }
}
