using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web.MessageHandlers
{
    public class SpawnCardBundleMessageHandler : IMessageHandler<SpawnCardBundleMessage>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserAnalyticsRepository _userAnalyticsRepository;
        private readonly ISystemClock _systemClock;
        private readonly ICacheManager _cacheManager;

        public SpawnCardBundleMessageHandler(
            IUserRepository userRepository,
            IUserAnalyticsRepository userAnalyticsRepository,
            ISystemClock systemClock,
            ICacheManager cacheManager)
        {
            _userRepository = userRepository;
            _userAnalyticsRepository = userAnalyticsRepository;
            _systemClock = systemClock;
            _userAnalyticsRepository = userAnalyticsRepository;
        }

        public async Task HandleAsync(SpawnCardBundleMessage message)
        {
            var botUser = await _userRepository.GetUserOrCreateAsync(message.DiscordUserId);

            if (botUser.IsBlacklisted)
            {
                return;
            }

            var pCnt = botUser.TimeStatuses.FirstOrDefault(x => x.Type == StatusType.Packet);

            if (pCnt == null)
            {
                pCnt = StatusType.Packet.NewTimeStatus();
                botUser.TimeStatuses.Add(pCnt);
            }

            if (!pCnt.IsActive())
            {
                pCnt.EndsAt = _systemClock.UtcNow.Date.AddDays(1);
                pCnt.IValue = 0;
            }

            if (++pCnt.IValue > 3)
            {
                return;
            }

            var boosterPack = new BoosterPack
            {
                CardCount = 2,
                MinRarity = Rarity.E,
                IsCardFromPackTradable = true,
                Name = "Pakiet kart za aktywność",
                CardSourceFromPack = CardSource.Activity
            };

            botUser.GameDeck.BoosterPacks.Add(boosterPack);
            await _userRepository.SaveChangesAsync();

            await message.MessageChannel
                .SendMessageAsync("", embed: $"{user.Mention} otrzymał pakiet losowych kart.".ToEmbedMessage(EMType.Bot).Build());

            var gUser = user as SocketGuildUser;

            var record = new UserAnalytics
            {
                Value = 1,
                UserId = message.DiscordUserId,
                MeasureDate = _systemClock.UtcNow,
                GuildId = gUser?.Guild?.Id ?? 0,
                Type = UserAnalyticsEventType.Pack
            };

            _userAnalyticsRepository.Add(record);
            await _userAnalyticsRepository.SaveChangesAsync();
        }
    }
}
