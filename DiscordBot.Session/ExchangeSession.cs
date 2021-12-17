using Discord;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
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
        private readonly ExchangeSessionPayload _payload;
        private ICacheManager _cacheManager = null;
        private IIconConfiguration _iconConfiguration = null;
        private IServiceProvider _serviceProvider = null;

        public class ExchangeSessionPayload
        {
            public IMessage Message { get; set; } = null;

            public PlayerInfo SourcePlayer { get; set; } = null;

            public PlayerInfo DestinationPlayer { get; set; } = null;

            public string Name { get; set; } = null;

            public string Tips { get; set; } = null;

            public ExchangeStatus State { get; set; } = ExchangeStatus.Add;

        }

        public ExchangeSession(
          ulong ownerId,
          DateTime createdOn,
          ExchangeSessionPayload payload)
            : base(
          ownerId,
          createdOn,
          TimeSpan.FromMinutes(2),
          Discord.Commands.RunMode.Sync,
          SessionExecuteCondition.AllEvents)
        {
            _payload = payload;
        }

        public override async Task ExecuteAsync(
           SessionContext sessionContext,
           IServiceProvider serviceProvider,
           CancellationToken cancellationToken = default)
        {
            IsRunning = true;
            _serviceProvider = serviceProvider;

            if (_payload.SourcePlayer == null || _payload.DestinationPlayer == null || _payload.Message == null)
            {
                return;
            }

            _iconConfiguration = _serviceProvider.GetRequiredService<IIconConfiguration>();
            await HandleMessageAsync(sessionContext);
            await HandleReactionAsync(sessionContext);
            IsRunning = false;
        }

        public Embed BuildEmbed()
        {
            var description = $"{_payload.Name}\n\n{_payload.SourcePlayer.CustomString}\n\n{_payload.DestinationPlayer.CustomString}\n\n{_payload.Tips}".ElipseTrimToLength(2000);
            return new EmbedBuilder
            {
                Color = EMType.Warning.Color(),
                Description = description,
            }.Build();
        }

        private async Task HandleMessageAsync(SessionContext context)
        {
            if (context.AddReaction != null || context.RemoveReaction != null)
            {
                return;
            }

            if (_payload.State != ExchangeStatus.Add)
            {
                return;
            }

            if (context.Message.Id == _payload.Message.Id)
            {
                return;
            }

            if (context.Message.Channel.Id != _payload.Message.Channel.Id)
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

            if (userId == _payload.SourcePlayer.DiscordId)
            {
                thisPlayer = _payload.SourcePlayer;
                targetPlayer = _payload.DestinationPlayer;
            }

            if (userId == _payload.DestinationPlayer.DiscordId)
            {
                thisPlayer = _payload.DestinationPlayer;
                targetPlayer = _payload.SourcePlayer;
            }

            if (thisPlayer == null)
            {
                return;
            }

            if (commandType.Contains("usuń") || commandType.Contains("usun"))
            {
                var WIDStr = splitedCommand?[1];
                if (string.IsNullOrEmpty(WIDStr))
                {
                    await userMessage.AddReactionAsync(_iconConfiguration.CrossMark);
                    return;
                }

                if (ulong.TryParse(WIDStr, out var WID))
                {
                    await HandleDeleteAsync(thisPlayer, WID, userMessage!);
                }
                ResetExpiry();
            }
            else if (commandType.Contains("dodaj"))
            {
                var ids = new List<ulong>();
                foreach (var WIDStr in splitedCommand)
                {
                    if (ulong.TryParse(WIDStr, out var WID))
                    {
                        ids.Add(WID);
                    }
                }

                if (ids.Any())
                {
                    await HandleAddAsync(thisPlayer, ids, userMessage!, targetPlayer!);
                }
                else
                {
                    await userMessage.AddReactionAsync(_iconConfiguration.CrossMark);
                }

                ResetExpiry();
            }
        }

        private async Task HandleAddAsync(PlayerInfo player, IEnumerable<ulong> wids, IUserMessage message, PlayerInfo target)
        {
            bool error = false;
            bool added = false;
            var gameDeckCards = player.DatabaseUser.GameDeck.Cards;
            var cards = wids.Join(gameDeckCards, pr => pr, pr => pr.Id, (src, dst) => dst);
            var targetGamedeck = target.DatabaseUser.GameDeck;

            foreach (var card in cards)
            {
                if (card == null)
                {
                    error = true;
                    continue;
                }

                if (card.Expedition != ExpeditionCardType.None)
                {
                    error = true;
                    continue;
                }

                if (card.InCage || !card.IsTradable || card.IsBroken)
                {
                    error = true;
                    continue;
                }

                if (card.Dere == Dere.Yato)
                {
                    error = true;
                    continue;
                }

                if (card.Dere == Dere.Yami && targetGamedeck.IsGood())
                {
                    error = true;
                    continue;
                }

                if (card.Dere == Dere.Raito && targetGamedeck.IsEvil())
                {
                    error = true;
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
                        error = true;
                        continue;
                    }

                    if (targetGamedeck.Cards
                        .Any(x => x.FromFigure && x.CharacterId == card.CharacterId))
                    {
                        error = true;
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
                await message.AddReactionAsync(_iconConfiguration.InboxTray);
            }

            if (error)
            {
                await message.AddReactionAsync(_iconConfiguration.CrossMark);
            }

            if (await _payload.Message.Channel.GetMessageAsync(_payload.Message.Id) is IUserMessage userMessage)
            {
                var embed = BuildEmbed();
                await userMessage.ModifyAsync(x => x.Embed = embed);
            }
        }

        private async Task HandleDeleteAsync(PlayerInfo player, ulong wid, IUserMessage message)
        {
            var card = player.Cards.FirstOrDefault(x => x.Id == wid);
            if (card == null)
            {
                await message.AddReactionAsync(_iconConfiguration.CrossMark);
                return;
            }

            if (!player.Cards.Any(x => x.Id == card.Id))
                return;

            player.Accepted = false;
            player.Cards.Remove(card);
            player.CustomString = BuildProposition(player);

            await message.AddReactionAsync(Emojis.OutboxTray);

            if (await _payload.Message.Channel.GetMessageAsync(_payload.Message.Id) is IUserMessage userMessage)
            {
                await userMessage.ModifyAsync(x => x.Embed = BuildEmbed());
            }
        }

        public string BuildProposition(PlayerInfo player)
        {
            var mention = player.Mention;
            if (player.Cards.Count > 12)
            {
                return $"{mention} oferuje:\n\n**[{player.Cards.Count}]** kart";
            }

            return $"{mention} oferuje:\n{string.Join("\n", player.Cards.Select(x => x.GetString(false, false, true)))}";
        }

        private async Task HandleReactionAsync(SessionContext context)
        {
            if (context.Message.Id != _payload.Message.Id)
            {
                return;
            }

            if (!(await _payload.Message.Channel.GetMessageAsync(_payload.Message.Id) is IUserMessage message))
            {
                return;
            }

            var reaction = context.AddReaction ?? context.RemoveReaction;

            if (reaction == null || message == null)
            {
                return;
            }

            switch (_payload.State)
            {
                case ExchangeStatus.AcceptSourcePlayer:
                    await HandleUserReactionInAccept(reaction, _payload.SourcePlayer, message);
                    break;

                case ExchangeStatus.AcceptDestinationPlayer:
                    await HandleUserReactionInAccept(reaction, _payload.DestinationPlayer, message);
                    break;

                default:
                case ExchangeStatus.Add:
                    await HandleReactionInAdd(reaction, message);
                    break;
            }

        }

        private async Task HandleReactionInAdd(IReaction reaction, IUserMessage userMessage)
        {
            var userId = reaction.GetUserId();
            if (reaction.Emote.Equals(_iconConfiguration.OneEmote) && userId == _payload.SourcePlayer.DiscordId)
            {
                _payload.SourcePlayer.Accepted = true;
                ResetExpiry();
            }
            else if (reaction.Emote.Equals(_iconConfiguration.TwoEmote) && userId == _payload.DestinationPlayer.DiscordId)
            {
                _payload.DestinationPlayer.Accepted = true;
                ResetExpiry();
            }

            if (_payload.SourcePlayer.Accepted && _payload.DestinationPlayer.Accepted)
            {
                _payload.State = ExchangeStatus.AcceptSourcePlayer;
                _payload.Tips = $"{_payload.SourcePlayer.Mention} daj {_iconConfiguration.Accept} aby zaakceptować, lub {_iconConfiguration.Decline} aby odrzucić.";

                await userMessage.RemoveAllReactionsAsync();
                await userMessage.ModifyAsync(x => x.Embed = BuildEmbed());
                await userMessage.AddReactionsAsync(_iconConfiguration.AcceptDecline);
            }
        }

        public double GetAvgValue(List<Card> cards)
        {
            if (cards.Count < 1) return 0.01;
            return cards.Average(x => x.MarketValue);
        }

        public double GetAvgRarity(List<Card> cards)
        {
            if (cards.Count < 1) return (int)Rarity.E;
            return cards.Average(x => (int)x.Rarity);
        }

        private async Task HandleUserReactionInAccept(IReaction reaction, PlayerInfo player, IUserMessage message)
        {
            var messageChannel = false;
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            _cacheManager = _serviceProvider.GetRequiredService<ICacheManager>();
            var userId = reaction.GetUserId();

            if (userId != player.DiscordId)
            {
                return;
            }

            if (reaction.Emote.Equals(Emojis.Checked))
            {
                if (_payload.State == ExchangeStatus.AcceptSourcePlayer)
                {
                    messageChannel = true;
                    ResetExpiry();
                    _payload.State = ExchangeStatus.AcceptDestinationPlayer;
                    _payload.Tips = $"{_payload.DestinationPlayer.Mention} daj {_iconConfiguration.Accept} aby zaakceptować, lub {_iconConfiguration.Decline} aby odrzucić.";
                }
                else if (_payload.State == ExchangeStatus.AcceptDestinationPlayer)
                {
                    _payload.Tips = $"Wymiana zakończona!";
                    messageChannel = true;
                    var sourceCards = _payload.SourcePlayer.Cards;
                    var destinationCards = _payload.DestinationPlayer.Cards;

                    if (sourceCards.Count == 0 && destinationCards.Count == 0)
                    {
                        return;
                    }

                    var sourceUser = await userRepository.GetUserOrCreateAsync(_payload.SourcePlayer.DiscordId);
                    var destinationUser = await userRepository.GetUserOrCreateAsync(_payload.DestinationPlayer.DiscordId);

                    double avgValueP1 = GetAvgValue(sourceCards);
                    double avgValueP2 = GetAvgValue(destinationCards);

                    double avgRarP1 = GetAvgRarity(sourceCards);
                    double avgRarP2 = GetAvgRarity(destinationCards);

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

                    if (exchangeRateP1 > 1) exchangeRateP1 = 10;
                    if (exchangeRateP1 < 0.0001) exchangeRateP1 = 0.001;

                    if (exchangeRateP2 > 1) exchangeRateP2 = 10;
                    if (exchangeRateP2 < 0.0001) exchangeRateP2 = 0.001;

                    foreach (var sourceCard in sourceCards)
                    {
                        var card = sourceUser.GameDeck.Cards.FirstOrDefault(x => x.Id == sourceCard.Id);
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
                        var changed = card.MarketValue + valueDiff * 0.8;
                        if (changed < 0.0001) changed = 0.0001;
                        if (changed > 1) changed = 1;
                        card.MarketValue = changed;

                        if (!card.FirstOwnerId.HasValue)
                        {
                            card.FirstOwnerId = sourceUser.Id;
                        }

                        sourceUser.GameDeck.RemoveFromWaifu(card);

                        card.GameDeckId = destinationUser.GameDeck.Id;

                        destinationUser.GameDeck.RemoveCharacterFromWishList(card.CharacterId);
                        destinationUser.GameDeck.RemoveCardFromWishList(card.Id);
                    }

                    foreach (var destinationCard in destinationCards)
                    {
                        var card = destinationUser.GameDeck.Cards.FirstOrDefault(x => x.Id == destinationCard.Id);
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
                        var changed = card.MarketValue + valueDiff * 0.8;
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

                        destinationUser.GameDeck.RemoveFromWaifu(card);

                        card.GameDeckId = sourceUser.GameDeck.Id;

                        sourceUser.GameDeck.RemoveCharacterFromWishList(card.CharacterId);
                        sourceUser.GameDeck.RemoveCardFromWishList(card.Id);
                    }

                    await userRepository.SaveChangesAsync();

                    _payload.State = ExchangeStatus.End;

                    _cacheManager.ExpireTag(
                        CacheKeys.User(_payload.SourcePlayer.DiscordId),
                        CacheKeys.User(_payload.DestinationPlayer.DiscordId),
                        CacheKeys.Users);
                }
            }
            else if (reaction.Emote.Equals(_iconConfiguration.Decline) && _payload.State != ExchangeStatus.End)
            {
                ResetExpiry();
                _payload.Tips = $"{player.Mention} odrzucił propozycje wymiany!";
                messageChannel = true;
            }

            if (message != null && messageChannel)
            {
                await message.ModifyAsync(x => x.Embed = BuildEmbed());
            }
        }

        public override async ValueTask DisposeAsync()
        {
            if (_payload.Message == null)
            {
                return;
            }

            if (await _payload.Message.Channel.GetMessageAsync(_payload.Message.Id) is IUserMessage userMessage)
            {
                try
                {
                    await userMessage.RemoveAllReactionsAsync();
                }
                catch (Exception) { }
            }

            _payload.Message = null;
            _payload.Name = null;
            _payload.Tips = null;
            _payload.SourcePlayer = null;
            _payload.DestinationPlayer = null;
            _serviceProvider = null;
        }
    }
}