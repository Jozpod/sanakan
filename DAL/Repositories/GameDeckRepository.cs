using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    internal class GameDeckRepository : BaseRepository<GameDeck>, IGameDeckRepository
    {
        private readonly SanakanDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public GameDeckRepository(
            SanakanDbContext dbContext,
            ICacheManager cacheManager) : base(dbContext)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task<List<GameDeck>> GetCachedPlayersForPVP(ulong ignore = 1)
        {
            var cacheResult = _cacheManager.Get<List<GameDeck>>(CacheKeys.GameDecks);

            if (cacheResult != null)
            {
                return cacheResult.Value ?? new();
            }

            var result = await _dbContext
                .GameDecks
                .AsQueryable()
                .Where(x => x.DeckPower > Constants.MinDeckPower
                    && x.DeckPower < Constants.MaxDeckPower
                    && x.UserId != ignore)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            _cacheManager.Add(CacheKeys.GameDecks, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2),
            });

            return result;
        }

        public async Task<GameDeck?> GetCachedUserGameDeckAsync(ulong userId)
        {
            var key = string.Format(CacheKeys.GameDeckUser, userId);

            var cacheResult = _cacheManager.Get<GameDeck>(key);

            if (cacheResult != null)
            {
                return cacheResult.Value;
            }

            var result = await _dbContext
                .GameDecks
                .AsQueryable()
                .Where(x => x.UserId == userId)
                .Include(x => x.Cards)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            _cacheManager.Add(key, result, CacheKeys.User(userId));

            return result;
        }

        public Task<List<GameDeck>> GetByAnimeIdAsync(ulong animeId)
        {
            return _dbContext.GameDecks
                .Include(x => x.Wishes)
                .Where(x => !x.WishlistIsPrivate
                    && x.Wishes.Any(c => c.Type == WishlistObjectType.Title
                    && c.ObjectId == animeId))
                .ToListAsync();
        }

        public Task<List<GameDeck>> GetByCardIdAndCharacterAsync(ulong cardId, ulong characterId)
        {
            var wishlists = _dbContext.GameDecks
                .Include(x => x.Wishes)
                .AsNoTracking()
                .Where(x => !x.WishlistIsPrivate
                    && (x.Wishes.Any(c => c.Type == WishlistObjectType.Card
                    && c.ObjectId == cardId)
                        || x.Wishes.Any(c => c.Type == WishlistObjectType.Character
                            && c.ObjectId == characterId)))
                .ToListAsync();

            return wishlists;
        }
    }
}
