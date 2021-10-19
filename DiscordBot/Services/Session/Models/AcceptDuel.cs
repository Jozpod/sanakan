using DAL.Repositories.Abstractions;
using Discord;
using DiscordBot.Services.PocketWaifu.Abstractions;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Config;
using Sanakan.DAL.Models;
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
        private readonly IAllRepository _repository;
        private readonly ICacheManager _cacheManager;

        public AcceptDuel(
            IWaifuService waifu,
            IAllRepository repository,
            ICacheManager cacheManager)
        {
            _waifu = waifu;
            _repository = repository;
            _cacheManager = cacheManager;
        }

        public async Task<bool> OnAccept(SessionContext context)
        {
            var players = new List<PlayerInfo> { P1, P2 };

            var fight = _waifu.MakeFightAsync(players);
            string deathLog = _waifu.GetDeathLog(fight, players);

            bool isWinner = fight.Winner != null;
            string winString = isWinner ? $"Zwycięża {fight.Winner.User.Mention}!": "Remis!";

            if (await Message.Channel.GetMessageAsync(Message.Id) is IUserMessage msg)
            {
                await msg.ModifyAsync(x => x.Embed = $"{DuelName}{deathLog.TrimToLength(1400)}{winString}".ToEmbedMessage(EMType.Error).Build());
            }

            var user1 = await _repository.GetUserOrCreateAsync(P1.User.Id);
            var user2 = await _repository.GetUserOrCreateAsync(P2.User.Id);

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

            await _repository.SaveChangesAsync();

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
