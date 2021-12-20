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
        public async Task Should_Handle_Message_Add_Success()
        {
            _payload.Message = _userMessageMock.Object;
            _payload.SourcePlayer = new PlayerInfo
            {
                DiscordId = 1ul,
                DatabaseUser = new User(1ul, DateTime.UtcNow),
                Cards = new List<Card>(),
            };
            var messageId = 1ul;
            var userId = 1ul;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
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
        public async Task Should_Handle_Message_Delete_Success()
        {
            _payload.Message = _userMessageMock.Object;
            _payload.SourcePlayer = new PlayerInfo
            {
                DiscordId = 1ul,
                DatabaseUser = new User(1ul, DateTime.UtcNow),
                Cards = new List<Card>(),
            };
            var messageId = 1ul;
            var userId = 1ul;
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            card.Id = 1ul;
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
    }
}
