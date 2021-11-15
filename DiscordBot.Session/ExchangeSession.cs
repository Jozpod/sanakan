using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.Extensions;
using Sanakan.Game.Extensions;
using Sanakan.Game.Models;
using Sanakan.Services.PocketWaifu;

namespace Sanakan.DiscordBot.Session
{
    public class ExchangeSession : InteractionSession
    {
        private readonly ExchangeSessionPayload _payload;
        private readonly ICacheManager _cacheManager;
        private IServiceProvider _serviceProvider { get; set; }

        public class ExchangeSessionPayload
        {
            public IMessage Message { get; set; }
            public PlayerInfo SourcePlayer { get; set; }
            public PlayerInfo DestinationPlayer { get; set; }
            public string Name { get; set; }
            public string Tips { get; set; }

            public ExchangeStatus State { get; set; } = ExchangeStatus.Add;

        }

        public IEmote[] StartReactions => new IEmote[] { Emojis.OneEmote, Emojis.TwoEmote };

        public ExchangeSession(
          ulong ownerId,
          DateTime createdOn,
          ExchangeSessionPayload payload) : base(
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
            _serviceProvider = serviceProvider;

            if (_payload.SourcePlayer == null || _payload.DestinationPlayer == null || _payload.Message == null)
            {
                return;
            }

            await HandleMessageAsync(sessionContext);
            await HandleReactionAsync(sessionContext);
            return;
        }

