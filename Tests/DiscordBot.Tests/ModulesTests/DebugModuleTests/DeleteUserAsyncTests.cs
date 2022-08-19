using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.DeleteUserAsync(ulong, bool)"/> method.
    /// </summary>
    [TestClass]
    public class DeleteUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_No_User()
        {
            var utcNow = DateTime.UtcNow;
            var fakeUser = new User(1ul, utcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(Sanakan.DAL.Constants.RootUserId))
                .ReturnsAsync(fakeUser);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(1))
                .ReturnsAsync(null as User);

            _userRepositoryMock
                .Setup(pr => pr.Remove(It.IsAny<User>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.DeleteUserAsync(1);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task Should_Delete_User_And_Send_Confirm_Message(bool deleteCards)
        {
            var utcNow = DateTime.UtcNow;
            var fakeUser = new User(1ul, utcNow);
            var user = new User(2ul, utcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(Sanakan.DAL.Constants.RootUserId))
                .ReturnsAsync(fakeUser);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(pr => pr.Remove(It.IsAny<User>()));

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.DeleteUserAsync(user.Id, deleteCards);
        }
    }
}
