using Microsoft.EntityFrameworkCore;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly BuildDatabaseContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public CardRepository(
            BuildDatabaseContext dbContext,
            ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public void Add(Card entity)
        {
            _dbContext.Cards.Add(entity);
        }

        public Task<List<Card>> GetByCharacterIdAsync(ulong characterId)
        {
            var cards = _dbContext
                .Cards
                .AsQueryable()
                .AsSplitQuery()
                .Where(x => x.Character == characterId)
                .ToListAsync();

            return cards;
        }

        public Task<Card> GetByIdAsync(ulong id)
        {
            return _dbContext.Cards
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Card>> GetByIdsAsync(ulong[] ids)
        {
            var result = await _dbContext.Cards
               .Where(x => ids.Contains(x.Id))
               .ToListAsync();

            return result;
        }

        public async Task<List<Card>> GetByIdsAsync(ulong[] ids, CardQueryOptions cardQueryOptions)
        {
            var result = await _dbContext.Cards
               .Where(x => ids.Contains(x.Id))
               .ToListAsync();

            return result;
        }

        public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
    }
}