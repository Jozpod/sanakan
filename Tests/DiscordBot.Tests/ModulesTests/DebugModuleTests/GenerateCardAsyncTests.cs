using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Diagnostics;
using System;
using Moq;
using Sanakan.ShindenApi.Models;
using Sanakan.DAL.Models;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    [TestClass]
    public class GenerateCardAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Return_Random_Card()
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
                .Setup(pr => pr.GenerateNewCard(discordUser, characterInfo))
                .Returns(card);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(1))
                .ReturnsAsync(user);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            await _module.GenerateCardAsync(guildUserMock.Object);
        }
    }
}
