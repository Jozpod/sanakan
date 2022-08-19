using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Session;
using Sanakan.Game.Models;
using Sanakan.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ShowTopAsync(TopType)"/> method.
    /// </summary>
    [TestClass]
    public class ShowTopAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_And_Start_Session()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var users = new[] { user };
            var strList = new List<string>();

            _userMock
               .Setup(pr => pr.Id)
               .Returns(user.Id);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _sessionManagerMock
                .Setup(pr => pr.RemoveIfExists<ListSession>(user.Id));

            _userRepositoryMock
                .Setup(pr => pr.GetAllCachedAsync())
                .ReturnsAsync(users);

            _profileServiceMock
               .Setup(pr => pr.GetTopUsers(
                   users,
                   It.IsAny<TopType>(),
                   It.IsAny<DateTime>()))
               .Returns(users);

            _profileServiceMock
                .Setup(pr => pr.BuildListViewAsync(
                    users,
                    It.IsAny<TopType>(),
                    It.IsAny<IGuild>()))
                .ReturnsAsync(strList);

            _messageChannelMock.SetupSendMessageAsync(userMessageMock.Object);

            userMessageMock
                .Setup(pr => pr.DeleteAsync(null))
                .Returns(Task.CompletedTask);

            userMessageMock
                .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                .Returns(Task.CompletedTask);

            _sessionManagerMock
                .Setup(pr => pr.Add(It.IsAny<ListSession>()));

            await _module.ShowTopAsync();
        }
    }
}
