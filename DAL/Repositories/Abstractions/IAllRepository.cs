using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface IAllRepository : ISaveRepository
    {
        Task<IEnumerable<PenaltyInfo>> GetCachedFullPenalties();
        Task<GameDeck> GetCachedUserGameDeckAsync(ulong userId);
        Task<List<User>> GetCachedAllUsersAsync();
        Task<List<GameDeck>> GetCachedPlayersForPVP(ulong ignore = 1);
        Task<Card> GetCardAsync(ulong wid);
        Task<IEnumerable<ulong>> GetUserShindenIdsByHavingCharacterAsync(ulong id);
        Task<List<Card>> GetCardsWithTagAsync(string tag);
        Task<List<Card>> GetCardsByCharacterIdAsync(ulong id);
        Task<User?> GetUserWaifuProfileAsync(ulong id);
    }
}
