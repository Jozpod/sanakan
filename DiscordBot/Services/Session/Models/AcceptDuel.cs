using Discord;
using DiscordBot.Services.PocketWaifu.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Extensions;
using Sanakan.Services.PocketWaifu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Services.Session.Models
{
    public class AcceptDuel : IAcceptActions
    {
        public IMessage Message { get; set; }
        public string DuelName { get; set; }
        public PlayerInfo P1 { get; set; }
        public PlayerInfo P2 { get; set; }

        private IWaifuService _waifu;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ICacheManager _cacheManager;

        public AcceptDuel(
            IWaifuService waifu,
            IServiceScopeFactory serviceScopeFactory,
            ICacheManager cacheManager)
        {
            _waifu = waifu;
            _serviceScopeFactory = serviceScopeFactory;
            _cacheManager = cacheManager;
        }

        public async Task<bool> OnAccept(SessionContext context)
        {
            var players = new List<PlayerInfo> { P1, P2 };

            var fight = _waifu.MakeFightAsync(players);
            var deathLog = _waifu.GetDeathLog(fight, players);

            var isWinner = fight.Winner != null;
            string winString = isWinner ? $"Zwycięża {fight.Winner.User.Mention}!": "Remis!";

            if (await Message.Channel.GetMessageAsync(Message.Id) is IUserMessage msg)
            {
                await msg.ModifyAsync(x => x.Embed = $"{DuelName}{deathLog.ElipseTrimToLength(1400)}{winString}".ToEmbedMessage(EMType.Error).Build());
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();

            var user1 = await userRepository.GetUserOrCreateAsync(P1.User.Id);
            var user2 = await userRepository.GetUserOrCreateAsync(P2.User.Id);

            user1.GameDeck.PvPStats.Add(new CardPvPStats
            {
                Type = FightType.Versus,
                Result = isWinner ? (fight.Winner.User.Id == user1.Id ? FightResult.Win : FightResult.Lose) : FightResult.Draw
            });

            user2.GameDeck.PvPStats.Add(new CardPvPStats
            {
                Type = FightType.Versus,
                Result = isWinner ? (fight.Winner.User.Id == user2.Id ? FightResult.Win : FightResult.Lose) : FightResult.Draw
            });

            await userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{user1.Id}", $"user-{user2.Id}", "users" });

            Dispose();
            return true;
        }

        public async Task<bool> OnDecline(SessionContext context)
        {
            if (await Message.Channel.GetMessageAsync(Message.Id) is IUserMessage msg)
            {
                await msg.ModifyAsync(x => x.Embed = $"{DuelName}{context.User.Mention} odrzucił wyzwanie!".ToEmbedMessage(EMType.Error).Build());
            }

            Dispose();
            return true;
        }

        private void Dispose()
        {
            P1 = null;
            P2 = null;
            DuelName = null;
        }
    }
}
