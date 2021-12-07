using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System;
using Sanakan.DAL.Models;
using Moq;
using System.Collections.Generic;
using Sanakan.DiscordBot.Modules;
using System.IO;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ShowUserProfileAsync(IGuildUser?)"/> method.
    /// </summary>
    [TestClass]
    public class ShowUserProfileAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Attachment_With_User_Profile_Image()
        {
            var user = new User(1ul, DateTime.UtcNow);
            var user1 = new User(2ul, DateTime.UtcNow);
            user1.ExperienceCount = 400;
            var user2 = new User(3ul, DateTime.UtcNow);
            user2.ExperienceCount = 500;
            var users = new List<User>
            {
                user,
                user1,
                user2,
            };
            var memoryStream = new MemoryStream();

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userRepositoryMock
                .Setup(pr => pr.GetCachedAllUsersLiteAsync())
                .ReturnsAsync(users);

            _gameDeckRepositoryMock
                .Setup(pr => pr.GetCachedUserGameDeckAsync(user.Id))
                .ReturnsAsync(user.GameDeck);

            _profileServiceMock
                .Setup(pr => pr.GetProfileImageAsync(_guildUserMock.Object, user, users.Count))
                .ReturnsAsync(memoryStream);

            _messageChannelMock
                .Setup(pr => pr.SendFileAsync(
                    memoryStream,
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<bool>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(null as IUserMessage);

            await _module.ShowUserProfileAsync(_guildUserMock.Object);
        }
    }
}
