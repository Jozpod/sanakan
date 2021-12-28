using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.ChangeWaifuSiteForegroundColorAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class ChangeWaifuSiteForegroundColorAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_No_Coins()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);

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

            await _module.ChangeWaifuSiteForegroundColorAsync("#FAFAFA");
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_Invalid_Color()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.TcCount = 500;

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

            await _module.ChangeWaifuSiteForegroundColorAsync("test");
        }

        [TestMethod]
        public async Task Should_Set_Color_And_Send_Confirm_Message()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.TcCount = 500;

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _userRepositoryMock
               .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ChangeWaifuSiteForegroundColorAsync("#FAFAFA");
        }
    }
}
