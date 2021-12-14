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
        public async Task Should_Add_To_Wish_List()
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
                .Setup(pr => pr.GetUserOrCreateAsync(sourceUser.Id))
                .ReturnsAsync(sourceUser);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(destinationUser.Id))
                .ReturnsAsync(destinationUser);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _messageChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(userMessageMock.Object);

            userMessageMock
                .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                .Returns(Task.CompletedTask);

            _sessionManagerMock
                .Setup(pr => pr.Exists<ExchangeSession>(sourceUser.Id));

            _sessionManagerMock
                .Setup(pr => pr.Add(It.IsAny<IInteractionSession>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.ExchangeCardsAsync(_guildUserMock.Object);
        }
    }
}
