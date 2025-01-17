using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ChangeBackgroundAsync(string, SCurrency)"/> method.
    /// </summary>
    [TestClass]
    public class ChangeBackgroundAsyncTests : Base
    {
        [TestMethod]
        [DataRow(SCurrency.Sc)]
        [DataRow(SCurrency.Tc)]
        public async Task Should_Send_Error_Message_No_Coins(SCurrency currency)
        {
            var imageUrl = new Uri("https://test.com/image.png");
            var user = new User(1ul, DateTime.UtcNow);

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
                embed.Description.Should().NotBeNull();
            });

            await _module.ChangeBackgroundAsync(imageUrl, currency);
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_Bad_Url()
        {
            var imageUrl = new Uri("https://test.com/image.png");
            var user = new User(1ul, DateTime.UtcNow)
            {
                ScCount = 5000,
                TcCount = 5000,
            };

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
                .ReturnsAsync(SaveResult.BadUrl);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ChangeBackgroundAsync(imageUrl);
        }

        [TestMethod]
        [DataRow(SCurrency.Sc)]
        [DataRow(SCurrency.Tc)]
        public async Task Should_Change_Background_And_Send_Confirm_Message(SCurrency currency)
        {
            var imageUrl = new Uri("https://test.com/image.png");
            var user = new User(1ul, DateTime.UtcNow)
            {
                ScCount = 5000,
                TcCount = 5000,
            };

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
                embed.Description.Should().NotBeNull();
            });

            await _module.ChangeBackgroundAsync(imageUrl, currency);
        }
    }
}
