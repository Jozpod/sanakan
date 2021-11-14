using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    internal class ToggleCardMessageHandler : IMessageHandler<ToggleCardMessage>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheManager _cacheManager;

        public ToggleCardMessageHandler(
            IUserRepository userRepository,
            ICacheManager cacheManager)
        {
            _userRepository = userRepository;
        }

        public async Task HandleAsync(ToggleCardMessage message)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(message.DiscordUserId);
            var userCard = databaseUser.GameDeck.Cards.FirstOrDefault(x => x.Id == message.WId);
            userCard.Active = !userCard.Active;
            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);
        }
    }
}
