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
    /// Defines tests for <see cref="PocketWaifuModule.IncCardLimitAsync(uint)"/> method.
    /// </summary>
    [TestClass]
    public class IncCardLimitAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Current_Count()
        {
            var count = 0u;
            var user = new User(1ul, DateTime.UtcNow);
            user.TcCount = 1200;

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

            await _module.IncCardLimitAsync(count);
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_Exceeded_Limit()
        {
            var count = 30u;
            var user = new User(1ul, DateTime.UtcNow);
            user.TcCount = 1200;

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

            await _module.IncCardLimitAsync(count);
        }

        [TestMethod]
        public async Task Should_Send_Error_Message_No_Coins()
        {
            var count = 10u;
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

            await _module.IncCardLimitAsync(count);
        }

        [TestMethod]
        public async Task Should_Increase_Card_Limit_And_Send_Confirm_Message()
        {
            var count = 10u;
            var user = new User(1ul, DateTime.UtcNow);
            user.TcCount = 1200;

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

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.IncCardLimitAsync(count);
        }
    }
}
