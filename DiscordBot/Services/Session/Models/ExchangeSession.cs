﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services.Session;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.Extensions;
using Sanakan.Services.PocketWaifu;

namespace Sanakan.Services.Session.Models
{
    public class ExchangeSession : Session
    {
        private enum ExchangeStatus
        {
            Add, AcceptP1, AcceptP2, End
        }

        public IMessage Message { get; set; }
        public PlayerInfo P1 { get; set; }
        public PlayerInfo P2 { get; set; }
        public string Name { get; set; }
        public string Tips { get; set; }

        private ExchangeStatus State;
        private readonly ICacheManager _cacheManager;
        private readonly IUserRepository _userRepository;
        public IEmote[] StartReactions => new IEmote[] { Emojis.OneEmote, Emojis.TwoEmote };

        public ExchangeSession(
            IUser owner,
            IUser exchanger) : base(owner)
        {
            State = ExchangeStatus.Add;
            Event = ExecuteOn.AllEvents;
            AddParticipant(exchanger);
            RunMode = RunMode.Sync;
            TimeoutMs = 120000;

            Message = null;

            OnExecute = ExecuteAction;
            OnDispose = DisposeAction;
        }

        private async Task<bool> ExecuteAction(SessionContext context, Session session)
        {
            if (P1 == null || P2 == null || Message == null)
                return true;

            await HandleMessageAsync(context);
            return await HandleReactionAsync(context);
        }

        public Embed BuildEmbed()
        {
            return new EmbedBuilder
            {
                Color = EMType.Warning.Color(),
                Description = $"{Name}\n\n{P1.CustomString}\n\n{P2.CustomString}\n\n{Tips}".TrimToLength(2000)
            }.Build();
        }

        private async Task HandleMessageAsync(SessionContext context)
        {
            if (context.ReactionAdded != null || context.ReactionRemoved != null)
                return;

            if (State != ExchangeStatus.Add)
                return;

            if (context.Message.Id == Message.Id)
                return;

            if (context.Message.Channel.Id != Message.Channel.Id)
                return;

            var cmd = context.Message?.Content?.ToLower();
            if (cmd == null) return;

            var splitedCmd = cmd.Replace("\n", " ").Split(" ");
            if (splitedCmd.Length < 2) return;

            var cmdType = splitedCmd[0];
            if (cmdType == null) return;

            PlayerInfo? thisPlayer = null;
            PlayerInfo? targetPlayer = null;

            var socketUserMessage = context.Message;

            if (context.User.Id == P1.User.Id)
            {
                thisPlayer = P1;
                targetPlayer = P2;
            }
            if (context.User.Id == P2.User.Id)
            {
                thisPlayer = P2;
                targetPlayer = P1;
            }
            if (thisPlayer == null) return;

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
                RestartTimer();
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

                RestartTimer();
            }
        }

