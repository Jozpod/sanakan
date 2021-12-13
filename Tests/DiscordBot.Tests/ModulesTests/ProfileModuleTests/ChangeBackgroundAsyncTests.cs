using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System;
using Sanakan.DAL.Models;
using Moq;
using Sanakan.DiscordBot.Services;
using System.Threading;
using Sanakan.DiscordBot.Modules;
using FluentAssertions;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ChangeBackgroundAsync(string, SCurrency)"/> method.
    /// </summary>
    [TestClass]
    public class ChangeBackgroundAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Change_Background_And_Send_Confirm_Message()
        {
            var imageUrl = "image.png";
            var user = new User(1ul, DateTime.UtcNow)
            {
                ScCount = 5000,
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
                .Setup(pr => pr.ExpireTag(It.IsAny<string>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ChangeBackgroundAsync(imageUrl);
        }
    }
}
