using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.ShindenApi.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.GenerateCardAsync(IGuildUser, ulong?, Rarity)"/> method.
    /// </summary>
    [TestClass]
    public class GenerateCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Generate_Card_And_Send_Message()
        {
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var mentionableMock = new Mock<IMentionable>(MockBehavior.Strict);

            var characterInfo = new CharacterInfo
            {
                IsReal = false,
                BirthDate = DateTime.UtcNow,
                Age = "42",
                FirstName = "john",
                LastName = "smith",
            };

            var card = new Card(
                1,
                "title",
                "name",
                100,
                50,
                Rarity.E,
                Dere.Bodere,
                DateTime.Now);

            var user = new User(1, DateTime.UtcNow);

            var discordUser = (IUser)guildUserMock.Object;

            guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("@user");

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(1);

            _waifuServiceMock
                .Setup(pr => pr.GetRandomCharacterAsync())
                .ReturnsAsync(characterInfo);

            _waifuServiceMock
                .Setup(pr => pr.GenerateNewCard(discordUser.Id, characterInfo))
                .Returns(card);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(1))
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

            await _module.GenerateCardAsync(guildUserMock.Object);
        }
    }
}
