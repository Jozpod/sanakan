using Sanakan.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface ICardRepository : ICreateRepository<Card>
    {
        Task<List<Card>> GetByIdsAsync(ulong[] ids);
        Task<List<Card>> GetByIdsAsync(ulong[] ids, CardQueryOptions cardQueryOptions);
        Task<List<Card>> GetByCharacterIdAsync(ulong characterId);
        Task<Card> GetByIdAsync(ulong id);
    }
}
