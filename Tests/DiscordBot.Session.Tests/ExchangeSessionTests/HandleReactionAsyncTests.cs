using Discord;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.Game.Models;
using Sanakan.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Sanakan.DiscordBot.Session.ExchangeSession;

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
            _payload.Message = _userMessageMock.Object;
            _payload.SourcePlayer = new PlayerInfo
            {
                DiscordId = 1ul,
                DatabaseUser = new User(1ul, DateTime.UtcNow),
                Cards = new List<Card>(),
            };
            var messageId = 1ul;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            var reaction = ReactionExtensions.CreateReaction(_payload.SourcePlayer.DiscordId, Emojis.Checked);
            _payload.State = ExchangeStatus.AcceptSourcePlayer;
            _payload.SourcePlayer.DatabaseUser.GameDeck.Cards.Add(card);
            _payload.DestinationPlayer = new PlayerInfo
            {
                DiscordId = 2ul,
                DatabaseUser = new User(2ul, DateTime.UtcNow),
            };
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
                UserId = _payload.SourcePlayer.DiscordId,
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
            _payload.Tips.Should().NotBeNullOrEmpty();
            _payload.State.Should().Be(ExchangeStatus.AcceptDestinationPlayer);
        }

        [TestMethod]
        public async Task Should_Handle_Reaction_Accept_Destination()
        {
            var card1 = new Card(1ul, "title 1", "name 1", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card1.Id = 1ul;
            var card2 = new Card(2ul, "title 2", "name 2", 100, 50, Rarity.B, Dere.Deredere, DateTime.UtcNow);
            card2.Id = 2ul;
            var messageId = 1ul;

            _payload.Message = _userMessageMock.Object;
            _payload.SourcePlayer = new PlayerInfo
            {
                DiscordId = 1ul,
                DatabaseUser = new User(1ul, DateTime.UtcNow),
                Cards = new List<Card>()
                {
                    card1,
                }
            };
            _payload.SourcePlayer.DatabaseUser.GameDeck.Cards.Add(card1);

            _payload.DestinationPlayer = new PlayerInfo
            {
                DiscordId = 2ul,
                DatabaseUser = new User(2ul, DateTime.UtcNow),
                Cards = new List<Card>
                {
                    card2,
                },
            };
            _payload.DestinationPlayer.DatabaseUser.GameDeck.Cards.Add(card2);
            var reaction = ReactionExtensions.CreateReaction(_payload.DestinationPlayer.DiscordId, Emojis.Checked);
            _payload.State = ExchangeStatus.AcceptDestinationPlayer;

            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
                UserId = _payload.SourcePlayer.DiscordId,
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
                .Setup(pr => pr.GetUserOrCreateAsync(_payload.SourcePlayer.DiscordId))
                .ReturnsAsync(_payload.SourcePlayer.DatabaseUser);
                
            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(_payload.DestinationPlayer.DiscordId))
                .ReturnsAsync(_payload.DestinationPlayer.DatabaseUser);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            await _session.ExecuteAsync(context, _serviceProvider);
            _payload.Tips.Should().NotBeNullOrEmpty();
            _payload.State.Should().Be(ExchangeStatus.End);
        }

        [TestMethod]
        public async Task Should_Handle_Reaction_Accept_Add_Source()
        {
            _payload.Message = _userMessageMock.Object;
            _payload.SourcePlayer = new PlayerInfo
            {
                DiscordId = 1ul,
                DatabaseUser = new User(1ul, DateTime.UtcNow),
                Cards = new List<Card>(),
            };
            var messageId = 1ul;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            var reaction = ReactionExtensions.CreateReaction(_payload.SourcePlayer.DiscordId, Emojis.OneEmote);
            _payload.State = ExchangeStatus.Add;
            _payload.SourcePlayer.DatabaseUser.GameDeck.Cards.Add(card);
            _payload.DestinationPlayer = new PlayerInfo
            {
                DiscordId = 2ul,
                DatabaseUser = new User(2ul, DateTime.UtcNow),
            };
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
                UserId = _payload.SourcePlayer.DiscordId,
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
            _payload.SourcePlayer.Accepted.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Handle_Reaction_Accept_Add_Destination()
        {
            _payload.Message = _userMessageMock.Object;
            _payload.SourcePlayer = new PlayerInfo
            {
                DiscordId = 1ul,
                DatabaseUser = new User(1ul, DateTime.UtcNow),
                Cards = new List<Card>(),
            };
            var messageId = 1ul;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            _payload.State = ExchangeStatus.Add;
            _payload.SourcePlayer.DatabaseUser.GameDeck.Cards.Add(card);
            _payload.DestinationPlayer = new PlayerInfo
            {
                DiscordId = 2ul,
                DatabaseUser = new User(2ul, DateTime.UtcNow),
            };
            var reaction = ReactionExtensions.CreateReaction(_payload.DestinationPlayer.DiscordId, Emojis.TwoEmote);
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
                UserId = _payload.SourcePlayer.DiscordId,
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
            _payload.DestinationPlayer.Accepted.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Handle_Reaction_Reject()
        {
            _payload.Message = _userMessageMock.Object;
            _payload.SourcePlayer = new PlayerInfo
            {
                DiscordId = 1ul,
                DatabaseUser = new User(1ul, DateTime.UtcNow),
                Cards = new List<Card>(),
            };
            var messageId = 1ul;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            var reaction = ReactionExtensions.CreateReaction(_payload.SourcePlayer.DiscordId, Emojis.CrossMark);
            _payload.State = ExchangeStatus.AcceptSourcePlayer;
            _payload.SourcePlayer.DatabaseUser.GameDeck.Cards.Add(card);
            _payload.DestinationPlayer = new PlayerInfo
            {
                DiscordId = 2ul,
                DatabaseUser = new User(2ul, DateTime.UtcNow),
            };
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
                UserId = _payload.SourcePlayer.DiscordId,
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
    }
}
