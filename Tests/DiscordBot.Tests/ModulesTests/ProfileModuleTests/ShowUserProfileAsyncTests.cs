using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System;
using Sanakan.DAL.Models;
using Moq;
using System.Collections.Generic;
using Sanakan.DiscordBot.Modules;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ShowUserProfileAsync(IGuildUser?)"/> method.
    /// </summary>
    [TestClass]
    public class ShowUserProfileAsyncTests : Base
    {
        protected readonly Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);

        [TestMethod]
        public async Task Should_Send_Attachment_With_User_Profile_Image()
        {
            var userId = 1ul;

            var users = new List<User>
            {
                new User(userId, DateTime.UtcNow),
            };

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            _userRepositoryMock
                .Setup(pr => pr.GetCachedAllUsersLiteAsync())
                .ReturnsAsync(users);

            await _module.ShowUserProfileAsync(_guildUserMock.Object);
        }
    }
}
