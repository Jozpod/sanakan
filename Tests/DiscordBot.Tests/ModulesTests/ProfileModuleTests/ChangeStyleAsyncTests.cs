using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.Common.Models;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;
using Sanakan.DAL.Models;
using System;
using System.Threading;
using FluentAssertions;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ChangeStyleAsync(ProfileType, string?, SCurrency)"/> method.
    /// </summary>
    [TestClass]
    public class ChangeStyleAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Change_Style()
        {
            var user = new User(1ul, DateTime.UtcNow);
            user.ScCount = 3000;
            user.TcCount = 1000;
            var profileType = ProfileType.Cards;

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
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.ChangeStyleAsync(profileType);
        }
    }
}
