using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Abstractions
{
    public interface IRepository
    {
        Task<GuildOptions> GetGuildConfigOrCreateAsync(ulong guildId);
        Task<GuildOptions> GetCachedGuildFullConfigAsync(ulong guildId);
        Task<IEnumerable<PenaltyInfo>> GetCachedFullPenalties();
        Task<GameDeck> GetCachedUserGameDeckAsync(ulong userId);
        Task<List<User>> GetCachedAllUsersAsync();
        Task<List<GameDeck>> GetCachedPlayersForPVP(ulong ignore = 1);
        Task<User> GetUserOrCreateAsync(ulong userId);
        Task<User> GetBaseUserAndDontTrackAsync(ulong userId);
        Task<User> GetUserAndDontTrackAsync(ulong userId);
        Task<List<Question>> GetCachedAllQuestionsAsync();
        Task<Question> GetCachedQuestionAsync(ulong id);
    }
}
