using Discord;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Session.Abstractions;
using Sanakan.Extensions;
using Sanakan.Game.Extensions;
using Sanakan.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session
{
    public class ExchangeSession : InteractionSession
    {
        private readonly IUserMessage _userMessage;
        private readonly PlayerInfo _sourcePlayer;
        private readonly PlayerInfo _destinationPlayer;
        private ICacheManager _cacheManager = null;
        private IIconConfiguration _iconConfiguration = null;
        private IServiceProvider _serviceProvider = null;
        private string _name;
        private string _tips;
        private ExchangeStatus _state;

        public ExchangeSession(
            ulong ownerId,
            ulong participantId,
            DateTime createdOn,
            IUserMessage userMessage,
            PlayerInfo sourcePlayer,
            PlayerInfo destinationPlayer,
            string name,
            string tips)
        : base(
          new[] { ownerId, participantId },
          createdOn,
          Durations.TwoMinutes,
          Discord.Commands.RunMode.Sync,
          SessionExecuteCondition.AllEvents)
        {
            _userMessage = userMessage;
            _sourcePlayer = sourcePlayer;
            _destinationPlayer = destinationPlayer;
            _name = name;
            _tips = tips;
            _state = ExchangeStatus.Add;
        }

        public override async Task<bool> ExecuteAsync(
           SessionContext sessionContext,
           IServiceProvider serviceProvider,
           CancellationToken cancellationToken = default)
        {
            try
            {
                IsRunning = true;
                _serviceProvider = serviceProvider;

                _iconConfiguration = _serviceProvider.GetRequiredService<IIconConfiguration>();

                var messageId = _userMessage.Id;
                var reaction = sessionContext.AddReaction ?? sessionContext.RemoveReaction;

                if (_state == ExchangeStatus.End)
                {
                    return true;
                }

                if (sessionContext.Message.Id == messageId)
                {
                    if (reaction == null)
                    {
                        return false;
                    }

                    await HandleReactionAsync(reaction);
                }
                else
                {
                    if (reaction != null)
                    {
                        return false;
                    }

                    await HandleMessageAsync(sessionContext);
                }

                IsRunning = false;
            }
            finally
            {
                IsRunning = true;
            }

            return false;
        }

        public Embed BuildEmbed()
        {
            var description = $"{_name}\n\n{_sourcePlayer.CustomString}\n\n{_destinationPlayer.CustomString}\n\n{_tips}".ElipseTrimToLength(2000);
            return new EmbedBuilder
            {
                Color = EMType.Warning.Color(),
                Description = description,
            }.Build();
        }

        public double GetAverageValue(IEnumerable<Card> cards)
        {
            if (!cards.Any())
            {
                return 0.01;
            }

            return cards.Average(x => x.MarketValue);
        }

        public double GetAverageRarity(IEnumerable<Card> cards)
        {
            if (!cards.Any())
            {
                return (int)Rarity.E;
            }

            return cards.Average(x => (int)x.Rarity);
        }

        public override async ValueTask DisposeAsync()
        {
            try
            {
                await _userMessage.RemoveAllReactionsAsync();
            }
            catch (Exception)
            {
            }
        }

        private async Task HandleMessageAsync(SessionContext context)
        {
            if (_state != ExchangeStatus.Add)
            {
                return;
            }

            if (context.Channel.Id != _userMessage.Channel.Id)
            {
                return;
            }

            var command = context.Message?.Content?.ToLower();
            if (command == null)
            {
                return;
            }

            var splitedCommand = command.Replace("\n", " ").Split(" ");
            if (splitedCommand.Length < 2)
            {
                return;
            }

            var commandType = splitedCommand[0];
            if (commandType == null)
            {
                return;
            }

            PlayerInfo? thisPlayer = null;
            PlayerInfo? targetPlayer = null;

            var userMessage = context.Message;
            var userId = context.UserId;

            if (userId == _sourcePlayer.DiscordId)
            {
                thisPlayer = _sourcePlayer;
                targetPlayer = _destinationPlayer;
            }

            if (userId == _destinationPlayer.DiscordId)
            {
                thisPlayer = _destinationPlayer;
                targetPlayer = _sourcePlayer;
            }

            if (thisPlayer == null)
            {
                return;
            }

            if (commandType.Contains("usuń") || commandType.Contains("usun"))
            {
                var cardIdStr = splitedCommand?[1];
                if (string.IsNullOrEmpty(cardIdStr))
                {
                    await userMessage.AddReactionAsync(_iconConfiguration.CrossMark);
                    return;
                }

                if (ulong.TryParse(cardIdStr, out var cardId))
                {
                    await HandleDeleteAsync(thisPlayer, cardId!);
                }

                ResetExpiry();
            }
            else if (commandType.Contains("dodaj"))
            {
                var ids = new List<ulong>();

                foreach (var cardIdStr in splitedCommand)
                {
                    if (ulong.TryParse(cardIdStr, out var cardId))
                    {
                        ids.Add(cardId);
                    }
                }

                if (ids.Any())
                {
                    await HandleAddAsync(thisPlayer, ids, targetPlayer!);
                }
                else
                {
                    await userMessage.AddReactionAsync(_iconConfiguration.CrossMark);
                }

                ResetExpiry();
            }
        }

        private async Task HandleAddAsync(PlayerInfo player, IEnumerable<ulong> cardIds, PlayerInfo target)
        {
            bool hasError = false;
            bool added = false;
            var gameDeckCards = player.DatabaseUser.GameDeck.Cards;
            var cards = cardIds.Join(gameDeckCards, pr => pr, pr => pr.Id, (src, dst) => dst);
            var targetGamedeck = target.DatabaseUser.GameDeck;

            foreach (var card in cards)
            {
                if (card == null)
                {
                    hasError = true;
                    continue;
                }

                if (card.Expedition != ExpeditionCardType.None)
                {
                    hasError = true;
                    continue;
                }

                if (card.InCage || !card.IsTradable || card.IsBroken)
                {
                    hasError = true;
                    continue;
                }

                if (card.Dere == Dere.Yato)
                {
                    hasError = true;
                    continue;
                }

                if (card.Dere == Dere.Yami && targetGamedeck.IsGood())
                {
                    hasError = true;
                    continue;
                }

                if (card.Dere == Dere.Raito && targetGamedeck.IsEvil())
                {
                    hasError = true;
                    continue;
                }

                if (player.Cards.Any(x => x.Id == card.Id))
                {
                    continue;
                }

                if (card.FromFigure)
                {
                    if (card.PAS != PreAssembledFigure.None)
                    {
                        hasError = true;
                        continue;
                    }

                    if (targetGamedeck.Cards
                        .Any(x => x.FromFigure && x.CharacterId == card.CharacterId))
                    {
                        hasError = true;
                        continue;
                    }
                }

                player.Cards.Add(card);
                added = true;
            }

            player.Accepted = false;
            player.CustomString = BuildProposition(player);

            if (added)
            {
                await _userMessage.AddReactionAsync(_iconConfiguration.InboxTray);
            }

            if (hasError)
            {
                await _userMessage.AddReactionAsync(_iconConfiguration.CrossMark);
            }

            var embed = BuildEmbed();
            await _userMessage.ModifyAsync(x => x.Embed = embed);
        }

        private async Task HandleDeleteAsync(PlayerInfo player, ulong cardId)
        {
            var card = player.Cards.FirstOrDefault(x => x.Id == cardId);
            if (card == null)
            {
                await _userMessage.AddReactionAsync(_iconConfiguration.CrossMark);
                return;
            }

            if (!player.Cards.Any(x => x.Id == card.Id))
            {
                return;
            }

            player.Accepted = false;
            player.Cards.Remove(card);
            player.CustomString = BuildProposition(player);

            await _userMessage.AddReactionAsync(Emojis.OutboxTray);
            var embed = BuildEmbed();
            await _userMessage.ModifyAsync(x => x.Embed = embed);
        }

        private string BuildProposition(PlayerInfo player)
        {
            var mention = player.Mention;
            if (player.Cards.Count > 12)
            {
                return $"{mention} oferuje:\n\n**[{player.Cards.Count}]** kart";
            }

            return $"{mention} oferuje:\n{string.Join("\n", player.Cards.Select(x => x.GetString(false, false, true)))}";
        }

        private async Task<bool> HandleReactionAsync(IReaction reaction)
        {
            switch (_state)
            {
                case ExchangeStatus.AcceptSourcePlayer:
                    return await HandleUserReactionInAccept(reaction, _sourcePlayer);
                case ExchangeStatus.AcceptDestinationPlayer:
                    return await HandleUserReactionInAccept(reaction, _destinationPlayer);
                default:
                case ExchangeStatus.Add:
                    await HandleReactionInAdd(reaction);
                    return false;
            }
        }

        private async Task HandleReactionInAdd(IReaction reaction)
        {
            var userId = reaction.GetUserId();
            var emote = reaction.Emote;

            if (emote.Equals(_iconConfiguration.OneEmote) && userId == _sourcePlayer.DiscordId)
            {
                _sourcePlayer.Accepted = true;
                ResetExpiry();
            }
            else if (emote.Equals(_iconConfiguration.TwoEmote) && userId == _destinationPlayer.DiscordId)
            {
                _destinationPlayer.Accepted = true;
                ResetExpiry();
            }

            if (_sourcePlayer.Accepted && _destinationPlayer.Accepted)
            {
                _state = ExchangeStatus.AcceptSourcePlayer;
                _tips = $"{_sourcePlayer.Mention} daj {_iconConfiguration.Accept} aby zaakceptować, lub {_iconConfiguration.Decline} aby odrzucić.";

                await _userMessage.RemoveAllReactionsAsync();
                await _userMessage.ModifyAsync(x => x.Embed = BuildEmbed());
                await _userMessage.AddReactionsAsync(_iconConfiguration.AcceptDecline);
            }
        }

        private async Task<bool> HandleUserReactionInAccept(IReaction reaction, PlayerInfo player)
        {
            var messageChannel = false;
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            _cacheManager = _serviceProvider.GetRequiredService<ICacheManager>();
            var userId = reaction.GetUserId();
            var emote = reaction.Emote;
            var canComplete = false;

            if (userId != player.DiscordId)
            {
                return canComplete;
            }

            if (emote.Equals(_iconConfiguration.Accept))
            {
                if (_state == ExchangeStatus.AcceptSourcePlayer)
                {
                    messageChannel = true;
                    ResetExpiry();
                    _state = ExchangeStatus.AcceptDestinationPlayer;
                    _tips = $"{_destinationPlayer.Mention} daj {_iconConfiguration.Accept} aby zaakceptować, lub {_iconConfiguration.Decline} aby odrzucić.";
                }
                else if (_state == ExchangeStatus.AcceptDestinationPlayer)
                {
                    _tips = $"Wymiana zakończona!";
                    messageChannel = true;
                    var sourceCards = _sourcePlayer.Cards;
                    var destinationCards = _destinationPlayer.Cards;

                    if (sourceCards.Count == 0 && destinationCards.Count == 0)
                    {
                        return canComplete;
                    }

                    var sourceUser = await userRepository.GetUserOrCreateAsync(_sourcePlayer.DiscordId);
                    var destinationUser = await userRepository.GetUserOrCreateAsync(_destinationPlayer.DiscordId);

                    var avgValueP1 = GetAverageValue(sourceCards);
                    var avgValueP2 = GetAverageValue(destinationCards);

                    var avgRarP1 = GetAverageRarity(sourceCards);
                    var avgRarP2 = GetAverageRarity(destinationCards);

                    var avgRarDif = avgRarP1 - avgRarP2;

                    if (avgRarDif > 0)
                    {
                        avgValueP1 /= avgRarDif + 1;
                    }
                    else if (avgRarDif < 0)
                    {
                        avgRarDif = -avgRarDif;
                        avgValueP2 /= avgRarDif + 1;
                    }

                    var divP1 = sourceCards.Count * ((avgValueP1 <= 0) ? 1 : avgValueP1);
                    var divP2 = destinationCards.Count * ((avgValueP2 <= 0) ? 1 : avgValueP2);

                    var exchangeRateP1 = divP2 / ((destinationCards.Count == 0) ? (divP2 * 0.5) : divP1);
                    var exchangeRateP2 = divP1 / ((destinationCards.Count == 0) ? (divP1 * 0.5) : divP2);

                    if (exchangeRateP1 > 1)
                    {
                        exchangeRateP1 = 10;
                    }

                    if (exchangeRateP1 < 0.0001)
                    {
                        exchangeRateP1 = 0.001;
                    }

                    if (exchangeRateP2 > 1)
                    {
                        exchangeRateP2 = 10;
                    }

                    if (exchangeRateP2 < 0.0001)
                    {
                        exchangeRateP2 = 0.001;
                    }

                    foreach (var sourceCard in sourceCards)
                    {
                        var gameDeck = sourceUser.GameDeck;
                        var card = gameDeck.Cards.FirstOrDefault(x => x.Id == sourceCard.Id);
                        if (card == null)
                        {
                            continue;
                        }

                        card.Active = false;
                        card.Tags.Clear();
                        card.Affection -= 1.5;

                        if (card.ExperienceCount > 1)
                        {
                            card.ExperienceCount *= 0.3;
                        }

                        var valueDiff = exchangeRateP1 - card.MarketValue;
                        var changed = card.MarketValue + (valueDiff * 0.8);

                        if (changed < 0.0001)
                        {
                            changed = 0.0001;
                        }

                        if (changed > 1)
                        {
                            changed = 1;
                        }

                        card.MarketValue = changed;

                        if (!card.FirstOwnerId.HasValue)
                        {
                            card.FirstOwnerId = sourceUser.Id;
                        }

                        gameDeck.RemoveFromWaifu(card);

                        card.GameDeckId = destinationUser.GameDeck.Id;

                        destinationUser.GameDeck.RemoveCharacterFromWishList(card.CharacterId);
                        destinationUser.GameDeck.RemoveCardFromWishList(card.Id);
                    }

                    foreach (var destinationCard in destinationCards)
                    {
                        var gameDeck = destinationUser.GameDeck;
                        var card = gameDeck.Cards.FirstOrDefault(x => x.Id == destinationCard.Id);
                        if (card == null)
                        {
                            continue;
                        }

                        card.Active = false;
                        card.Tags.Clear();
                        card.Affection -= 1.5;

                        if (card.ExperienceCount > 1)
                        {
                            card.ExperienceCount *= 0.3;
                        }

                        var valueDiff = exchangeRateP2 - card.MarketValue;
                        var changed = card.MarketValue + (valueDiff * 0.8);
                        if (changed < 0.0001)
                        {
                            changed = 0.0001;
                        }

                        if (changed > 1)
                        {
                            changed = 1;
                        }

                        card.MarketValue = changed;

                        if (!card.FirstOwnerId.HasValue)
                        {
                            card.FirstOwnerId = destinationUser.Id;
                        }

                        gameDeck.RemoveFromWaifu(card);

                        card.GameDeckId = sourceUser.GameDeck.Id;

                        sourceUser.GameDeck.RemoveCharacterFromWishList(card.CharacterId);
                        sourceUser.GameDeck.RemoveCardFromWishList(card.Id);
                    }

                    await userRepository.SaveChangesAsync();

                    _state = ExchangeStatus.End;
                    canComplete = true;

                    _cacheManager.ExpireTag(
                        CacheKeys.User(_sourcePlayer.DiscordId),
                        CacheKeys.User(_destinationPlayer.DiscordId),
                        CacheKeys.Users);
                }
            }
            else if (emote.Equals(_iconConfiguration.Decline)
                    && _state != ExchangeStatus.End)
            {
                ResetExpiry();
                _tips = $"{player.Mention} odrzucił propozycje wymiany!";
                messageChannel = true;
            }

            if (messageChannel)
            {
                await _userMessage.ModifyAsync(x => x.Embed = BuildEmbed());
            }

            return canComplete;
        }
    }
}