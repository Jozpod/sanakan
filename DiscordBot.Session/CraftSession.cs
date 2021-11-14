using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.Extensions;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.Services.PocketWaifu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session
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

            var command = context.Message?.Content?.ToLower();
            if (command == null)
            {
                return;
            }

            var splitedCmd = command.Replace("\n", " ").Split(" ");
            if (splitedCmd.Length < 2)
            {
                return;
            }

            var commandType = splitedCmd[0];
            if (commandType == null)
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

            if (commandType.Contains("usuń") || commandType.Contains("usun"))
            {
                await HandleDeleteAsync(itemNum - 1, itemCount, context.Message);
                ResetExpiry();
            }
            else if (commandType.Contains("dodaj"))
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

        private async Task HandleReactionAsync(SessionContext context)
        {
            var cacheManager = _serviceProvider.GetRequiredService<ICacheManager>();
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            var waifuService = _serviceProvider.GetRequiredService<IWaifuService>();
            
            if (context.Message.Id != _payload.Message.Id)
            {
                return;
            }

            var userMessage = await _payload.Message.Channel.GetMessageAsync(_payload.Message.Id) as IUserMessage;

            if (userMessage == null)
            {
                return;
            }

            var reaction = context.AddReaction ?? context.RemoveReaction;
            
            if (reaction == null)
            {
                return;
            }

            if (reaction.Emote.Equals(Emojis.DeclineEmote))
            {
                await userMessage.ModifyAsync(x => x.Embed = $"{_payload.Name}\n\nOdrzucono tworzenie karty."
                    .ToEmbedMessage(EMType.Bot).Build());

                cacheManager.ExpireTag(CacheKeys.User(_payload.PlayerInfo.User.Id), CacheKeys.Users);

                return;
            }

            if (!reaction.Emote.Equals(Emojis.Checked))
            {
                return;
            }

            bool error = true;

            if (!_payload.PlayerInfo.Accepted)
            {
                return;
            }

            error = false;

            var user = await userRepository.GetUserOrCreateAsync(_payload.PlayerInfo.User.Id);
            var totalCValue = _payload.PlayerInfo.Items.Sum(x => x.Type.CValue() * x.Count);
            var rarity = RarityExtensions.GetRarityFromValue(totalCValue);
            var characterInfo = await waifuService.GetRandomCharacterAsync();
            var newCard = waifuService.GenerateNewCard(
                _payload.PlayerInfo.User,
                characterInfo,
                rarity);

            var gameDeck = user.GameDeck;
            newCard.Source = CardSource.Crafting;
            newCard.Affection = gameDeck.AffectionFromKarma();

            foreach (var item in _payload.PlayerInfo.Items)
            {
                var thisItem = gameDeck.Items
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
                    gameDeck.Items.Remove(thisItem);
                }
            }

            if (!error)
            {
                await userMessage.ModifyAsync(x => x.Embed = $"{_payload.Name}\n\nBrakuje przedmiotów, tworzenie karty nie powiodło się."
                    .ToEmbedMessage(EMType.Bot).Build());
            }
            else
            {
                gameDeck.Cards.Add(newCard);

                await userRepository.SaveChangesAsync();

                var cardSummary = newCard.GetString(false, false, true);

                await userMessage.ModifyAsync(x => x.Embed = $"{_payload.Name}\n\n**Utworzono:** {cardSummary}"
                    .ToEmbedMessage(EMType.Success).Build());
            }

            cacheManager.ExpireTag(CacheKeys.User(_payload.PlayerInfo.User.Id), CacheKeys.Users);
        }

        public Embed BuildEmbed()
        {
            var owned = _payload.Items.ToItemList();
            var used = _payload.PlayerInfo.Items.ToItemList();

            var summary = "---";
            var value = _payload.PlayerInfo.Items.Sum(x => x.Type.CValue() * x.Count);
            if (value > 1000)
            {
                _payload.PlayerInfo.Accepted = true;
                summary = RarityExtensions.GetRarityFromValue(value).ToString();
            }
            else
            {
                _payload.PlayerInfo.Accepted = false;
            }

            var craftingView = $"**Posiadane:**\n{owned}\n**Użyte:**\n{used}\n**Karta:** {summary}";

            return new EmbedBuilder
            {
                Color = EMType.Bot.Color(),
                Description = $"{_payload.Name}\n\n{craftingView}\n\n{_payload.Tips}"
            }.Build();
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
