using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
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
        private IIconConfiguration _iconConfiguration = null;
        private IServiceProvider _serviceProvider = null;
        private readonly IUserMessage _userMessage;
        private readonly IList<Item> _items;
        private readonly PlayerInfo _playerInfo;
        private readonly string _name;
        private readonly string _tips;

        public CraftSession(
            ulong ownerId,
            DateTime createdOn,
            IUserMessage userMessage,
            IList<Item> items,
            PlayerInfo playerInfo,
            string name,
            string tips)
            : base(
                ownerId,
                createdOn,
                TimeSpan.FromMinutes(2),
                RunMode.Sync,
                SessionExecuteCondition.AllEvents)
        {
            _userMessage = userMessage;
            _items = items;
            _playerInfo = playerInfo;
            _name = name;
            _tips = tips;
        }

        public override async Task ExecuteAsync(
            SessionContext context,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default)
        {
            IsRunning = true;
            _serviceProvider = serviceProvider;

            _iconConfiguration = _serviceProvider.GetRequiredService<IIconConfiguration>();
            await HandleMessageAsync(context);
            await HandleReactionAsync(context);
            IsRunning = false;
        }

        private async Task HandleMessageAsync(SessionContext context)
        {
            var message = context.Message;

            if (message.Id == _userMessage.Id)
            {
                return;
            }

            if (message.Channel.Id != _userMessage.Channel.Id)
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
                await HandleDeleteAsync(itemNumber - 1, itemCount!);
                ResetExpiry();
            }
            else if (commandType.Contains("dodaj"))
            {
                await HandleAddAsync(itemNumber - 1, itemCount!);
                ResetExpiry();
            }
        }

        private async Task HandleAddAsync(int number, long count)
        {
            if (number >= _items.Count)
            {
                await _userMessage.AddReactionAsync(_iconConfiguration.CrossMark);
                return;
            }

            var firstItem = _items[number];
            if (firstItem.Count <= count)
            {
                count = firstItem.Count;
                _items.Remove(firstItem);
            }
            else
            {
                firstItem.Count -= count;
            }

            var secondItem = _playerInfo.Items
                .FirstOrDefault(x => x.Type == firstItem.Type
                    && x.Quality == firstItem.Quality);

            if (secondItem == null)
            {
                secondItem = firstItem.Type.ToItem(count, firstItem.Quality);
                _playerInfo.Items.Add(secondItem);
            }
            else
            {
                secondItem.Count += count;
            }

            await _userMessage.AddReactionAsync(_iconConfiguration.InboxTray);

            var embed = BuildEmbed();
            await _userMessage.ModifyAsync(x => x.Embed = embed);
        }

        private async Task HandleDeleteAsync(int number, long count)
        {
            var playerItems = _playerInfo.Items;

            if (number >= playerItems.Count)
            {
                await _userMessage.AddReactionAsync(_iconConfiguration.CrossMark);
                return;
            }

            var items = _items;
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

            await _userMessage.AddReactionAsync(Emojis.OutboxTray);

            var embed = BuildEmbed();
            await _userMessage.ModifyAsync(x => x.Embed = embed);
        }

        private async Task HandleReactionAsync(SessionContext context)
        {
            var cacheManager = _serviceProvider.GetRequiredService<ICacheManager>();
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            var waifuService = _serviceProvider.GetRequiredService<IWaifuService>();

            if (context.Message.Id != _userMessage.Id)
            {
                return;
            }

            var reaction = context.AddReaction ?? context.RemoveReaction;

            if (reaction == null)
            {
                return;
            }

            var emote = reaction.Emote;
            var discordUserId = _playerInfo.DiscordId;

            if (emote.Equals(_iconConfiguration.Decline))
            {
                await _userMessage.ModifyAsync(x => x.Embed = $"{_name}\n\nOdrzucono tworzenie karty."
                    .ToEmbedMessage(EMType.Bot).Build());

                cacheManager.ExpireTag(CacheKeys.User(discordUserId), CacheKeys.Users);

                return;
            }

            if (!emote.Equals(_iconConfiguration.Accept))
            {
                return;
            }

            bool error = true;

            if (!_playerInfo.Accepted)
            {
                return;
            }

            error = false;

            var user = await userRepository.GetUserOrCreateAsync(discordUserId);
            var totalCValue = _playerInfo.Items.Sum(x => x.Type.CValue() * x.Count);
            var rarity = RarityExtensions.GetRarityFromValue(totalCValue);
            var characterInfo = await waifuService.GetRandomCharacterAsync();
            var newCard = waifuService.GenerateNewCard(
                discordUserId,
                characterInfo!,
                rarity);

            var gameDeck = user.GameDeck;
            newCard.Source = CardSource.Crafting;
            newCard.Affection = gameDeck.AffectionFromKarma();
            var items = gameDeck.Items;

            foreach (var item in _playerInfo.Items)
            {
                var thisItem = items
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
                    items.Remove(thisItem);
                }
            }

            if (error)
            {
                var embed = $"{_name}\n\nBrakuje przedmiotów, tworzenie karty nie powiodło się."
                    .ToEmbedMessage(EMType.Bot)
                    .Build();
                await _userMessage.ModifyAsync(x => x.Embed = embed);
            }
            else
            {
                gameDeck.Cards.Add(newCard);

                await userRepository.SaveChangesAsync();

                var cardSummary = newCard.GetString(false, false, true);

                var embed = $"{_name}\n\n**Utworzono:** {cardSummary}"
                    .ToEmbedMessage(EMType.Success).Build();
                await _userMessage.ModifyAsync(x => x.Embed = embed);
            }

            cacheManager.ExpireTag(CacheKeys.User(_playerInfo.DiscordId), CacheKeys.Users);
        }

        public Embed BuildEmbed()
        {
            var owned = _items.ToItemList();
            var usedItems = _playerInfo.Items;
            var used = usedItems.ToItemList();

            var summary = "---";
            var value = usedItems.Sum(x => x.Type.CValue() * x.Count);

            if (value > 1000)
            {
                _playerInfo.Accepted = true;
                summary = RarityExtensions.GetRarityFromValue(value).ToString();
            }
            else
            {
                _playerInfo.Accepted = false;
            }

            var craftingView = $"**Posiadane:**\n{owned}\n**Użyte:**\n{used}\n**Karta:** {summary}";

            return new EmbedBuilder
            {
                Color = EMType.Bot.Color(),
                Description = $"{_name}\n\n{craftingView}\n\n{_tips}"
            }.Build();
        }

        public override async ValueTask DisposeAsync()
        {
            try
            {
                await _userMessage.RemoveAllReactionsAsync();
            }
            catch (Exception) { }

            _serviceProvider = null;
        }
    }
}