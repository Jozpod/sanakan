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
    /// Defines tests for <see cref="PocketWaifuModule.IncGalleryLimitAsync(uint)"/> method.
    /// </summary>
    [TestClass]
    public class IncGalleryLimitAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Increase_Gallery_Limit_And_Send_Confirm_Message()
        {
            var count = 10u;
            var user = new User(1ul, DateTime.UtcNow);
            user.TcCount = 2000;

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

            await _module.IncGalleryLimitAsync(count);
        }
    }
}
