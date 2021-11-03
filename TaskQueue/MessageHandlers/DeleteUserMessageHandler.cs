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
    internal class DeleteUserMessageHandler : IMessageHandler<DeleteUserMessage>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheManager _cacheManager;

        public DeleteUserMessageHandler()
        {

        }

        public async Task HandleAsync(DeleteUserMessage message)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(message.DiscordUserId);
            var fakeUser = await _userRepository.GetUserOrCreateAsync(1);

            foreach (var card in databaseUser.GameDeck.Cards)
            {
                card.InCage = false;
                card.TagList.Clear();
                card.LastOwnerId = databaseUser.Id;
                card.GameDeckId = fakeUser.GameDeck.Id;
            }

            _userRepository.Remove(databaseUser);

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.Users);
        }
    }
}
