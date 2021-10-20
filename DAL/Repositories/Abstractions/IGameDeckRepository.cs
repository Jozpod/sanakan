using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface IGameDeckRepository : ICreateRepository<GameDeck>, ISaveRepository
    {
        Task<List<GameDeck>> GetByCardIdAndCharacterAsync(ulong cardId, ulong character);
        Task<GameDeck> GetByCardIdAsync(ulong id);
        Task<List<GameDeck>> GetByAnimeIdAsync(ulong id);
    }
}
