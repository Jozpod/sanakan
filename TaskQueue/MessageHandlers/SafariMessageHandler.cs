using Discord;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.Extensions;
using Sanakan.Game.Extensions;
using Sanakan.Game.Services.Abstractions;
using Sanakan.TaskQueue.Messages;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    internal class SafariMessageHandler : IMessageHandler<SafariMessage>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserAnalyticsRepository _userAnalyticsRepository;
        private readonly ISystemClock _systemClock;
        private readonly ICacheManager _cacheManager;
        private readonly IWaifuService _waifuService;

        public SafariMessageHandler(
            IUserRepository userRepository,
            IUserAnalyticsRepository userAnalyticsRepository,
            ISystemClock systemClock,
            ICacheManager cacheManager,
            IWaifuService waifuService)
        {
            _userRepository = userRepository;
            _userAnalyticsRepository = userAnalyticsRepository;
            _systemClock = systemClock;
            _cacheManager = cacheManager;
            _waifuService = waifuService;
        }

        public async Task HandleAsync(SafariMessage safariMessage)
        {
            var embed = safariMessage.Embed;
            var winner = safariMessage.Winner;
            var card = safariMessage.Card;
            var guildId = safariMessage.GuildId;
            var trashChannel = safariMessage.TrashChannel;
            var pokeImage = safariMessage.Image;
            var message = safariMessage.Message;
            var discordUserId = winner.Id;
            var botUser = await _userRepository.GetUserOrCreateAsync(discordUserId);

            card.FirstOwnerId = discordUserId;
            card.Affection += botUser.GameDeck.AffectionFromKarma();
            botUser.GameDeck.RemoveCharacterFromWishList(card.CharacterId);

            botUser.GameDeck.Cards.Add(card);
            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(botUser.Id), CacheKeys.Users);

            var record = new UserAnalytics
            {
                Value = 1,
                UserId = discordUserId,
                MeasuredOn = _systemClock.UtcNow,
                GuildId = guildId,
                Type = UserAnalyticsEventType.Card
            };

            _userAnalyticsRepository.Add(record);
            await _userAnalyticsRepository.SaveChangesAsync();

            var cardDescription = card.GetString(false, false, true);
            embed.ImageUrl = await _waifuService.GetSafariViewAsync(pokeImage, card, trashChannel);
            embed.Description = $"{winner.Mention} zdobył na polowaniu i wsadził do klatki:\n{cardDescription}\n({card.Title})";
            await message.ModifyAsync(x => x.Embed = embed.Build());

            var privateEmbed = new EmbedBuilder()
            {
                Color = EMType.Info.Color(),
                Description = $"Na [polowaniu]({message.GetJumpUrl()}) zdobyłeś: {cardDescription}"
            };

            var dmChannel = await winner.GetOrCreateDMChannelAsync();
            if (dmChannel != null)
            {
                await dmChannel.SendMessageAsync("", false, privateEmbed.Build());
            }
        }
    }
}
