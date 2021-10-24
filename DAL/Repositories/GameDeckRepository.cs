using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Sanakan.Common;
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

        public const double MAX_DECK_POWER = 800;
        public const double MIN_DECK_POWER = 200;

        public async Task<List<GameDeck>> GetCachedPlayersForPVP(ulong ignore = 1)
        {
            var key = "gamedecks";

            var cached = _cacheManager.Get<List<GameDeck>>(key);

            if (cached != null)
            {
                return cached;
            }

            var result = await _dbContext
                .GameDecks
                .AsQueryable()
                .Where(x => x.DeckPower > MIN_DECK_POWER
                    && x.DeckPower < MAX_DECK_POWER
                    && x.UserId != ignore)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            _cacheManager.Add(key, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
            });

            return result;
        }
        public async Task<GameDeck> GetCachedUserGameDeckAsync(ulong userId)
        {
            var key = $"gamedeck-user-{userId}";

            var cached = _cacheManager.Get<GameDeck>(key);

            if (cached != null)
            {
                return cached;
            }

            var result = await _dbContext
                .GameDecks
                .AsQueryable()
                .Where(x => x.UserId == userId)
                .Include(x => x.Cards)
                .AsNoTracking()
                .AsSplitQuery()
                //.FromCacheAsync(new string[] { $"user-{userId}", "users" }))
                .FirstOrDefaultAsync();

            return result;
        }
        public Task<List<GameDeck>> GetByAnimeIdAsync(ulong id)
        {
            return _dbContext.GameDecks
                .Include(x => x.Wishes)
                .Where(x => !x.WishlistIsPrivate
                    && x.Wishes.Any(c => c.Type == WishlistObjectType.Title
                    && c.ObjectId == id))
                .ToListAsync();
        }

        public Task<List<GameDeck>> GetByCardIdAndCharacterAsync(ulong cardId, ulong character)
        {
            var wishlists = _dbContext.GameDecks
                .Include(x => x.Wishes)
                .AsNoTracking()
                .Where(x => !x.WishlistIsPrivate
                    && (x.Wishes.Any(c => c.Type == WishlistObjectType.Card
                    && c.ObjectId == cardId)
                        || x.Wishes.Any(c => c.Type == WishlistObjectType.Character
                            && c.ObjectId == character)))
                .ToListAsync();

            return wishlists;
        }
    }
}
