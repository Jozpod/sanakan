using Discord;
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
    internal class LotteryMessageHandler : IMessageHandler<LotteryMessage>
    {
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly ISystemClock _systemClock;
        private readonly ICacheManager _cacheManager;

        public LotteryMessageHandler(
            IUserRepository userRepository,
            IGuildConfigRepository guildConfigRepository,
            ISystemClock systemClock,
            ICacheManager cacheManager)
        {
            _userRepository = userRepository;
            _systemClock = systemClock;
            _guildConfigRepository = guildConfigRepository;
        }

        public async Task HandleAsync(LotteryMessage message)
        {
            var userMessage = message.UserMessage;
            var user = await _userRepository.GetUserOrCreateAsync(message.DiscordUserId);
            if (user == null)
            {
                await userMessage.ModifyAsync(x => x.Embed = "Nie odnaleziono kart do rozdania!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var loteryCards = user.GameDeck.Cards.ToList();
            if (loteryCards.Count < 1)
            {
                await userMessage.ModifyAsync(x => x.Embed = "Nie odnaleziono kart do rozdania!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var winnerUser = await _userRepository.GetUserOrCreateAsync(message.WinnerUserId);

            if (winnerUser == null)
            {
                await userMessage.ModifyAsync(x => x.Embed = "Nie odnaleziono docelowego użytkownika!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var cardsIds = new List<string>();
            var idsToSelect = loteryCards.Select(x => x.Id).ToList();

            for (int i = 0; i < message.CardCount; i++)
            {
                if (idsToSelect.Count < 1)
                    break;

                var wid = _randomNumberGenerator.GetOneRandomFrom(idsToSelect);
                var thisCard = loteryCards.FirstOrDefault(x => x.Id == wid);

                cardsIds.Add(thisCard.GetString(false, false, true));

                thisCard.Active = false;
                thisCard.InCage = false;
                thisCard.TagList.Clear();
                thisCard.Expedition = ExpeditionCardType.None;

                thisCard.GameDeckId = winnerUser.GameDeck.Id;

                winnerUser.GameDeck.RemoveCharacterFromWishList(thisCard.CharacterId);
                winnerUser.GameDeck.RemoveCardFromWishList(thisCard.Id);

                idsToSelect.Remove(wid);
            }

            await _guildConfigRepository.SaveChangesAsync();
            await userMessage.DeleteAsync();

            var discordUserkey = string.Format(CacheKeys.User, message.DiscordUserId);
            var invokingUserkey = string.Format(CacheKeys.User, message.InvokingUserId);
            _cacheManager.ExpireTag(discordUserkey, CacheKeys.Users, invokingUserkey); 

            var content = $"Loterie wygrywa {message.WinnerUser.Mention}.\nOtrzymuje: {string.Join("\n", cardsIds)}"
                .ElipseTrimToLength(2000)
                .ToEmbedMessage(EMType.Success).Build();

            userMessage = await message.Channel.SendMessageAsync(embed: content);

            var jumpUrl = userMessage.GetJumpUrl();

            try
            {
                var privEmb = new EmbedBuilder()
                {
                    Color = EMType.Info.Color(),
                    Description = $"Na [loterii]({jumpUrl}) zdobyłeś {cardsIds.Count} kart."
                };

                var dmChannel = await message.WinnerUser.GetOrCreateDMChannelAsync();

                if (dmChannel != null)
                {
                    await dmChannel.SendMessageAsync("", embed: privEmb.Build());
                }
            }
            catch (Exception) { }
        }
    }
}
