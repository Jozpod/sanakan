using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Session.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session.Tests.CraftSessionTests
{
    /// <summary>
    /// Defines tests for <see cref="CraftSession.HandleReactionAsync"/> method.
    /// </summary>
    [TestClass]
    public class HandleReactionAsyncTests : Base
    {
        private readonly Mock<IReaction> _reactionMock = new(MockBehavior.Strict);

        [TestMethod]
        public async Task Should_Exit_When_No_PlayerInfo()
        {
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
            };

            _userMessageMock
                .SetupSequence(pr => pr.Id)
                .Returns(1ul)
                .Returns(1ul);

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Exit_When_Same_Message()
        {
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
            };

            _userMessageMock
                .SetupSequence(pr => pr.Id)
                .Returns(1ul)
                .Returns(1ul);

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Handle_Accept_Reaction()
        {
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                AddReaction = _reactionMock.Object,
            };

            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            var item = new Item { Quality = Quality.Alpha, Type = ItemType.AffectionRecoveryBig };
            user.GameDeck.Items.Add(item);

            _playerInfo.DiscordId = 1ul;
            _playerInfo.Accepted = true;
            _playerInfo.Items.Add(item);

            card.Id = 1ul;
            var characterInfo = new ShindenApi.Models.CharacterInfo();

            _userMessageMock
               .SetupSequence(pr => pr.Id)
               .Returns(1ul)
               .Returns(2ul);

            _messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns<string>(null);

            _reactionMock
               .Setup(pr => pr.Emote)
               .Returns(Emojis.Checked);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(_playerInfo.DiscordId))
                .ReturnsAsync(user);

            _waifuServiceMock
                .Setup(pr => pr.GetRandomCharacterAsync())
                .ReturnsAsync(characterInfo);

            _waifuServiceMock
                .Setup(pr => pr.GenerateNewCard(
                    _playerInfo.DiscordId,
                    characterInfo,
                    It.IsAny<Rarity>()))
                .Returns(card);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
               .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            var result = await _session.ExecuteAsync(context, _serviceProvider);
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Return_Error_No_Item()
        {
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                AddReaction = _reactionMock.Object,
            };

            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            var item = new Item { Quality = Quality.Alpha, Type = ItemType.AffectionRecoveryBig };
            user.GameDeck.Items.Add(item);

            _playerInfo.DiscordId = 1ul;
            _playerInfo.Accepted = true;
            var usedItem = new Item { Quality = Quality.Epsilon, Type = ItemType.FigureHeadPart };
            _playerInfo.Items.Add(usedItem);

            card.Id = 1ul;
            var characterInfo = new ShindenApi.Models.CharacterInfo();

            _userMessageMock
               .SetupSequence(pr => pr.Id)
               .Returns(1ul)
               .Returns(2ul);

            _messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns<string>(null);

            _reactionMock
               .Setup(pr => pr.Emote)
               .Returns(Emojis.Checked);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(_playerInfo.DiscordId))
                .ReturnsAsync(user);

            _waifuServiceMock
                .Setup(pr => pr.GetRandomCharacterAsync())
                .ReturnsAsync(characterInfo);

            _waifuServiceMock
                .Setup(pr => pr.GenerateNewCard(
                    _playerInfo.DiscordId,
                    characterInfo,
                    It.IsAny<Rarity>()))
                .Returns(card);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            var result = await _session.ExecuteAsync(context, _serviceProvider);
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Handle_Decline_Reaction()
        {
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                AddReaction = _reactionMock.Object,
            };

            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            var item = new Item { Quality = Quality.Alpha, Type = ItemType.AffectionRecoveryBig };
            user.GameDeck.Items.Add(item);

            _playerInfo.DiscordId = 1ul;
            _playerInfo.Accepted = true;
            _playerInfo.Items.Add(item);

            card.Id = 1ul;
            var characterInfo = new ShindenApi.Models.CharacterInfo();

            _userMessageMock
               .SetupSequence(pr => pr.Id)
               .Returns(1ul)
               .Returns(2ul);

            _messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns<string>(null);

            _reactionMock
               .Setup(pr => pr.Emote)
               .Returns(Emojis.CrossMark);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(_playerInfo.DiscordId))
                .ReturnsAsync(user);

            _waifuServiceMock
                .Setup(pr => pr.GetRandomCharacterAsync())
                .ReturnsAsync(characterInfo);

            _waifuServiceMock
                .Setup(pr => pr.GenerateNewCard(
                    _playerInfo.DiscordId,
                    characterInfo,
                    It.IsAny<Rarity>()))
                .Returns(card);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
               .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            var result = await _session.ExecuteAsync(context, _serviceProvider);
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Remove_Reactions()
        {
            _userMessageMock
                .Setup(pr => pr.RemoveAllReactionsAsync(null))
                .Returns(Task.CompletedTask);

            await _session.DisposeAsync();
        }
    }
}
