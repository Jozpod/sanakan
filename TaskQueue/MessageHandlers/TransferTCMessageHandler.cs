using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.TaskQueue.Messages;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    internal class TransferTCMessageHandler : BaseMessageHandler<TransferTCMessage>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITransferAnalyticsRepository _transferAnalyticsRepository;
        private readonly ISystemClock _systemClock;
        private readonly ICacheManager _cacheManager;

        public TransferTCMessageHandler(
            IUserRepository userRepository,
            ITransferAnalyticsRepository transferAnalyticsRepository,
            ISystemClock systemClock,
            ICacheManager cacheManager)
        {
            _userRepository = userRepository;
            _transferAnalyticsRepository = transferAnalyticsRepository;
            _systemClock = systemClock;
            _cacheManager = cacheManager;
        }

        public override async Task HandleAsync(TransferTCMessage message)
        {
            var user = await _userRepository.GetByShindenIdAsync(message.ShindenUserId);

            user.TcCount += (long)message.Amount;
            await _userRepository.SaveChangesAsync();
            _cacheManager.ExpireTag(CacheKeys.User(message.DiscordUserId), CacheKeys.Users);

            var transferAnalytics = new TransferAnalytics
            {
                Value = message.Amount,
                DiscordUserId = message.DiscordUserId,
                CreatedOn = _systemClock.UtcNow,
                ShindenUserId = message.ShindenUserId,
                Source = TransferSource.ByShindenId,
            };

            _transferAnalyticsRepository.Add(transferAnalytics);
            await _transferAnalyticsRepository.SaveChangesAsync();
        }
    }
}
