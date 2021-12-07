using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DAL.Models;
using Moq;
using System.Collections.Generic;
using Sanakan.DiscordBot.Modules;
using Discord;
using System;
using System.Threading;
using FluentAssertions;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ToggleWaifuViewInProfileAsync"/> method.
    /// </summary>
    [TestClass]
    public class ToggleWaifuViewInProfileAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Set_Waifu_In_Profile()
        {
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

            await _module.ToggleWaifuViewInProfileAsync();
        }
    }
}