        private async Task HandleAddAsync(PlayerInfo player, List<ulong> wid, IUserMessage message, PlayerInfo target)
        {
            bool error = false;
            bool added = false;

            foreach (var id in wid)
            {
                var card = player.Dbuser.GameDeck.Cards.FirstOrDefault(x => x.Id == id);
                if (card == null)
                {
                    error = true;
                    continue;
                }

                if (card.Expedition != CardExpedition.None)
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

                if (card.Dere == Dere.Yami && target.Dbuser.GameDeck.IsGood())
                {
                    error = true;
                    continue;
                }

                if (card.Dere == Dere.Raito && target.Dbuser.GameDeck.IsEvil())
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

                    if (target.Dbuser.GameDeck.Cards
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

            if (await Message.Channel.GetMessageAsync(Message.Id) is IUserMessage msg)
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

            if (await Message.Channel.GetMessageAsync(Message.Id) is IUserMessage msg)
            {
                await msg.ModifyAsync(x => x.Embed = BuildEmbed());
            }
        }

        public string BuildProposition(PlayerInfo player)
        {
            if (player.Cards.Count > 12)
                return $"{player.User.Mention} oferuje:\n\n**[{player.Cards.Count}]** kart";

            return $"{player.User.Mention} oferuje:\n{string.Join("\n", player.Cards.Select(x => x.GetString(false, false, true)))}";
        }

        private async Task<bool> HandleReactionAsync(SessionContext context)
        {
            bool end = false;
            if (context.Message.Id != Message.Id)
                return false;

            if (!(await Message.Channel.GetMessageAsync(Message.Id) is IUserMessage message))
            {
                return end;
            }

            var reaction = context.ReactionAdded ?? context.ReactionRemoved;

            if (reaction == null || message == null)
            {
                return false;
            }

            switch (State)
            {
                case ExchangeStatus.AcceptP1:
                    end = await HandleUserReactionInAccept(reaction, P1, message);
                    break;

                case ExchangeStatus.AcceptP2:
                    end = await HandleUserReactionInAccept(reaction, P2, message);
                    break;

                default:
                case ExchangeStatus.Add:
                    await HandleReactionInAdd(reaction, message);
                    break;
            }

            return end;
        }

        private async Task HandleReactionInAdd(SocketReaction reaction, IUserMessage msg)
        {
            if (reaction.Emote.Equals(Emojis.OneEmote) && reaction.UserId == P1.User.Id)
            {
                P1.Accepted = true;
                RestartTimer();
            }
            else if (reaction.Emote.Equals(Emojis.TwoEmote) && reaction.UserId == P2.User.Id)
            {
                P2.Accepted = true;
                RestartTimer();
            }

            if (P1.Accepted && P2.Accepted)
            {
                State = ExchangeStatus.AcceptP1;
                Tips = $"{P1.User.Mention} daj {Emojis.Checked} aby zaakceptować, lub {Emojis.DeclineEmote} aby odrzucić.";

                await msg.RemoveAllReactionsAsync();
                await msg.ModifyAsync(x => x.Embed = BuildEmbed());
                await msg.AddReactionsAsync(new IEmote[] {
                    Emojis.Checked,
                    Emojis.DeclineEmote
                });
            }
        }

        private async Task<bool> HandleUserReactionInAccept(SocketReaction reaction, PlayerInfo player, IUserMessage msg)
        {
            bool end = false;
            bool msgCh = false;

            if (reaction.UserId == player.User.Id)
            {
                if (reaction.Emote.Equals(Emojis.Checked))
                {
                    if (State == ExchangeStatus.AcceptP1)
                    {
                        msgCh = true;
                        RestartTimer();
                        State = ExchangeStatus.AcceptP2;
                        Tips = $"{P2.User.Mention} daj {Emojis.Checked} aby zaakceptować, lub {Emojis.DeclineEmote} aby odrzucić.";
                    }
                    else if (State == ExchangeStatus.AcceptP2)
                    {
                        Tips = $"Wymiana zakończona!";
                        msgCh = true;
                        end = true;

                        if (P1.Cards.Count == 0 && P2.Cards.Count == 0)
                        {
                            return end;
                        }
                            
                        var user1 = await _userRepository.GetUserOrCreateAsync(P1.User.Id);
                        var user2 = await _userRepository.GetUserOrCreateAsync(P2.User.Id);

                        double avgValueP1 = P1.Cards.Sum(x => x.MarketValue) / ((P1.Cards.Count == 0) ? 1 : P1.Cards.Count);
                        double avgValueP2 = P2.Cards.Sum(x => x.MarketValue) / ((P2.Cards.Count == 0) ? 1 : P2.Cards.Count);

                        double avgRarP1 = P1.Cards.Sum(x => (int)x.Rarity) / ((P1.Cards.Count == 0) ? 1 : P1.Cards.Count);
                        double avgRarP2 = P2.Cards.Sum(x => (int)x.Rarity) / ((P2.Cards.Count == 0) ? 1 : P2.Cards.Count);
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

                        var divP1 = P1.Cards.Count / ((avgValueP1 <= 0) ? 1 : avgValueP1);
                        var divP2 = P2.Cards.Count / ((avgValueP2 <= 0) ? 1 : avgValueP2);

                        var exchangeRateP1 = divP2 / ((P1.Cards.Count == 0) ? (divP2 * 0.5) : divP1);
                        var exchangeRateP2 = divP1 / ((P2.Cards.Count == 0) ? (divP1 * 0.5) : divP2);

                        if (exchangeRateP1 > 1) exchangeRateP1 = 10;
                        if (exchangeRateP1 < 0.0001) exchangeRateP1 = 0.001;

                        if (exchangeRateP2 > 1) exchangeRateP2 = 10;
                        if (exchangeRateP2 < 0.0001) exchangeRateP2 = 0.001;

                        foreach (var c in P1.Cards)
                        {
                            var card = user1.GameDeck.Cards.FirstOrDefault(x => x.Id == c.Id);
                            if (card != null)
                            {
                                card.Active = false;
                                card.TagList.Clear();
                                card.Affection -= 1.5;

                                if (card.ExpCnt > 1)
                                    card.ExpCnt *= 0.3;

                                var valueDiff = card.MarketValue - exchangeRateP1;
                                var changed = card.MarketValue + valueDiff * 0.8;
                                if (changed < 0.0001) changed = 0.0001;
                                if (changed > 1) changed = 1;
                                card.MarketValue = changed;

                                if (card.FirstIdOwner == 0)
                                    card.FirstIdOwner = user1.Id;

                                user1.GameDeck.RemoveFromWaifu(card);

                                card.GameDeckId = user2.GameDeck.Id;

                                user2.GameDeck.RemoveCharacterFromWishList(card.CharacterId);
                                user2.GameDeck.RemoveCardFromWishList(card.Id);
                            }
                        }

                        foreach (var c in P2.Cards)
                        {
                            var card = user2.GameDeck.Cards.FirstOrDefault(x => x.Id == c.Id);
                            if (card != null)
                            {
                                card.Active = false;
                                card.TagList.Clear();
                                card.Affection -= 1.5;

                                if (card.ExpCnt > 1)
                                    card.ExpCnt *= 0.3;

                                var valueDiff = card.MarketValue - exchangeRateP2;
                                var changed = card.MarketValue + valueDiff * 0.8;
                                if (changed < 0.0001) changed = 0.0001;
                                if (changed > 1) changed = 1;
                                card.MarketValue = changed;

                                if (card.FirstIdOwner == 0)
                                    card.FirstIdOwner = user2.Id;

                                user2.GameDeck.RemoveFromWaifu(card);

                                card.GameDeckId = user1.GameDeck.Id;

                                user1.GameDeck.RemoveCharacterFromWishList(card.CharacterId);
                                user1.GameDeck.RemoveCardFromWishList(card.Id);
                            }
                        }

                        await _userRepository.SaveChangesAsync();

                        State = ExchangeStatus.End;
                        _cacheManager.ExpireTag(new string[] { $"user-{P1.User.Id}", $"user-{P2.User.Id}", "users" });
                    }
                }
                else if (reaction.Emote.Equals(Emojis.DeclineEmote) && State != ExchangeStatus.End)
                {
                    RestartTimer();
                    Tips = $"{player.User.Mention} odrzucił propozycje wymiany!";
                    msgCh = true;
                    end = true;
                }

                if (msg != null && msgCh) await msg.ModifyAsync(x => x.Embed = BuildEmbed());
            }
            return end;
        }

        private async Task DisposeAction()
        {
            if (Message != null)
            {
                if (await Message.Channel.GetMessageAsync(Message.Id) is IUserMessage msg)
                {
                    try
                    {
                        await msg.RemoveAllReactionsAsync();
                    }
                    catch (Exception) { }
                }

                Message = null;
                Name = null;
                Tips = null;
                P1 = null;
                P2 = null;
            }
        }
    }
}