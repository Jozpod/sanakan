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
    public class CardRepository : BaseRepository<Card>, ICardRepository
    {
        private readonly BuildDatabaseContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public CardRepository(
            BuildDatabaseContext dbContext,
            ICacheManager cacheManager) : base(dbContext)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public Task<int> CountByRarityAndSucceedingIdAsync(Rarity rarity, ulong wid)
        {
            return _dbContext.Cards.CountAsync(x => x.Rarity == rarity && x.Id >= wid);
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

        public Task<List<Card>> GetByCharacterIdAsync(ulong characterId, CardQueryOptions cardQueryOptions)
        {
            throw new NotImplementedException();
        }

        public Task<List<Card>> GetByCharacterIdsAsync(IEnumerable<ulong> characterIds)
        {
            return _dbContext.Cards
                .AsQueryable()
                .Include(x => x.TagList)
                .Include(x => x.GameDeck)
                .AsSplitQuery()
                .Where(x => characterIds.Contains(x.Character))
                .AsNoTracking()
                .ToListAsync();
                //.FromCacheAsync(new[] { "users" });
        }

        public Task<List<Card>> GetByCharactersAndNotInUserGameDeckAsync(ulong userId, IEnumerable<ulong> characterIds)
        {
            return _dbContext.Cards
                .Include(x => x.TagList)
                .Include(x => x.GameDeck)
                .Where(x => x.GameDeckId != userId
                    && characterIds.Any(pr => pr == x.Character))
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<List<ulong>> GetByExcludedGameDeckIdsAsync(IEnumerable<ulong> allUsers)
        {
            return _dbContext.Cards
                .AsQueryable()
                .AsSplitQuery()
                .Where(x => !allUsers.Any(id => id == x.GameDeckId))
                .Select(x => x.Id)
                .ToListAsync();
        }

        public Task<List<Card>> GetByGameDeckIdAsync(ulong gameDeckId, int offset, int count)
        {
            return _dbContext.Cards
                .AsQueryable()
                .AsSplitQuery()
                .Where(x => x.GameDeckId == gameDeckId)
                .Include(x => x.ArenaStats)
                .Include(x => x.TagList)
                .Skip(offset)
                .Take(count)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<Card> GetByIdAsync(ulong id)
        {
            return _dbContext.Cards
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<Card> GetByIdAsync(ulong id, CardQueryOptions cardQueryOptions)
        {
            var query = _dbContext.Cards
                .AsNoTracking();

            if(cardQueryOptions.IncludeTagList)
            {
                query = query.Include(pr => pr.TagList);
            }

            if (cardQueryOptions.IncludeArenaStats)
            {
                query = query.Include(pr => pr.ArenaStats);
            }

            if (cardQueryOptions.IncludeGameDeck)
            {
                query = query.Include(pr => pr.GameDeck);
            }

            if (cardQueryOptions.AsSingleQuery)
            {
                query = query.AsSingleQuery();
            }

            if (cardQueryOptions.AsNoTracking)
            {
                query = query.AsNoTracking();
            }

            return query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<Card>> GetByIdFirstOrLastOwnerAsync(ulong userId)
        {
            return _dbContext.Cards
                .Include(x => x.TagList)
                .Where(x => (x.LastIdOwner == userId || (x.FirstIdOwner == userId
                    && x.LastIdOwner == 0))
                    && x.GameDeckId == 1)
                .ToListAsync();
        }

        public async Task<List<Card>> GetByIdsAsync(ulong[] ids)
        {
            var result = await _dbContext.Cards
               .Where(x => ids.Contains(x.Id))
               .ToListAsync();

            return result;
        }

        public Task<List<Card>> GetByIdsAsync(ulong[] ids, CardQueryOptions cardQueryOptions)
        {
            var query = _dbContext.Cards
               .Where(x => ids.Contains(x.Id));

            if (cardQueryOptions.IncludeTagList)
            {
                query = query.Include(pr => pr.TagList);
            }

            if (cardQueryOptions.IncludeArenaStats)
            {
                query = query.Include(pr => pr.ArenaStats);
            }

            if (cardQueryOptions.AsSingleQuery)
            {
                query = query.AsSingleQuery();
            }

            return query.ToListAsync();
        }
    }
}