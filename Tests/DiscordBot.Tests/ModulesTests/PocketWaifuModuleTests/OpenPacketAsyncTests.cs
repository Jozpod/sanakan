using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.PocketWaifuModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="PocketWaifuModule.OpenPacketAsync(IGuildUser)"/> method.
    /// </summary>
    [TestClass]
    public class OpenPacketAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Open_Packet()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            var boosterPack = new BoosterPack
            {
                CardCount = 5,
            };
            var cards = new List<Card>
            {
                card,
                card,
                card,
                card,
                card
            };

            user.GameDeck.BoosterPacks.Add(boosterPack);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
               .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
               .ReturnsAsync(user);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _waifuServiceMock
                .Setup(pr => pr.OpenBoosterPackAsync(user.Id, It.IsAny<BoosterPack>()))
                .ReturnsAsync(cards);

            _userRepositoryMock
               .Setup(pr => pr.SaveChangesAsync(default))
               .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.OpenPacketAsync(1);
        }
    }
}
