using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.TaskQueue.Messages;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    internal class SpawnCardBundleMessageHandler : IMessageHandler<SpawnCardBundleMessage>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserAnalyticsRepository _userAnalyticsRepository;
        private readonly ISystemClock _systemClock;
        private const int _dailyCardBundleLimit = 3;

        public SpawnCardBundleMessageHandler(
            IUserRepository userRepository,
            IUserAnalyticsRepository userAnalyticsRepository,
            ISystemClock systemClock)
        {
            _userRepository = userRepository;
            _userAnalyticsRepository = userAnalyticsRepository;
            _systemClock = systemClock;
        }

        public async Task HandleAsync(SpawnCardBundleMessage message)
        {
            var botUser = await _userRepository.GetUserOrCreateAsync(message.DiscordUserId);

            if (botUser.IsBlacklisted)
            {
                return;
            }

            var statusType = StatusType.Packet;
            var timeStatus = botUser.TimeStatuses.FirstOrDefault(x => x.Type == statusType);

            if (timeStatus == null)
            {
                timeStatus = new TimeStatus(statusType);
                botUser.TimeStatuses.Add(timeStatus);
            }

            var utcNow = _systemClock.UtcNow;

            if (!timeStatus.IsActive(utcNow))
            {
                timeStatus.EndsOn = utcNow.Date.AddDays(1);
                timeStatus.IntegerValue = 0;
            }

            if (++timeStatus.IntegerValue > _dailyCardBundleLimit)
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

            var content = $"{message.Mention} otrzymał pakiet losowych kart.".ToEmbedMessage(EMType.Bot).Build();

            await message.MessageChannel
                .SendMessageAsync("", embed: content);

            var record = new UserAnalytics
            {
                Value = 1,
                UserId = message.DiscordUserId,
                MeasuredOn = _systemClock.UtcNow,
                GuildId = message.GuildId ?? 0,
                Type = UserAnalyticsEventType.Pack
            };

            _userAnalyticsRepository.Add(record);
            await _userAnalyticsRepository.SaveChangesAsync();
        }
    }
}
