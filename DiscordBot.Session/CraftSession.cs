using Discord;
using Discord.Commands;
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
        private IServiceProvider _serviceProvider { get; set; }

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
            CraftSessionPayload payload)
            : base(
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
            IsRunning = true;
            _serviceProvider = serviceProvider;

            if (_payload.PlayerInfo == null || _payload.Message == null)
            {
                return;
            }

            await HandleMessageAsync(context);
            await HandleReactionAsync(context);
            IsRunning = false;
        }

        private async Task HandleMessageAsync(SessionContext context)
        {
            var message = context.Message;

            if (message.Id == _payload.Message.Id)
            {
                return;
            }

            if (message.Channel.Id != _payload.Message.Channel.Id)
            {
                return;
            }

            var command = message?.Content?.ToLower();
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

            var itemNumber = 0;
            var itemNr = splitedCommand[1];
            if (!string.IsNullOrEmpty(itemNr))
            {
                int.TryParse(itemNr, out itemNumber);
            }

            var itemCount = 1;
            if (splitedCommand.Length > 2)
            {
                var newItemCount = splitedCommand[2];
                if (!string.IsNullOrEmpty(newItemCount))
                {
                    if (int.TryParse(newItemCount, out var count))
                    {
                        itemCount = count;
                    }
                }
            }

            if (itemNumber < 1)
            {
                await message.AddReactionAsync(Emojis.CrossMark);
                return;
            }

            if (commandType.Contains("usuń") || commandType.Contains("usun"))
            {
                await HandleDeleteAsync(itemNumber - 1, itemCount, message);
                ResetExpiry();
            }
            else if (commandType.Contains("dodaj"))
            {
                await HandleAddAsync(itemNumber - 1, itemCount, message);
                ResetExpiry();
            }
        }

        private async Task HandleAddAsync(int number, long count, IUserMessage message)
        {
            if (number >= _payload.Items.Count)
            {
                await message.AddReactionAsync(Emojis.CrossMark);
                return;
            }

            var firstItem = _payload.Items[number];
            if (firstItem.Count <= count)
            {
                count = firstItem.Count;
                _payload.Items.Remove(firstItem);
            }
            else
            {
                firstItem.Count -= count;
            }

            var secondItem = _payload.PlayerInfo.Items
                .FirstOrDefault(x => x.Type == firstItem.Type
                    && x.Quality == firstItem.Quality);

            if (secondItem == null)
            {
                secondItem = firstItem.Type.ToItem(count, firstItem.Quality);
                _payload.PlayerInfo.Items.Add(secondItem);
            }
            else
            {
                secondItem.Count += count;
            }

            await message.AddReactionAsync(Emojis.InboxTray);

            if (await _payload.Message.Channel.GetMessageAsync(_payload.Message.Id) is IUserMessage userMessage)
            {
                await userMessage.ModifyAsync(x => x.Embed = BuildEmbed());
            }
        }

        private async Task HandleDeleteAsync(int number, long count, IUserMessage message)
        {
            var playerItems = _payload.PlayerInfo.Items;

            if (number >= playerItems.Count)
            {
                await message.AddReactionAsync(Emojis.CrossMark);
                return;
            }

            var items = _payload.Items;
            var firstItem = playerItems[number];
            if (firstItem.Count <= count)
            {
                count = firstItem.Count;
                playerItems.Remove(firstItem);
            }
            else firstItem.Count -= count;

            var secondItem = items.FirstOrDefault(x => x.Type == firstItem.Type
                && x.Quality == firstItem.Quality);

            if (secondItem == null)
            {
                secondItem = firstItem.Type.ToItem(count, firstItem.Quality);
                items.Add(secondItem);
            }
            else
            {
                secondItem.Count += count;
            }

            await message.AddReactionAsync(Emojis.OutboxTray);

            if (await _payload.Message.Channel.GetMessageAsync(_payload.Message.Id) is IUserMessage userMessage)
            {
                await userMessage.ModifyAsync(x => x.Embed = BuildEmbed());
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

            var discordUserId = _payload.PlayerInfo.DiscordId;

            if (reaction.Emote.Equals(Emojis.DeclineEmote))
            {
                await userMessage.ModifyAsync(x => x.Embed = $"{_payload.Name}\n\nOdrzucono tworzenie karty."
                    .ToEmbedMessage(EMType.Bot).Build());

                cacheManager.ExpireTag(CacheKeys.User(discordUserId), CacheKeys.Users);

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

            var user = await userRepository.GetUserOrCreateAsync(discordUserId);
            var totalCValue = _payload.PlayerInfo.Items.Sum(x => x.Type.CValue() * x.Count);
            var rarity = RarityExtensions.GetRarityFromValue(totalCValue);
            var characterInfo = await waifuService.GetRandomCharacterAsync();
            var newCard = waifuService.GenerateNewCard(
                discordUserId,
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

            cacheManager.ExpireTag(CacheKeys.User(_payload.PlayerInfo.DiscordId), CacheKeys.Users);
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

        public override async ValueTask DisposeAsync()
        {
            if (_payload.Message == null)
            {
                return;
            }

            var userMessage = await _payload.Message.Channel.GetMessageAsync(_payload.Message.Id) as IUserMessage;

            if (userMessage == null)
            {
                return;             
            }

            try
            {
                await userMessage.RemoveAllReactionsAsync();
            }
            catch (Exception) { }

            _serviceProvider = null;
        }
    }
}
