using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.Extensions;
using Sanakan.Services.PocketWaifu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue
{
    public class CraftSession : InteractionSession
    {
        private readonly CraftSessionPayload _payload;
        public IEmote[] StartReactions => new IEmote[] { Emojis.Checked, Emojis.DeclineEmote };

        public IServiceProvider _serviceProvider { get; private set; }

        public class CraftSessionPayload
        {
            public IMessage? Message { get; set; }
            public List<Item> Items { get; set; }
            public PlayerInfo? PlayerInfo { get; set; }
            public string Name { get; set; }
            public string Tips { get; set; }
        }

        public CraftSession(
            ulong ownerId,
            DateTime createdOn,
            CraftSessionPayload payload) : base(
                ownerId,
                createdOn,
                TimeSpan.FromMinutes(2),
                RunMode.Sync,
                SessionExecuteCondition.AllEvents)
        {
            _payload = payload;
        }

        public override async Task ExecuteAsync(
            SessionContext context,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default)
        {
            _serviceProvider = serviceProvider;

            if (_payload.PlayerInfo == null || _payload.Message == null)
            {
                return;
                //return true;
            }

            await HandleMessageAsync(context);
            await HandleReactionAsync(context);
            return; 
        }

        private async Task HandleMessageAsync(SessionContext context)
        {
            if (context.Message.Id == _payload.Message.Id)
                return;

            if (context.Message.Channel.Id != _payload.Message.Channel.Id)
                return;

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

            int itemNum = -1;
            var itemNr = splitedCmd[1];
            if (!string.IsNullOrEmpty(itemNr))
            {
                if (int.TryParse(itemNr, out var num))
                {
                    itemNum = num;
                }
            }

            int itemCount = 1;
            if (splitedCmd.Length > 2)
            {
                var itemCnt = splitedCmd[2];
                if (!string.IsNullOrEmpty(itemCnt))
                {
                    if (int.TryParse(itemCnt, out var count))
                    {
                        itemCount = count;
                    }
                }
            }

            if (itemNum < 1)
            {
                await context.Message.AddReactionAsync(Emojis.CrossMark);
                return;
            }

            if (cmdType.Contains("usuń") || cmdType.Contains("usun"))
            {
                await HandleDeleteAsync(itemNum - 1, itemCount, context.Message);
                ResetExpiry();
            }
            else if (cmdType.Contains("dodaj"))
            {
                await HandleAddAsync(itemNum - 1, itemCount, context.Message);
                ResetExpiry();
            }
        }

        private async Task HandleAddAsync(int number, long count, SocketUserMessage message)
        {
            if (number >= _payload.Items.Count)
            {
                await message.AddReactionAsync(Emojis.CrossMark);
                return;
            }

            var thisItem = _payload.Items[number];
            if (thisItem.Count <= count)
            {
                count = thisItem.Count;
                _payload.Items.Remove(thisItem);
            }
            else thisItem.Count -= count;

            var thisItem2 = _payload.PlayerInfo.Items.FirstOrDefault(x => x.Type == thisItem.Type && x.Quality == thisItem.Quality);
            if (thisItem2 == null)
            {
                thisItem2 = thisItem.Type.ToItem(count, thisItem.Quality);
                _payload.PlayerInfo.Items.Add(thisItem2);
            }
            else thisItem2.Count += count;

            await message.AddReactionAsync(Emojis.InboxTray);

            if (await _payload.Message.Channel.GetMessageAsync(_payload.Message.Id) is IUserMessage userMessage)
            {
                await userMessage.ModifyAsync(x => x.Embed = BuildEmbed());
            }
        }

        private async Task HandleDeleteAsync(int number, long count, SocketUserMessage message)
        {
            if (number >= _payload.PlayerInfo.Items.Count)
            {
                await message.AddReactionAsync(Emojis.CrossMark);
                return;
            }

            var thisItem = _payload.PlayerInfo.Items[number];
            if (thisItem.Count <= count)
            {
                count = thisItem.Count;
                _payload.PlayerInfo.Items.Remove(thisItem);
            }
            else thisItem.Count -= count;

            var thisItem2 = _payload.Items.FirstOrDefault(x => x.Type == thisItem.Type
                && x.Quality == thisItem.Quality);

            if (thisItem2 == null)
            {
                thisItem2 = thisItem.Type.ToItem(count, thisItem.Quality);
                _payload.Items.Add(thisItem2);
            }
            else thisItem2.Count += count;

            await message.AddReactionAsync(Emojis.OutboxTray);

            if (await _payload.Message.Channel.GetMessageAsync(_payload.Message.Id) is IUserMessage msg)
            {
                await msg.ModifyAsync(x => x.Embed = BuildEmbed());
            }
        }

        private async Task<bool> HandleReactionAsync(SessionContext context)
        {
            var cacheManager = _serviceProvider.GetRequiredService<ICacheManager>();
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            var waifuService = _serviceProvider.GetRequiredService<dynamic>();
            
            bool end = false;
            if (context.Message.Id != _payload.Message.Id)
            {
                return false;
            }

            if (await _payload.Message.Channel.GetMessageAsync(_payload.Message.Id) is IUserMessage msg)
            {
                var reaction = context.AddReaction ?? context.RemoveReaction;
                if (reaction == null)
                {
                    return false;
                }

                if (reaction.Emote.Equals(Emojis.DeclineEmote))
                {
                    await msg.ModifyAsync(x => x.Embed = $"{_payload.Name}\n\nOdrzucono tworzenie karty.".ToEmbedMessage(EMType.Bot).Build());

                    cacheManager.ExpireTag(new string[] { $"user-{_payload.PlayerInfo.User.Id}", "users" });

                    return true;
                }

                if (reaction.Emote.Equals(Emojis.Checked))
                {
                    bool error = true;

                    if (_payload.PlayerInfo.Accepted)
                    {
                        error = false;

                        var user = await userRepository.GetUserOrCreateAsync(_payload.PlayerInfo.User.Id);
                        var rarity = GetRarityFromValue(GetValue());
                        var characterInfo = await waifuService.GetRandomCharacterAsync();
                        var newCard = waifuService.GenerateNewCard(
                            _payload.PlayerInfo.User,
                            characterInfo,
                            rarity);

                        newCard.Source = CardSource.Crafting;
                        newCard.Affection = user.GameDeck.AffectionFromKarma();

                        foreach (var item in _payload.PlayerInfo.Items)
                        {
                            var thisItem = user.GameDeck.Items
                                .FirstOrDefault(x => x.Type == item.Type
                                    && x.Quality == item.Quality);

                            if (thisItem == null)
                            {
                                error = true;
                                break;
                            }

                            if (thisItem.Count < item.Count)
                            {
                                error = true;
                                break;
                            }
                            thisItem.Count -= item.Count;
                            if (thisItem.Count < 1)
                            {
                                user.GameDeck.Items.Remove(thisItem);
                            }
                        }

                        if (!error)
                        {
                            user.GameDeck.Cards.Add(newCard);

                            await userRepository.SaveChangesAsync();

                            await msg.ModifyAsync(x => x.Embed = $"{_payload.Name}\n\n**Utworzono:** {newCard.GetString(false, false, true)}"
                                .ToEmbedMessage(EMType.Success).Build());
                        }
                    }

                    if (error)
                    {
                        await msg.ModifyAsync(x => x.Embed = $"{_payload.Name}\n\nBrakuje przedmiotów, tworzenie karty nie powiodło się."
                        .ToEmbedMessage(EMType.Bot).Build());
                    }

                    cacheManager.ExpireTag(new string[] { $"user-{_payload.PlayerInfo.User.Id}", "users" });

                    return true;
                }
            }

            return end;
        }

        public Embed BuildEmbed()
        {
            var owned = _payload.Items.ToItemList();
            var used = _payload.PlayerInfo.Items.ToItemList();

            var craftingView = $"**Posiadane:**\n{owned}\n**Użyte:**\n{used}\n**Karta:** {GetCardClassFromItems()}";

            return new EmbedBuilder
            {
                Color = EMType.Bot.Color(),
                Description = $"{_payload.Name}\n\n{craftingView}\n\n{_payload.Tips}"
            }.Build();
        }

        private string GetCardClassFromItems()
        {
            var value = GetValue();
            if (value > 1000)
            {
                _payload.PlayerInfo.Accepted = true;
                return GetRarityFromValue(value).ToString();
            }
            else _payload.PlayerInfo.Accepted = false;

            return "---";
        }

        private long GetValue() => _payload.PlayerInfo.Items.Sum(x => x.Type.CValue() * x.Count);

        private Rarity GetRarityFromValue(long value)
        {
            if (value > 100000) return Rarity.SS;
            if (value > 10000) return Rarity.S;
            if (value > 8000) return Rarity.A;
            if (value > 6000) return Rarity.B;
            if (value > 4000) return Rarity.C;
            if (value > 2000) return Rarity.D;
            return Rarity.E;
        }

        public override async void Dispose()
        {
            if (_payload.Message == null)
            {
                return;
            }

            var message = await _payload.Message.Channel.GetMessageAsync(_payload.Message.Id);

            if (message is IUserMessage userMessage)
            {
                try
                {
                    await userMessage.RemoveAllReactionsAsync();
                }
                catch (Exception) { }
            }

        }
    }
}
