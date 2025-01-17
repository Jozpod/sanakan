﻿using Discord;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.Extensions;
using Sanakan.Game.Extensions;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    internal class LotteryMessageHandler : BaseMessageHandler<LotteryMessage>
    {
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IUserRepository _userRepository;
        private readonly ICacheManager _cacheManager;

        public LotteryMessageHandler(
            IRandomNumberGenerator randomNumberGenerator,
            IUserRepository userRepository,
            ICacheManager cacheManager)
        {
            _randomNumberGenerator = randomNumberGenerator;
            _userRepository = userRepository;
            _cacheManager = cacheManager;
        }

        public override async Task HandleAsync(LotteryMessage message)
        {
            var userMessage = message.UserMessage;
            var user = await _userRepository.GetUserOrCreateAsync(message.DiscordUserId);
            Embed embed;

            if (user == null)
            {
                embed = "Nie odnaleziono kart do rozdania!".ToEmbedMessage(EMType.Error).Build();
                await userMessage.ModifyAsync(x => x.Embed = embed);
                return;
            }

            var loteryCards = user.GameDeck.Cards;

            if (!loteryCards.Any())
            {
                embed = "Nie odnaleziono kart do rozdania!".ToEmbedMessage(EMType.Error).Build();
                await userMessage.ModifyAsync(x => x.Embed = embed);
                return;
            }

            var winnerUser = await _userRepository.GetUserOrCreateAsync(message.WinnerUserId);

            if (winnerUser == null)
            {
                embed = "Nie odnaleziono docelowego użytkownika!".ToEmbedMessage(EMType.Error).Build();
                await userMessage.ModifyAsync(x => x.Embed = embed);
                return;
            }

            var cardsIds = new List<string>();
            var idsToSelect = loteryCards.Select(x => x.Id).ToList();

            for (var index = 0; index < message.CardCount; index++)
            {
                if (!idsToSelect.Any())
                {
                    break;
                }

                var waifuId = _randomNumberGenerator.GetOneRandomFrom(idsToSelect);
                var randomCard = loteryCards.FirstOrDefault(x => x.Id == waifuId);

                cardsIds.Add(randomCard!.GetString(false, false, true));

                randomCard.Active = false;
                randomCard.InCage = false;
                randomCard.Tags.Clear();
                randomCard.Expedition = ExpeditionCardType.None;

                randomCard.GameDeckId = winnerUser.GameDeck.Id;

                winnerUser.GameDeck.RemoveCharacterFromWishList(randomCard.CharacterId);
                winnerUser.GameDeck.RemoveCardFromWishList(randomCard.Id);

                idsToSelect.Remove(waifuId);
            }

            await _userRepository.SaveChangesAsync();
            await userMessage.DeleteAsync();

            _cacheManager.ExpireTag(CacheKeys.User(message.DiscordUserId), CacheKeys.Users, CacheKeys.User(message.InvokingUserId));

            embed = $"Loterie wygrywa {message.WinnerUser.Mention}.\nOtrzymuje: {string.Join("\n", cardsIds)}"
                .ElipseTrimToLength(2000)
                .ToEmbedMessage(EMType.Success).Build();

            userMessage = await message.Channel.SendMessageAsync(embed: embed);

            var jumpUrl = userMessage.GetJumpUrl();

            try
            {
                var privateEmbed = new EmbedBuilder()
                {
                    Color = EMType.Info.Color(),
                    Description = $"Na [loterii]({jumpUrl}) zdobyłeś {cardsIds.Count} kart."
                };

                var dmChannel = await message.WinnerUser.CreateDMChannelAsync();

                if (dmChannel != null)
                {
                    await dmChannel.SendMessageAsync(embed: privateEmbed.Build());
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
