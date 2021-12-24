using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Sanakan.DiscordBot.Session.ExchangeSession;

namespace Sanakan.DiscordBot.Session.ExchangeSessionTests
{
    /// <summary>
    /// Defines tests for <see cref="ExchangeSession"/> class.
    /// </summary>
    [TestClass]
    public class HandleMessageAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Quit_Wrong_Channel()
        {
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
                UserId = _sourcePlayer.DiscordId,
            };

            _userMessageMock
                .SetupSequence(pr => pr.Id)
                .Returns(1ul)
                .Returns(2ul);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _messageChannelMock
                .SetupSequence(pr => pr.Id)
                .Returns(1ul)
                .Returns(2ul);

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Quit_No_Reactions()
        {
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
                UserId = _sourcePlayer.DiscordId,
            };

            _userMessageMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Handle_Message_Add_Success()
        {
            _sourcePlayer.DiscordId = 1ul;
            _sourcePlayer.DatabaseUser = new User(1ul, DateTime.UtcNow);
            var messageId = 1ul;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            _sourcePlayer.DatabaseUser.GameDeck.Cards.Add(card);
            _destinationPlayer.DiscordId = 2ul;
            _destinationPlayer.DatabaseUser = new User(2ul, DateTime.UtcNow);
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
                UserId = _sourcePlayer.DiscordId,
            };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .SetupSequence(pr => pr.Id)
                .Returns(messageId)
                .Returns(2ul)
                .Returns(messageId)
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

        public (SessionContext, Card, User) Setup()
        {
            var sourceUser = new User(1ul, DateTime.UtcNow);
            var targetUser = new User(2ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            sourceUser.GameDeck.Cards.Add(card);

            _sourcePlayer.DiscordId = 1ul;
            _sourcePlayer.DatabaseUser = sourceUser;
            _sourcePlayer.Cards = new List<Card>();
            var messageId = 1ul;
            _destinationPlayer.DiscordId = 2ul;
            _destinationPlayer.DatabaseUser = targetUser;
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
                UserId = _sourcePlayer.DiscordId,
            };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .SetupSequence(pr => pr.Id)
                .Returns(messageId)
                .Returns(2ul)
                .Returns(messageId)
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

            return (context, card, targetUser);
        }

        [TestMethod]
        public async Task Should_Handle_Message_Error_Invalid_Expedition()
        {
            var (context, card, targetUser) = Setup();
            card.Expedition = ExpeditionCardType.DarkExp;

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Handle_Message_Error_Invalid_Dere()
        {
            var (context, card, targetUser) = Setup();
            card.Dere = Dere.Yato;

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Handle_Message_Error_Invalid_Card()
        {
            var (context, card, targetUser) = Setup();
            card.IsTradable = false;

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Handle_Message_Error_Invalid_Card_Figure()
        {
            var (context, card, targetUser) = Setup();
            card.FromFigure = true;
            card.PAS = PreAssembledFigure.Asuna;

            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Handle_Message_Delete_No_Cards()
        {
            _sourcePlayer.DiscordId = 1ul;
            _sourcePlayer.DatabaseUser = new User(1ul, DateTime.UtcNow);
            _sourcePlayer.Cards = new List<Card>();
            var messageId = 1ul;
            var userId = 1ul;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            _sourcePlayer.DatabaseUser.GameDeck.Cards.Add(card);
            _destinationPlayer.DiscordId = 2ul;
            _destinationPlayer.DatabaseUser = new User(2ul, DateTime.UtcNow);
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
                UserId = userId,
            };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .SetupSequence(pr => pr.Id)
                .Returns(messageId)
                .Returns(2ul)
                .Returns(messageId)
                .Returns(messageId);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns($"usun {card.Id}");

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

        public SessionContext SetupDelete()
        {
            _sourcePlayer.DiscordId = 1ul;
            _sourcePlayer.DatabaseUser = new User(1ul, DateTime.UtcNow);
            _sourcePlayer.Cards = new List<Card>();
            var messageId = 1ul;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
            _sourcePlayer.DatabaseUser.GameDeck.Cards.Add(card);
            _sourcePlayer.Cards.Add(card);
            _destinationPlayer.DiscordId = 2ul;
            _destinationPlayer.DatabaseUser = new User(2ul, DateTime.UtcNow);
            var context = new SessionContext
            {
                Message = _userMessageMock.Object,
                Channel = _messageChannelMock.Object,
            };

            _userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .SetupSequence(pr => pr.Id)
                .Returns(messageId)
                .Returns(2ul)
                .Returns(messageId)
                .Returns(messageId);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns($"usun {card.Id}");

            _userMessageMock
                 .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                 .Returns(Task.CompletedTask);

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_userMessageMock.Object);

            _userMessageMock
               .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
               .Returns(Task.CompletedTask);

            return context;
        }

        [TestMethod]
        public async Task Should_Handle_Message_Delete_Success_Source()
        {
            var context = SetupDelete();
            context.UserId = _sourcePlayer.DiscordId;
            await _session.ExecuteAsync(context, _serviceProvider);
        }

        [TestMethod]
        public async Task Should_Handle_Message_Delete_Success_Destination()
        {
            var context = SetupDelete();
            context.UserId = _destinationPlayer.DiscordId;
            await _session.ExecuteAsync(context, _serviceProvider);
        }
    }
}
