using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Session;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.ExchangeCardsAsync(IGuildUser)"/> method.
    /// </summary>
    [TestClass]
    public class ExchangeCardsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_Session_Started_Already()
        {
            var utcNow = DateTime.UtcNow;
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var sourceUser = new User(1ul, utcNow);
            var destinationUser = new User(2ul, utcNow);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(sourceUser.Id);

            _guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("source mention");

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(destinationUser.Id);

            guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("target mention");

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(sourceUser.Id))
                .ReturnsAsync(sourceUser);

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(destinationUser.Id))
                .ReturnsAsync(destinationUser);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _sessionManagerMock
                .Setup(pr => pr.Exists<ExchangeSession>(sourceUser.Id))
                .Returns(true);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ExchangeCardsAsync(guildUserMock.Object);
        }

        [TestMethod]
        public async Task Should_Start_Session_And_Send_Confirm_Message()
        {
            var utcNow = DateTime.UtcNow;
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var sourceUser = new User(1ul, utcNow);
            var destinationUser = new User(2ul, utcNow);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(sourceUser.Id);

            _guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("source mention");

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(destinationUser.Id);

            guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("target mention");

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(sourceUser.Id))
                .ReturnsAsync(sourceUser);

            _userRepositoryMock
                .Setup(pr => pr.GetCachedFullUserAsync(destinationUser.Id))
                .ReturnsAsync(destinationUser);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _userMessageMock
                .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                .Returns(Task.CompletedTask);

            _sessionManagerMock
                .Setup(pr => pr.Exists<ExchangeSession>(sourceUser.Id))
                .Returns(false);

            _sessionManagerMock
                .Setup(pr => pr.Add(It.IsAny<ExchangeSession>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ExchangeCardsAsync(guildUserMock.Object);
        }
    }
}
