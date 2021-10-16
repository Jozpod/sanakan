using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using System.Collections.Generic;
using System.Linq;
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
        Task<Card> GetCardAsync(ulong wid);
        Task<IEnumerable<ulong>> GetUserShindenIdsByHavingCharacterAsync(ulong id);
        Task AddQuestionAsync(Question question);
        Task RemoveQuestionAsync(Question question);
        Task<Question> GetQuestionAsync(ulong id);
        Task<User?> GetUserCardsAsync(ulong id);
        Task AddSystemAnalyticsAsync(SystemAnalytics record);
        Task<User> GetUsersCardsByShindenIdWithOffsetAndFilterAsync1(ulong id);
        Task<IQueryable<Card>> GetUsersCardsByShindenIdWithOffsetAndFilterAsync2(ulong id);
        Task<List<Card>> GetCardsWithTagAsync(string tag);
        Task<List<Card>> GetCardsByCharacterIdAsync(ulong id);
        Task SaveChangesAsync();
        Task<User?> GetUserWaifuProfileAsync(ulong id);
        Task AddTransferAnalyticsAsync(TransferAnalytics record);
    }
}
