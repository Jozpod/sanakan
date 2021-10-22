using Sanakan.DAL.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface IGameDeckRepository : ICreateRepository<GameDeck>, ISaveRepository
    {
        Task<List<GameDeck>> GetCachedPlayersForPVP(ulong ignore = 1);
        Task<GameDeck> GetCachedUserGameDeckAsync(ulong userId);
        Task<List<GameDeck>> GetByCardIdAndCharacterAsync(ulong cardId, ulong character);
        Task<List<GameDeck>> GetByAnimeIdAsync(ulong id);
    }
}
