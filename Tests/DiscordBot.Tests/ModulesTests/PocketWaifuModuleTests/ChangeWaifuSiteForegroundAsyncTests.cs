using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.ChangeWaifuSiteForegroundAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class ChangeWaifuSiteForegroundAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_No_Coins()
        {
            var imageUrl = "https://test.com/image.jpg";
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _guildUserMock
               .Setup(pr => pr.Id)
               .Returns(user.Id);

            _guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ChangeWaifuSiteForegroundAsync(imageUrl);
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_Invalid_Url()
        {
            var imageUrl = "test";
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.TcCount = 2000;

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _guildUserMock
               .Setup(pr => pr.Id)
               .Returns(user.Id);

            _guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ChangeWaifuSiteForegroundAsync(imageUrl);
        }

        [TestMethod]
        public async Task Should_Change_Waifu_Site_Fackground_And_Send_Confirm_Message()
        {
            var imageUrl = "https://test.com/image.jpg";
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.TcCount = 2000;

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _guildUserMock
               .Setup(pr => pr.Id)
               .Returns(user.Id);

            _guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ChangeWaifuSiteForegroundAsync(imageUrl);
        }
    }
}
