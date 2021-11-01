using Sanakan.Common;
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
            var botUser = await _userRepository.GetUserOrCreateAsync(message.DiscordUserId);
            var thisCard = botUser.GameDeck.Cards.FirstOrDefault(x => x.Id == message.WId);
            thisCard.Active = !thisCard.Active;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{botUser.Id}", "users" });

            //var exe = new Executable($"api-deck u{discordId}", new Task<Task>(async () =>
            //{

            //}));
        }
    }
}
