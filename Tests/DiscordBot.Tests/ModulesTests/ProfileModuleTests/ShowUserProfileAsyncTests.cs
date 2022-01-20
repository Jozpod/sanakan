using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.Tests.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ShowUserProfileAsync(IGuildUser?)"/> method.
    /// </summary>
    [TestClass]
    public class ShowUserProfileAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_No_User()
        {
            _commandContextMock
                .Setup(pr => pr.User)
                .Returns<IUser>(null);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.ShowUserProfileAsync(null);
        }

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

            _messageChannelMock.SetupSendFileAsync(null);

            await _module.ShowUserProfileAsync(_guildUserMock.Object);
        }
    }
}
