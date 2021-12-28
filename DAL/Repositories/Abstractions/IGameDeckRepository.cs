using Sanakan.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface IGameDeckRepository : ICreateRepository<GameDeck>, ISaveRepository
    {
        Task<List<GameDeck>> GetCachedPlayersForPVP(ulong discordUserId, double minDeckPower, double maxDeckPower);

        Task<GameDeck?> GetCachedUserGameDeckAsync(ulong userId);

        Task<List<GameDeck>> GetByCardIdAndCharacterAsync(ulong cardId, ulong characterId);

        Task<List<GameDeck>> GetByAnimeIdAsync(ulong animeId);
    }
}
