using Microsoft.EntityFrameworkCore;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    public class CardRepository : BaseRepository<Card>, ICardRepository
    {
        private readonly SanakanDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public CardRepository(
            SanakanDbContext dbContext,
            ICacheManager cacheManager)
            : base(dbContext)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task<Card?> GetCardAsync(ulong wid)
        {
            var result = await _dbContext
                .Cards
                .Include(x => x.GameDeck)
                .Include(x => x.ArenaStats)
                .Include(x => x.Tags)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == wid);

            return result;
        }

        public async Task<List<Card>> GetCardsWithTagAsync(string tag)
        {
            var result = await _dbContext
                .Cards
               .Include(x => x.ArenaStats)
               .Include(x => x.Tags)
               .Where(x => x.Tags
                   .Any(c => c.Name.Equals(tag, StringComparison.CurrentCultureIgnoreCase)))
               .AsNoTracking()
               .ToListAsync();

            return result;
        }

        public async Task<List<Card>> GetCardsByCharacterIdAsync(ulong characterId)
        {
            var cards = await _dbContext
                .Cards
                .AsQueryable()
                .AsSplitQuery()
                .Where(x => x.CharacterId == characterId)
                .ToListAsync();

            return cards;
        }

        public async Task<User?> GetUserCardsAsync(ulong shindenUserId)
        {
            var result = await _dbContext
                .Users
               .AsQueryable()
               .Where(x => x.ShindenId == shindenUserId)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.Cards)
                   .ThenInclude(x => x.ArenaStats)
               .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Cards)
                    .ThenInclude(x => x.Tags)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync();

            return result;
        }

        public Task<int> CountByRarityAndSucceedingIdAsync(Rarity rarity, ulong wid)
        {
            return _dbContext.Cards.CountAsync(x => x.Rarity == rarity && x.Id >= wid);
        }

        public async Task<List<Card>> GetAsync(ulong gameDeckId, CardsQueryFilter filter)
        {
            var query = _dbContext
                .Cards
                .AsQueryable()
                .AsSplitQuery()
                .Where(x => x.GameDeckId == gameDeckId)
                .Include(x => x.ArenaStats)
                .Include(x => x.Tags)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                query = query.Where(x => x.Name.Contains(filter.SearchText)
                    || x.Title.Contains(filter.SearchText));
            }

            query = CardsQueryFilter.QueryOrderBy(filter.OrderBy, query);

            var result = await query.ToListAsync();

            if (filter.IncludeTags != null && filter.IncludeTags.Count > 0)
            {
                if (filter.FilterTagsMethod == FilterTagsMethodType.And)
                {
                    foreach (var iTag in filter.IncludeTags)
                    {
                        result = result.Where(x => x.HasTag(iTag)).ToList();
                    }
                }
                else
                {
                    result = result.Where(x => x.HasAnyTag(filter.IncludeTags)).ToList();
                }
            }

            if (filter.ExcludeTags != null)
            {
                foreach (var eTag in filter.ExcludeTags)
                {
                    result = result.Where(x => !x.HasTag(eTag))
                        .ToList();
                }
            }

            return result;
        }

        public Task<List<Card>> GetByCharacterIdAsync(ulong characterId)
        {
            var cards = _dbContext
                .Cards
                .AsQueryable()
                .AsSplitQuery()
                .Where(x => x.CharacterId == characterId)
                .ToListAsync();

            return cards;
        }

        public Task<List<Card>> GetByCharacterIdAsync(ulong characterId, CardQueryOptions cardQueryOptions)
        {
            var query = _dbContext.Cards
               .AsQueryable()
               .Include(x => x.Tags)
               .Include(x => x.GameDeck)
               .Where(x => x.CharacterId == characterId);

            if (cardQueryOptions.IncludeTagList)
            {
                query = query.Include(pr => pr.Tags);
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

            return query.ToListAsync();
        }

        public Task<List<Card>> GetByCharacterIdsAsync(IEnumerable<ulong> characterIds)
        {
            return _dbContext.Cards
                .AsQueryable()
                .Include(x => x.Tags)
                .Include(x => x.GameDeck)
                .AsSplitQuery()
                .Where(x => characterIds.Contains(x.CharacterId))
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<List<Card>> GetByCharactersAndNotInUserGameDeckAsync(ulong userId, IEnumerable<ulong> characterIds)
        {
            return _dbContext.Cards
                .Include(x => x.Tags)
                .Include(x => x.GameDeck)
                .Where(x => x.GameDeckId != userId
                    && characterIds.Any(pr => pr == x.CharacterId))
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

        public Task<List<Card>> GetByGameDeckIdAsync(ulong gameDeckId)
        {
            return _dbContext.Cards
                .AsQueryable()
                .AsSplitQuery()
                .Where(x => x.GameDeckId == gameDeckId)
                .Include(x => x.ArenaStats)
                .Include(x => x.Tags)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<List<Card>> GetByGameDeckIdAsync(ulong gameDeckId, int offset, int count)
        {
            return _dbContext.Cards
                .AsQueryable()
                .AsSplitQuery()
                .Where(x => x.GameDeckId == gameDeckId)
                .Include(x => x.ArenaStats)
                .Include(x => x.Tags)
                .Skip(offset)
                .Take(count)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<Card?> GetByIdAsync(ulong id)
        {
            return _dbContext.Cards
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id)!;
        }

        public Task<Card?> GetByIdAsync(ulong id, CardQueryOptions cardQueryOptions)
        {
            var query = _dbContext.Cards
                .AsNoTracking();

            if (cardQueryOptions.IncludeTagList)
            {
                query = query.Include(pr => pr.Tags);
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

            return query.FirstOrDefaultAsync(x => x.Id == id)!;
        }

        public Task<List<Card>> GetByIdFirstOrLastOwnerAsync(ulong discordUserId)
        {
            return _dbContext.Cards
                .Include(x => x.Tags)
                .Where(x => (x.LastOwnerId == discordUserId || (x.FirstOwnerId == discordUserId
                    && x.LastOwnerId == 0))
                    && x.GameDeckId == 1)
                .ToListAsync();
        }

        public async Task<List<Card>> GetByIdsAsync(IEnumerable<ulong> cardIds)
        {
            var result = await _dbContext.Cards
               .Where(x => cardIds.Contains(x.Id))
               .ToListAsync();

            return result;
        }

        public Task<List<Card>> GetByIdsAsync(IEnumerable<ulong> cardIds, CardQueryOptions cardQueryOptions)
        {
            var query = _dbContext.Cards
               .Where(x => cardIds.Contains(x.Id));

            if (cardQueryOptions.IncludeTagList)
            {
                query = query.Include(pr => pr.Tags);
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