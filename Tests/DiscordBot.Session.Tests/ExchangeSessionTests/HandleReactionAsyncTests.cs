using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Session.Abstractions;
using Sanakan.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session.ExchangeSessionTests
{
    /// <summary>
    /// Defines tests for <see cref="ExchangeSession"/> class.
    /// </summary>
    [TestClass]
    public class HandleReactionAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Handle_Reaction_Accept_Source()
        {
            _sourcePlayer.DiscordId = 1ul;
            _sourcePlayer.DatabaseUser = new User(1ul, DateTime.UtcNow);
            _sourcePlayer.Cards = new List<Card>();
            var messageId = 1ul;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            var reaction = ReactionExtensions.CreateReaction(_sourcePlayer.DiscordId, Emojis.Checked);
            _sourcePlayer.DatabaseUser.GameDeck.Cards.Add(card);
            _destinationPlayer.DiscordId = 2ul;
            _destinationPlayer.DatabaseUser = new User(2ul, DateTime.UtcNow);
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
                UserId = _sourcePlayer.DiscordId,
                AddReaction = reaction,
            };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Id)
                .Returns(messageId);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns($"dodaj {card.Id}");

            _userMessageMock
                 .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                 .Returns(Task.CompletedTask);

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_userMessageMock.Object);

            _userMessageMock
               .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
               .Returns(Task.CompletedTask);

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Handle_Reaction_Accept_Destination()
        {
            var card1 = new Card(1ul, "title 1", "name 1", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card1.Id = 1ul;
            var card2 = new Card(2ul, "title 2", "name 2", 100, 50, Rarity.B, Dere.Deredere, DateTime.UtcNow);
            card2.Id = 2ul;
            var messageId = 1ul;

            _sourcePlayer.Cards.Add(card1);
            _sourcePlayer.DiscordId = 1ul;
            _sourcePlayer.DatabaseUser = new User(1ul, DateTime.UtcNow);
            _sourcePlayer.DatabaseUser.GameDeck.Cards.Add(card1);
            _destinationPlayer.DiscordId = 2ul;
            _destinationPlayer.DatabaseUser = new User(2ul, DateTime.UtcNow);
            _destinationPlayer.Cards.Add(card2);
            _destinationPlayer.DatabaseUser.GameDeck.Cards.Add(card2);
            var reaction = ReactionExtensions.CreateReaction(_destinationPlayer.DiscordId, Emojis.Checked);

            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
                UserId = _sourcePlayer.DiscordId,
                AddReaction = reaction,
            };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                 .Setup(pr => pr.Id)
                 .Returns(messageId);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns($"dodaj {card1.Id}");

            _userMessageMock
                 .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                 .Returns(Task.CompletedTask);

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_userMessageMock.Object);

            _userMessageMock
               .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
               .Returns(Task.CompletedTask);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(_sourcePlayer.DiscordId))
                .ReturnsAsync(_sourcePlayer.DatabaseUser);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(_destinationPlayer.DiscordId))
                .ReturnsAsync(_destinationPlayer.DatabaseUser);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Handle_Reaction_Accept_Add_Source()
        {
            _sourcePlayer.DiscordId = 1ul;
            _sourcePlayer.DatabaseUser = new User(1ul, DateTime.UtcNow);
            var messageId = 1ul;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            var reaction = ReactionExtensions.CreateReaction(_sourcePlayer.DiscordId, Emojis.OneEmote);
            _sourcePlayer.DatabaseUser.GameDeck.Cards.Add(card);
            _destinationPlayer.DiscordId = 2ul;
            _destinationPlayer.DatabaseUser = new User(2ul, DateTime.UtcNow);
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
                UserId = _sourcePlayer.DiscordId,
                AddReaction = reaction,
            };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Id)
                .Returns(messageId);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns($"dodaj {card.Id}");

            _userMessageMock
                 .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                 .Returns(Task.CompletedTask);

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_userMessageMock.Object);

            _userMessageMock
               .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
               .Returns(Task.CompletedTask);

            await _session.ExecuteAsync(context, _serviceProvider);
            _sourcePlayer.Accepted.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Handle_Reaction_Accept_Add_Destination()
        {
            _sourcePlayer.DiscordId = 1ul;
            _sourcePlayer.DatabaseUser = new User(1ul, DateTime.UtcNow);
            var messageId = 1ul;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            _sourcePlayer.DatabaseUser.GameDeck.Cards.Add(card);
            _destinationPlayer.DiscordId = 2ul;
            _destinationPlayer.DatabaseUser = new User(2ul, DateTime.UtcNow);
            var reaction = ReactionExtensions.CreateReaction(_destinationPlayer.DiscordId, Emojis.TwoEmote);
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
                UserId = _sourcePlayer.DiscordId,
                AddReaction = reaction,
            };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Id)
                .Returns(messageId);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns($"dodaj {card.Id}");

            _userMessageMock
                 .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                 .Returns(Task.CompletedTask);

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_userMessageMock.Object);

            _userMessageMock
               .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
               .Returns(Task.CompletedTask);

            await _session.ExecuteAsync(context, _serviceProvider);
            _destinationPlayer.Accepted.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Handle_Reaction_Accept_Add_Both()
        {
            _sourcePlayer.DiscordId = 1ul;
            _sourcePlayer.DatabaseUser = new User(1ul, DateTime.UtcNow);
            var messageId = 1ul;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            _sourcePlayer.DatabaseUser.GameDeck.Cards.Add(card);
            _sourcePlayer.Accepted = true;
            _destinationPlayer.DiscordId = 2ul;
            _destinationPlayer.DatabaseUser = new User(2ul, DateTime.UtcNow);
            var reaction = ReactionExtensions.CreateReaction(_destinationPlayer.DiscordId, Emojis.TwoEmote);
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
                UserId = _sourcePlayer.DiscordId,
                AddReaction = reaction,
            };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Id)
                .Returns(messageId);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns($"dodaj {card.Id}");

            _userMessageMock
                 .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                 .Returns(Task.CompletedTask);

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_userMessageMock.Object);

            _userMessageMock
               .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
               .Returns(Task.CompletedTask);

            await _session.ExecuteAsync(context, _serviceProvider);
            _destinationPlayer.Accepted.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Run()
        {
            _sourcePlayer.DiscordId = 1ul;
            _sourcePlayer.DatabaseUser = new User(1ul, DateTime.UtcNow);
            var messageId = 1ul;
            var card1 = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card1.Id = 1ul;
            _sourcePlayer.DatabaseUser.GameDeck.Cards.Add(card1);
            _sourcePlayer.Accepted = true;
            _sourcePlayer.Cards.Add(card1);

            _destinationPlayer.DiscordId = 2ul;
            _destinationPlayer.DatabaseUser = new User(2ul, DateTime.UtcNow);
            var card2 = new Card(2ul, "title", "name", 200, 100, Rarity.B, Dere.Deredere, DateTime.UtcNow);
            card2.Id = 2ul;
            _destinationPlayer.DatabaseUser.GameDeck.Cards.Add(card2);
            _destinationPlayer.Cards.Add(card2);

            var destinationPlayerReaction = ReactionExtensions.CreateReaction(_destinationPlayer.DiscordId, Emojis.TwoEmote);
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
                UserId = _sourcePlayer.DiscordId,
                AddReaction = destinationPlayerReaction,
            };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Id)
                .Returns(messageId);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                 .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                 .Returns(Task.CompletedTask);

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_userMessageMock.Object);

            _userMessageMock
               .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
               .Returns(Task.CompletedTask);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(_sourcePlayer.DiscordId))
                .ReturnsAsync(_sourcePlayer.DatabaseUser);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(_destinationPlayer.DiscordId))
                .ReturnsAsync(_destinationPlayer.DatabaseUser);

            _userRepositoryMock
               .Setup(pr => pr.SaveChangesAsync(default))
               .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            await _session.ExecuteAsync(context, _serviceProvider);
            var sourcePlayerReaction = ReactionExtensions.CreateReaction(_sourcePlayer.DiscordId, Emojis.Checked);
            context.AddReaction = sourcePlayerReaction;
            await _session.ExecuteAsync(context, _serviceProvider);
            destinationPlayerReaction = ReactionExtensions.CreateReaction(_destinationPlayer.DiscordId, Emojis.Checked);
            context.AddReaction = destinationPlayerReaction;
            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Handle_Reaction_Reject()
        {
            _sourcePlayer.DiscordId = 1ul;
            _sourcePlayer.DatabaseUser = new User(1ul, DateTime.UtcNow);

            var messageId = 1ul;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            var reaction = ReactionExtensions.CreateReaction(_sourcePlayer.DiscordId, Emojis.CrossMark);
            _sourcePlayer.DatabaseUser.GameDeck.Cards.Add(card);
            _destinationPlayer.DiscordId = 2ul;
            _destinationPlayer.DatabaseUser = new User(2ul, DateTime.UtcNow);
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
                UserId = _sourcePlayer.DiscordId,
                AddReaction = reaction,
            };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Id)
                .Returns(messageId);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns($"dodaj {card.Id}");

            _userMessageMock
                 .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                 .Returns(Task.CompletedTask);

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_userMessageMock.Object);

            _userMessageMock
               .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
               .Returns(Task.CompletedTask);

            await _session.ExecuteAsync(context, _serviceProvider);
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