        public Embed BuildEmbed()
        {
            return new EmbedBuilder
            {
                Color = EMType.Warning.Color(),
                Description = $"{_payload.Name}\n\n{_payload.SourcePlayer.CustomString}\n\n{_payload.DestinationPlayer.CustomString}\n\n{_payload.Tips}".ElipseTrimToLength(2000)
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

            var cmd = context.Message?.Content?.ToLower();
            if (cmd == null)
            {
                return;
            }

            var splitedCmd = cmd.Replace("\n", " ").Split(" ");
            if (splitedCmd.Length < 2)
            {
                return;
            }

            var cmdType = splitedCmd[0];
            if (cmdType == null)
            {
                return;
            }

            PlayerInfo? thisPlayer = null;
            PlayerInfo? targetPlayer = null;

            var socketUserMessage = context.Message;

            if (context.User.Id == _payload.SourcePlayer.User.Id)
            {
                thisPlayer = _payload.SourcePlayer;
                targetPlayer = _payload.DestinationPlayer;
            }
            if (context.User.Id == _payload.DestinationPlayer.User.Id)
            {
                thisPlayer = _payload.DestinationPlayer;
                targetPlayer = _payload.SourcePlayer;
            }
            if (thisPlayer == null)
            {
                return;
            }

            if (cmdType.Contains("usuń") || cmdType.Contains("usun"))
            {
                var WIDStr = splitedCmd?[1];
                if (string.IsNullOrEmpty(WIDStr))
                {
                    await socketUserMessage.AddReactionAsync(Emojis.CrossMark);
                    return;
                }

                if (ulong.TryParse(WIDStr, out var WID))
                {
                    await HandleDeleteAsync(thisPlayer, WID, socketUserMessage);
                }
                ResetExpiry();
            }
            else if (cmdType.Contains("dodaj"))
            {
                var ids = new List<ulong>();
                foreach (var WIDStr in splitedCmd)
                {
                    if (ulong.TryParse(WIDStr, out var WID))
                    {
                        ids.Add(WID);
                    }
                }

                if (ids.Any())
                {
                    await HandleAddAsync(thisPlayer, ids, socketUserMessage, targetPlayer);
                }
                else
                {
                    await socketUserMessage.AddReactionAsync(Emojis.CrossMark);
                }

                ResetExpiry();
            }
        }

        private async Task HandleAddAsync(PlayerInfo player, List<ulong> wid, IUserMessage message, PlayerInfo target)
        {
            bool error = false;
            bool added = false;

            foreach (var id in wid)
            {
                var card = player.DatabaseUser.GameDeck.Cards.FirstOrDefault(x => x.Id == id);
                
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

                if (card.Dere == Dere.Yami && target.DatabaseUser.GameDeck.IsGood())
                {
                    error = true;
                    continue;
                }

                if (card.Dere == Dere.Raito && target.DatabaseUser.GameDeck.IsEvil())
                {
                    error = true;
                    continue;
                }

                if (player.Cards.Any(x => x.Id == card.Id))
                    continue;

                if (card.FromFigure)
                {
                    if (card.PAS != PreAssembledFigure.None)
                    {
                        error = true;
                        continue;
                    }

                    if (target.DatabaseUser.GameDeck.Cards
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
                await message.AddReactionAsync(Emojis.InboxTray);
            }

            if (error)
            {
                await message.AddReactionAsync(Emojis.CrossMark);
            }

            if (await _payload.Message.Channel.GetMessageAsync(_payload.Message.Id) is IUserMessage msg)
            {
                await msg.ModifyAsync(x => x.Embed = BuildEmbed());
            }
        }

        private async Task HandleDeleteAsync(PlayerInfo player, ulong wid, IUserMessage message)
        {
            var card = player.Cards.FirstOrDefault(x => x.Id == wid);
            if (card == null)
            {
                await message.AddReactionAsync(Emojis.CrossMark);
                return;
            }

            if (!player.Cards.Any(x => x.Id == card.Id))
                return;

            player.Accepted = false;
            player.Cards.Remove(card);
            player.CustomString = BuildProposition(player);

            await message.AddReactionAsync(Emojis.OutboxTray);

            if (await _payload.Message.Channel.GetMessageAsync(_payload.Message.Id) is IUserMessage msg)
            {
                await msg.ModifyAsync(x => x.Embed = BuildEmbed());
            }
        }

        public string BuildProposition(PlayerInfo player)
        {
            if (player.Cards.Count > 12)
            {
                return $"{player.User.Mention} oferuje:\n\n**[{player.Cards.Count}]** kart";
            }

            return $"{player.User.Mention} oferuje:\n{string.Join("\n", player.Cards.Select(x => x.GetString(false, false, true)))}";
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
                case ExchangeStatus.AcceptP1:
                    await HandleUserReactionInAccept(reaction, _payload.SourcePlayer, message);
                    break;

                case ExchangeStatus.AcceptP2:
                    await HandleUserReactionInAccept(reaction, _payload.DestinationPlayer, message);
                    break;

                default:
                case ExchangeStatus.Add:
                    await HandleReactionInAdd(reaction, message);
                    break;
            }

        }

        private async Task HandleReactionInAdd(SocketReaction reaction, IUserMessage msg)
        {
            if (reaction.Emote.Equals(Emojis.OneEmote) && reaction.UserId == _payload.SourcePlayer.User.Id)
            {
                _payload.SourcePlayer.Accepted = true;
                ResetExpiry();
            }
            else if (reaction.Emote.Equals(Emojis.TwoEmote) && reaction.UserId == _payload.DestinationPlayer.User.Id)
            {
                _payload.DestinationPlayer.Accepted = true;
                ResetExpiry();
            }

            if (_payload.SourcePlayer.Accepted && _payload.DestinationPlayer.Accepted)
            {
                _payload.State = ExchangeStatus.AcceptP1;
                _payload.Tips = $"{_payload.SourcePlayer.User.Mention} daj {Emojis.Checked} aby zaakceptować, lub {Emojis.DeclineEmote} aby odrzucić.";

                await msg.RemoveAllReactionsAsync();
                await msg.ModifyAsync(x => x.Embed = BuildEmbed());
                await msg.AddReactionsAsync(new IEmote[] {
                    Emojis.Checked,
                    Emojis.DeclineEmote
                });
            }
        }

        private async Task HandleUserReactionInAccept(SocketReaction reaction, PlayerInfo player, IUserMessage message)
        {
            var msgCh = false;
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();

            if (reaction.UserId != player.User.Id)
            {
                return;
            }

            if (reaction.Emote.Equals(Emojis.Checked))
            {
                if (_payload.State == ExchangeStatus.AcceptP1)
                {
                    msgCh = true;
                    ResetExpiry();
                    _payload.State = ExchangeStatus.AcceptP2;
                    _payload.Tips = $"{_payload.DestinationPlayer.User.Mention} daj {Emojis.Checked} aby zaakceptować, lub {Emojis.DeclineEmote} aby odrzucić.";
                }
                else if (_payload.State == ExchangeStatus.AcceptP2)
                {
                    _payload.Tips = $"Wymiana zakończona!";
                    msgCh = true;

                    if (_payload.SourcePlayer.Cards.Count == 0 && _payload.DestinationPlayer.Cards.Count == 0)
                    {
                        return;
                    }

                    var sourceUser = await userRepository.GetUserOrCreateAsync(_payload.SourcePlayer.User.Id);
                    var user2 = await userRepository.GetUserOrCreateAsync(_payload.DestinationPlayer.User.Id);

                    double avgValueP1 = _payload.SourcePlayer.Cards.Sum(x => x.MarketValue) / ((_payload.SourcePlayer.Cards.Count == 0) ? 1 : _payload.SourcePlayer.Cards.Count);
                    double avgValueP2 = _payload.DestinationPlayer.Cards.Sum(x => x.MarketValue) / ((_payload.DestinationPlayer.Cards.Count == 0) ? 1 : _payload.DestinationPlayer.Cards.Count);

                    double avgRarP1 = _payload.SourcePlayer.Cards.Sum(x => (int)x.Rarity) / ((_payload.SourcePlayer.Cards.Count == 0) ? 1 : _payload.SourcePlayer.Cards.Count);
                    double avgRarP2 = _payload.DestinationPlayer.Cards.Sum(x => (int)x.Rarity) / ((_payload.DestinationPlayer.Cards.Count == 0) ? 1 : _payload.DestinationPlayer.Cards.Count);
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

                    var divP1 = _payload.SourcePlayer.Cards.Count / ((avgValueP1 <= 0) ? 1 : avgValueP1);
                    var divP2 = _payload.DestinationPlayer.Cards.Count / ((avgValueP2 <= 0) ? 1 : avgValueP2);

                    var exchangeRateP1 = divP2 / ((_payload.SourcePlayer.Cards.Count == 0) ? (divP2 * 0.5) : divP1);
                    var exchangeRateP2 = divP1 / ((_payload.DestinationPlayer.Cards.Count == 0) ? (divP1 * 0.5) : divP2);

                    if (exchangeRateP1 > 1) exchangeRateP1 = 10;
                    if (exchangeRateP1 < 0.0001) exchangeRateP1 = 0.001;

                    if (exchangeRateP2 > 1) exchangeRateP2 = 10;
                    if (exchangeRateP2 < 0.0001) exchangeRateP2 = 0.001;

                    foreach (var sourceCards in _payload.SourcePlayer.Cards)
                    {
                        var card = sourceUser.GameDeck.Cards.FirstOrDefault(x => x.Id == sourceCards.Id);
                        if (card == null)
                        {
                            continue;
                        }

                        card.Active = false;
                        card.TagList.Clear();
                        card.Affection -= 1.5;

                        if (card.ExperienceCount > 1)
                        {
                            card.ExperienceCount *= 0.3;
                        }

                        var valueDiff = card.MarketValue - exchangeRateP1;
                        var changed = card.MarketValue + valueDiff * 0.8;
                        if (changed < 0.0001) changed = 0.0001;
                        if (changed > 1) changed = 1;
                        card.MarketValue = changed;

                        if (!card.FirstOwnerId.HasValue)
                        {
                            card.FirstOwnerId = sourceUser.Id;
                        }

                        sourceUser.GameDeck.RemoveFromWaifu(card);

                        card.GameDeckId = user2.GameDeck.Id;

                        user2.GameDeck.RemoveCharacterFromWishList(card.CharacterId);
                        user2.GameDeck.RemoveCardFromWishList(card.Id);
                    }

                    foreach (var destinationCards in _payload.DestinationPlayer.Cards)
                    {
                        var card = user2.GameDeck.Cards.FirstOrDefault(x => x.Id == destinationCards.Id);
                        if (card == null)
                        {
                            continue;
                        }

                        card.Active = false;
                        card.TagList.Clear();
                        card.Affection -= 1.5;

                        if (card.ExperienceCount > 1)
                            card.ExperienceCount *= 0.3;

                        var valueDiff = card.MarketValue - exchangeRateP2;
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
                            card.FirstOwnerId = user2.Id;
                        }

                        user2.GameDeck.RemoveFromWaifu(card);

                        card.GameDeckId = sourceUser.GameDeck.Id;

                        sourceUser.GameDeck.RemoveCharacterFromWishList(card.CharacterId);
                        sourceUser.GameDeck.RemoveCardFromWishList(card.Id);
                    }

                    await userRepository.SaveChangesAsync();

                    _payload.State = ExchangeStatus.End;

                    _cacheManager.ExpireTag(
                        CacheKeys.User(_payload.SourcePlayer.User.Id),
                        CacheKeys.User(_payload.DestinationPlayer.User.Id),
                        CacheKeys.Users);
                }
            }
            else if (reaction.Emote.Equals(Emojis.DeclineEmote) && _payload.State != ExchangeStatus.End)
            {
                ResetExpiry();
                _payload.Tips = $"{player.User.Mention} odrzucił propozycje wymiany!";
                msgCh = true;
            }

            if (message != null && msgCh)
            {
                await message.ModifyAsync(x => x.Embed = BuildEmbed());
            }
        }

        private async Task DisposeAction()
        {
            if (_payload.Message == null)
            {
                return;
            }

            if (await _payload.Message.Channel.GetMessageAsync(_payload.Message.Id) is IUserMessage msg)
            {
                try
                {
                    await msg.RemoveAllReactionsAsync();
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