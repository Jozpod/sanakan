using Sanakan.Common.Cache;
using Sanakan.DAL;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.TaskQueue.Messages;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    internal class DeleteUserMessageHandler : BaseMessageHandler<DeleteUserMessage>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheManager _cacheManager;

        public DeleteUserMessageHandler(
            IUserRepository userRepository,
            ICacheManager cacheManager)
        {
            _userRepository = userRepository;
            _cacheManager = cacheManager;
        }

        public override async Task HandleAsync(DeleteUserMessage message)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(message.DiscordUserId);
            var fakeUser = await _userRepository.GetUserOrCreateAsync(Constants.RootUserId);

            foreach (var card in databaseUser.GameDeck.Cards)
            {
                card.InCage = false;
                card.Tags.Clear();
                card.LastOwnerId = databaseUser.Id;
                card.GameDeckId = fakeUser.GameDeck.Id;
            }

            _userRepository.Remove(databaseUser);

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.Users);
        }
    }
}
