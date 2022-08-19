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
    internal class SpawnCardBundleMessageHandler : BaseMessageHandler<SpawnCardBundleMessage>
    {
        private const int _dailyCardBundleLimit = 3;
        private readonly IUserRepository _userRepository;
        private readonly IUserAnalyticsRepository _userAnalyticsRepository;
        private readonly ISystemClock _systemClock;

        public SpawnCardBundleMessageHandler(
            IUserRepository userRepository,
            IUserAnalyticsRepository userAnalyticsRepository,
            ISystemClock systemClock)
        {
            _userRepository = userRepository;
            _userAnalyticsRepository = userAnalyticsRepository;
            _systemClock = systemClock;
        }

        public override async Task HandleAsync(SpawnCardBundleMessage message)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(message.DiscordUserId);

            if (databaseUser.IsBlacklisted)
            {
                return;
            }

            var statusType = StatusType.Packet;
            var timeStatus = databaseUser.TimeStatuses.FirstOrDefault(x => x.Type == statusType);

            if (timeStatus == null)
            {
                timeStatus = new TimeStatus(statusType);
                databaseUser.TimeStatuses.Add(timeStatus);
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
                Name = BoosterPackTypes.Activity,
                CardSourceFromPack = CardSource.Activity
            };

            databaseUser.GameDeck.BoosterPacks.Add(boosterPack);
            await _userRepository.SaveChangesAsync();

            var content = $"{message.Mention} otrzymał pakiet losowych kart.".ToEmbedMessage(EMType.Bot).Build();

            await message.MessageChannel.SendMessageAsync(embed: content);

            var record = new UserAnalytics
            {
                Value = 1,
                UserId = message.DiscordUserId,
                MeasuredOn = utcNow,
                GuildId = message.GuildId,
                Type = UserAnalyticsEventType.Pack
            };

            _userAnalyticsRepository.Add(record);
            await _userAnalyticsRepository.SaveChangesAsync();
        }
    }
}
