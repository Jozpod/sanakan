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
    /// Defines tests for <see cref="PocketWaifuModule.OpenPacketAsync(int, int, bool)"/> method.
    /// </summary>
    [TestClass]
    public class OpenPacketAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Error_Message_No_Booster_Packs()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.GameDeck.BoosterPacks.Clear();
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);

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

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.OpenPacketAsync(1);
        }

        [TestMethod]
        public async Task Should_Send_Message_Containing_List()
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

            _waifuServiceMock
                .Setup(pr => pr.GetBoosterPackList(_userMock.Object, It.IsAny<List<BoosterPack>>()))
                .Returns(new EmbedBuilder().Build());

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.OpenPacketAsync(0);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_Index_Out_Of_Bounds()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            var boosterPack = new BoosterPack
            {
                CardCount = 5,
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

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.OpenPacketAsync(3);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_Invalid_Count()
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

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.OpenPacketAsync(1, 10);
        }

        [TestMethod]
        public async Task Should_Return_Error_Message_Too_Many_Cards()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            var boosterPack = new BoosterPack
            {
                CardCount = 21,
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

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.OpenPacketAsync(1, 2);
        }

        public async Task Should_Return_Error_Message_No_Space()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.GameDeck.MaxNumberOfCards = 1;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, utcNow);
            var boosterPack = new BoosterPack
            {
                CardCount = 2,
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

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.OpenPacketAsync(1);
        }

        [TestMethod]
        public async Task Should_Open_Packet_And_Return_Confirm_Message()
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
