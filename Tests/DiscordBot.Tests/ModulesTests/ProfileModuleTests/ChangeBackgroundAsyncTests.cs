using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Diagnostics;
using System;
using Sanakan.DAL.Models;
using Moq;
using DiscordBot.Services;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    [TestClass]
    public class ChangeBackgroundAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Give_Bot_Info()
        {
            var imageUrl = "image.png";
            var user = new User(1ul, DateTime.UtcNow)
            {
                ScCount = 5000,
            };

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
                .Setup(pr => pr.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string>()));

            await _module.ChangeBackgroundAsync(imageUrl);
        }
    }
}
