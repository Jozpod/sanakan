using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ShowSubscriptionsAsync"/> method.
    /// </summary>
    [TestClass]
    public class ShowSubscriptionsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_Subscriptions()
        {
            var utcNow = DateTime.UtcNow;
            var databaseUser = new User(1ul, utcNow);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(databaseUser.Id);

            _guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(databaseUser.Id))
                .ReturnsAsync(databaseUser);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.ShowSubscriptionsAsync();
        }
    }
}
